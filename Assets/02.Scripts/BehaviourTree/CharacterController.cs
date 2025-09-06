using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField] private CharacterData characterData;

    [SerializeField] private HealthBar healthBar;
    private CharacterCombat characterCombat;


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

    public float spawnRadius = 1.5f;
    public float forceAmount = 5f;

    [Header("Territory")]
    public PolygonCollider2D territoryZone;  //check hunter zone

    public List<AbilityInstance> skills = new List<AbilityInstance>();

    private void Start()
    {
        healthBar = GetComponentInChildren<HealthBar>();
        characterObj = GetComponent<PlayerObj>();
        characterCombat = GetComponent<CharacterCombat>();
        if (characterCombat != null)
        {
            skills = characterCombat.GetAbilityInstances();
        }

        if (characterData is EnemyData)
            territoryZone = townTransform.GetComponent<PolygonCollider2D>();
    }

    public void SetCharacterData(CharacterData hunterData)
    {
        characterData = hunterData;
        healthBar.SetMaxHealth(characterData.Health);
        attackDamage = hunterData.Damage;
        GetComponent<Character>().SetCharacterData(hunterData);
    }

    public void SetTown(Transform town)
    {
        townTransform = town;
    }

    public void SelectedThisHunter()
    {
        Camera.main.GetComponent<CameraFollow>().SetCurrentHunter(gameObject.transform);
    }

    public bool IsHealthLow() => healthBar.IsHealthLow();

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

    public void DisplayItemsToUI(HunterInventory hunterInventory)
    {
        UIInventoryPage.Instance.ShowInventory(hunterInventory);
    }

    public NodeState IdleMovement()
    {
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
            return NodeState.Failure;
        }

        return NodeState.Running;
    }

    public NodeState MoveToMapSpawn()
    {
        if (currentMapSpawn == null)
            return NodeState.Failure;

        characterObj.SetMovePos(currentMapSpawn.position);

        if (Vector2.Distance(transform.position, currentMapSpawn.position) <= arriveTolerance)
        {
            characterObj.SetIdle();
            hasPlayerCommand = false;
            return NodeState.Success;
        }

        return NodeState.Running;
    }

    public NodeState FindNearestTarget()
    {
        if (hasPlayerCommand && mapSelected && currentMapSpawn != null)
        {
            currentEnemy = null;
            return NodeState.Failure;
        }

        GameObject[] Target = new GameObject[] { };

        if (characterData is HunterData)
        {
            switch (currentMapSpawn.name)
            {
                case "OrcZone":
                    Target = GameObject.FindGameObjectsWithTag("Orc");
                    break;
                case "UndeadZone":
                    Target = GameObject.FindGameObjectsWithTag("Undead");
                    break;
                case "DevilZone":
                    Target = GameObject.FindGameObjectsWithTag("Devil");
                    break;
            }
        }

        if (characterData is EnemyData)
        {
            Target = GameObject.FindGameObjectsWithTag("Hunter");
        }


        if (territoryZone != null)
        {
            Target = Target
                .Where(t => territoryZone.OverlapPoint(t.transform.position))
                .ToArray();
        }

        if (Target.Length == 0)
            return NodeState.Failure;

        currentEnemy = Target
            .OrderBy(m => Vector2.Distance(transform.position, m.transform.position))
            .First().transform;

        return NodeState.Success;
    }


    public NodeState MoveToTarget()
    {
        if (hasPlayerCommand && mapSelected && currentMapSpawn != null)
        {
            currentEnemy = null;
            return NodeState.Failure;
        }

        if (currentEnemy == null) return NodeState.Failure;

        characterObj.SetMovePos(currentEnemy.position);

        if (Vector2.Distance(transform.position, currentEnemy.position) <= arriveTolerance)
        {
            characterObj.SetIdle();
            return NodeState.Success;
        }

        if (territoryZone != null)
        {
            if (!territoryZone.OverlapPoint(currentEnemy.position))
            {
                currentEnemy = null;
                return NodeState.Failure;
            }
        }

        return NodeState.Running;
    }

    public NodeState AttackTarget()
    {
        if (hasPlayerCommand && mapSelected && currentMapSpawn != null)
        {
            currentEnemy = null;
            return NodeState.Failure;
        }

        if (currentEnemy == null)
            return NodeState.Failure;

        if (!currentEnemy.TryGetComponent<CharacterController>(out var enemy))
            return NodeState.Failure;

        float dist = Vector2.Distance(transform.position, currentEnemy.position);
        if (dist > attackRange)
            return NodeState.Failure;

        if (Time.time - lastAttackTime >= attackSpeed)
        {
            if (characterData is HunterData && skills.Count > 0)
            {
                int index = Random.Range(0, skills.Count);
                AbilityInstance skill = skills[index];

                if (skill.CanUse())
                {
                    skill.Use(transform, currentEnemy.transform);
                    lastAttackTime = Time.time;
                    return NodeState.Running;
                }
            }

            characterObj.SetAttack();
            enemy.TakeDamage(attackDamage);
            healthBar.UpdateProgressBar();
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
            if (healthBar.maxHealth < characterData.Health)
            {
                healthBar.Heal(5);
            }
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

    public void TakeDamage(float damage)
    {
        healthBar.currentHealth -= damage;
        if (healthBar.currentHealth <= 0)
        {
            healthBar.currentHealth = 0;
            Die();
        }

        healthBar.UpdateProgressBar();
    }


    private void Die()
    {
        if (characterData is EnemyData enemyData)
        {
            DropItem(enemyData.ItemsIsDrop);
            var spawner = townTransform?.GetComponent<EnemySpawner>();
            if (spawner != null)
            {
                spawner.OnEnemyDied();
            }

        }


        ObjectPoolAddressable.Instance.ReturnToPool(gameObject);
    }


    public bool IsDead() => healthBar.currentHealth <= 0;


    private void DropItem(List<ItemData> items)
    {
        for (int i = 0; i < items.Count; i++)
        {
            GameObject item = Instantiate(GameResources.instance.dropItem, transform.position, Quaternion.identity);
            item.GetComponent<Item>().SetData(items[i]);
            Vector2 randomDirection = UnityEngine.Random.insideUnitCircle.normalized;

            Rigidbody2D rb = item.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.AddForce(randomDirection * forceAmount, ForceMode2D.Impulse);
                rb.linearDamping = 3f;
            }
            currentEnemy.GetComponent<PickUpSystem>().PickUpItem(item.GetComponent<Item>());
        }

    }

    public HunterData GetHunterData()
    {
        return characterData as HunterData;
    }

    public void ClickHunter()
    {
        MenuManager.instance.OpenScreen("Diologue");
        SelectedThisHunter();
        //SelectedHunter.Instance.SelectedHunterObj(this);
    }
}
