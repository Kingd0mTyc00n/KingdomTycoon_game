using Pathfinding;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField]  private CharacterData characterData;

    [Header("Health")]
    public float maxHealth;
    public float health;
    public float healThreshold;

    [Header("Attack")]
    public float attackDamage;
    public float lastAttackTime = 0f;
    public float attackSpeed = 1f; // attacks per second

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float arriveTolerance = 0.1f;

    [Header("Map / Target")]
    public Transform currentMapSpawn;   
    public Transform currentEnemy; 
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
        characterObj = GetComponent<PlayerObj>();
    }

    public void SetCharacterData(CharacterData hunterData)
    {
        this.characterData = hunterData;
        maxHealth = hunterData.Health;
        health = hunterData.Health;
        attackDamage = hunterData.Damage;
        healThreshold = 0.1f * maxHealth;
        GetComponent<Character>().SetCharacterData(hunterData);
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

    public NodeState FindNearestEnemy()
    {
        GameObject[] Enemys = GameObject.FindGameObjectsWithTag("Goblin");
        if (Enemys.Length == 0) 
            return NodeState.Failure;

        currentEnemy = Enemys
            .OrderBy(m => Vector2.Distance(transform.position, m.transform.position))
            .First().transform;

        return NodeState.Success;
    }

    public NodeState MoveToEnemy()
    {
        if (currentEnemy == null) return NodeState.Failure;

        characterObj.SetMovePos(currentEnemy.position);

        if (Vector2.Distance(transform.position, currentEnemy.position) <= arriveTolerance)
        {
            characterObj.SetIdle();
            return NodeState.Success;
        }

        return NodeState.Running;
    }

    public NodeState AttackEnemy()
    {
        if (currentEnemy == null) 
            return NodeState.Failure;

        if (!currentEnemy.TryGetComponent<Character>(out var enemy)) 
            return NodeState.Failure;

        float dist = Vector2.Distance(transform.position, currentEnemy.position);
        if (dist > attackRange) 
            return NodeState.Failure;
        if(Time.time - lastAttackTime >= attackSpeed)
        {
            characterObj.SetAttack();
            enemy.TakeDamage(attackDamage);
            lastAttackTime = Time.time;
        }

        if (enemy.IsDead())
        {
            currentEnemy = null;
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

    public CharacterData GetHunterData()
    {
        return characterData;
    }

    public void ClickHunter()
    {
        MenuManager.instance.OpenScreen("Diologue");
        SelectedThisHunter();
    }

    //IEnumerator DealDamage(Character enemy)
    //{
    //    isAttacking = true;
    //    characterObj.SetAttack();
    //    yield return new WaitForSeconds(0.5f);
    //    enemy.TakeDamage(attackDamage);
    //    isAttacking = false;
    //}
}
