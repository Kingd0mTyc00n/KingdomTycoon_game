using Pathfinding;
using System.Linq;
using TMPro;
using UnityEngine;

public class CharacterController : MonoBehaviour
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

    [Header("Animation")]
    public PlayerObj characterObj;

    [Header("BeahaviourPara")]
    public bool hasPlayerCommand = false;
    private Vector2 roamTarget;
    private bool hasRoamTarget = false;

    private Vector2 currentMoveTarget;
    private bool isMovingByHunter = false;

    private void Start()
    {
        SetHunterData(UserData.UserDeepData.HuntersData[0]);
        characterObj = GetComponent<PlayerObj>();
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

    private bool TryGetRandomPointInPolygon(PolygonCollider2D poly, out Vector2 point, int maxTries = 30)
    {
        var b = poly.bounds;
        for (int i = 0; i < maxTries; i++)
        {
            float x = Random.Range(b.min.x, b.max.x);
            float y = Random.Range(b.min.y, b.max.y);
            var p = new Vector2(x, y);
            if (poly.OverlapPoint(p))
            {
                point = p;
                return true;
            }
        }
        point = default;
        return false;
    }

    public NodeState IdleMovement()
    {
        Debug.Log("[Hunter] IdleMovement");
        if (hasPlayerCommand || (mapSelected && currentMapSpawn != null))
            return NodeState.Failure;

        if (townTransform == null) return NodeState.Failure;

        var poly = townTransform.GetComponent<PolygonCollider2D>();
        if (poly == null) return NodeState.Failure;

        if (!hasRoamTarget)
        {
            if (!TryGetRandomPointInPolygon(poly, out roamTarget))
                return NodeState.Failure;

            hasRoamTarget = true;
            characterObj.SetMovePos(roamTarget);
        }
        if (Vector2.Distance(transform.position, roamTarget) <= arriveTolerance)
        {
            characterObj.SetIdle();
            hasRoamTarget = false;
            isMovingByHunter = false;
            Debug.Log("[Hunter] Arrived roam target");
            return NodeState.Success;
        }

        return NodeState.Running;
    } 

    public NodeState MoveToMapSpawn()
    {
        if (currentMapSpawn == null) return NodeState.Failure;

        characterObj.SetMovePos(currentMapSpawn.position);

        if (Vector2.Distance(transform.position, currentMapSpawn.position) <= arriveTolerance)
        {
            characterObj.SetIdle();
            hasPlayerCommand = false;
            return NodeState.Success;
        }

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

        characterObj.SetMovePos(currentMonster.position);

        if (Vector2.Distance(transform.position, currentMonster.position) <= arriveTolerance)
        {
            characterObj.SetIdle();
            return NodeState.Success;
        }

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

        characterObj.SetAttack();
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

        characterObj.SetMovePos(townTransform.position);


        if (Vector2.Distance(transform.position, townTransform.position) <= arriveTolerance)
        {
            health = maxHealth;
            mapSelected = false;
            characterObj.SetIdle();
            return NodeState.Success;
        }

        return NodeState.Running;
    }
    public void SelectMap(Transform spawnPoint)
    {
        currentMapSpawn = spawnPoint;
        mapSelected = true;
        hasPlayerCommand = true;

        hasRoamTarget = false;
        isMovingByHunter = false;
    }

    public HunterData GetHunterData()
    {
        return hunterData;
    }
}
