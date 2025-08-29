using UnityEngine;
using System.Linq;

public class HunterController : MonoBehaviour
{
    [SerializeField]  private HunterData hunterData;

    [Header("Health")]
    public float maxHealth;
    public float health;
    public float healThreshold;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float arriveTolerance = 0.1f;

    [Header("Map / Target")]
    public Transform currentMapSpawn;   
    public Transform currentMonster; 
    public bool mapSelected = false;

    [Header("Town")]
    public Transform townTransform;

    [Header("Combat")]
    public float attackRange = 1f;
    public float dps = 10f;

    private void Start()
    {
        SetHunterData(UserData.UserDeepData.HuntersData[0]);
    }

    public void SetHunterData(HunterData hunterData)
    {
        this.hunterData = hunterData;
        maxHealth = hunterData.Health;
        health = hunterData.Health;
        healThreshold = 0.1f * maxHealth;
    }

    public void SelectedThisHunter()
    {
        Camera.main.GetComponent<CameraFollow>().SetCurrentHunter(gameObject.transform);
    }

    public bool IsHealthLow() => health <= healThreshold;

    public NodeState WaitForMapSelection()
    {
        return (mapSelected && currentMapSpawn != null) ? NodeState.Success : NodeState.Running;
    }

    public NodeState MoveToMapSpawn()
    {
        if (currentMapSpawn == null) return NodeState.Failure;

        transform.position = Vector2.MoveTowards(
            transform.position,
            currentMapSpawn.position,
            moveSpeed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, currentMapSpawn.position) <= arriveTolerance)
            return NodeState.Success;

        return NodeState.Running;
    }

    public NodeState FindNearestMonster()
    {
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Enemy");
        if (monsters.Length == 0) 
            return NodeState.Failure;

        currentMonster = monsters
            .OrderBy(m => Vector2.Distance(transform.position, m.transform.position))
            .First().transform;

        return NodeState.Success;
    }

    public NodeState MoveToMonster()
    {
        if (currentMonster == null) return NodeState.Failure;

        transform.position = Vector2.MoveTowards(
            transform.position,
            currentMonster.position,
            moveSpeed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, currentMonster.position) <= arriveTolerance)
            return NodeState.Success;

        return NodeState.Running;
    }

    public NodeState AttackMonster()
    {
        if (currentMonster == null) 
            return NodeState.Failure;

        if (!currentMonster.TryGetComponent<Enemy>(out var enemy)) 
            return NodeState.Failure;

        float dist = Vector2.Distance(transform.position, currentMonster.position);
        if (dist > attackRange) 
            return NodeState.Failure;

        enemy.TakeDamage(dps * Time.deltaTime);

        if (enemy.IsDead())
        {
            currentMonster = null;
            return NodeState.Success;
        }

        if (IsHealthLow()) 
            return NodeState.Failure;

        return NodeState.Running;
    }

    public NodeState GoToTownAndHeal()
    {
        if (townTransform == null) return NodeState.Failure;

        transform.position = Vector2.MoveTowards(
            transform.position,
            townTransform.position,
            moveSpeed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, townTransform.position) <= arriveTolerance)
        {
            health = maxHealth;
            mapSelected = false; 
            return NodeState.Success;
        }

        return NodeState.Running;
    }
    public void SelectMap(Transform spawnPoint)
    {
        currentMapSpawn = spawnPoint;
        mapSelected = true;
    }

    public HunterData GetHunterData()
    {
        return hunterData;
    }
}
