using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Controller cho Hunter characters
/// Chứa logic đặc thù của Hunter như map selection, skills, inventory
/// </summary>
public class HunterController : BaseCharacterController
{
    [Header("Hunter Specific")]
    public Transform currentMapSpawn;
    public bool mapSelected = false;
    public bool hasPlayerCommand = false;

    [Header("Town")]
    public Transform townTransform;

    [Header("Roaming")]
    private Vector2 roamTarget;
    private bool hasRoamTarget = false;
    private bool isMovingByHunter = false;

    [Header("Skills")]
    public List<AbilityInstance> skills = new List<AbilityInstance>();

    protected void Start()
    {
        // Khởi tạo skills từ characterCombat
        if (characterCombat != null)
        {
            skills = characterCombat.GetAbilityInstances();
        }
    }

    public void SetTown(Transform town)
    {
        townTransform = town;
    }

    public void SelectedThisHunter()
    {
        Camera.main.GetComponent<CameraFollow>().SetCurrentHunter(gameObject.transform);
    }

    #region Map Selection
    public NodeState WaitForMapSelection()
    {
        Debug.Log($"[Hunter] WaitForMapSelection - mapSelected: {mapSelected}, currentMapSpawn: {(currentMapSpawn != null ? currentMapSpawn.name : "null")}");
        return (mapSelected && currentMapSpawn != null) ? NodeState.Success : NodeState.Failure;
    }

    public void SelectMap(Transform spawnPoint)
    {
        currentMapSpawn = spawnPoint;
        mapSelected = true;
        hasPlayerCommand = true;
        hasRoamTarget = false;
        isMovingByHunter = false;
    }

    public NodeState MoveToMapSpawn()
    {
        if (currentMapSpawn == null)
            return NodeState.Failure;

        MoveToPosition(currentMapSpawn.position);

        if (IsAtPosition(currentMapSpawn.position))
        {
            StopMovement();
            hasPlayerCommand = false;
            return NodeState.Success;
        }

        return NodeState.Running;
    }
    #endregion

    #region Roaming
    public override NodeState IdleMovement()
    {
        Debug.Log($"[Hunter] IdleMovement - hasPlayerCommand: {hasPlayerCommand}, mapSelected: {mapSelected}, currentMapSpawn: {(currentMapSpawn != null ? currentMapSpawn.name : "null")}, hasRoamTarget: {hasRoamTarget}, isMovingByHunter: {isMovingByHunter}");
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
            MoveToPosition(roamTarget);
        }

        if (IsAtPosition(roamTarget))
        {
            StopMovement();
            hasRoamTarget = false;
            isMovingByHunter = false;
            Debug.Log("[Hunter] Arrived roam target");
            return NodeState.Failure;
        }

        return NodeState.Running;
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
    #endregion

    #region Combat - Hunter specific
    public override NodeState FindTarget()
    {
        if (hasPlayerCommand && mapSelected && currentMapSpawn != null)
        {
            currentTarget = null;
            return NodeState.Failure;
        }

        GameObject[] targets = GetTargetsBasedOnMap();

        if (targets.Length == 0)
            return NodeState.Failure;

        // Chỉ chọn enemy chưa bị hunter khác tấn công
        var availableTargets = targets
            .Select(t => t.GetComponent<EnemyController>())
            .Where(e => e != null && e.currentAttacker == null)
            .OrderBy(e => Vector2.Distance(transform.position, e.transform.position))
            .ToList();

        if (availableTargets.Count == 0)
            return NodeState.Failure;

        var chosenEnemy = availableTargets.First();
        currentTarget = chosenEnemy.transform;
        chosenEnemy.currentAttacker = this;

        return NodeState.Success;
    }

    private GameObject[] GetTargetsBasedOnMap()
    {
        if (currentMapSpawn == null) return new GameObject[0];

        return currentMapSpawn.name switch
        {
            "OrcZone" => GameObject.FindGameObjectsWithTag("Orc"),
            "UndeadZone" => GameObject.FindGameObjectsWithTag("Undead"),
            "DevilZone" => GameObject.FindGameObjectsWithTag("Devil"),
            _ => new GameObject[0]
        };
    }

    public override NodeState MoveToTarget()
    {
        if (hasPlayerCommand && mapSelected && currentMapSpawn != null)
        {
            currentTarget = null;
            return NodeState.Failure;
        }

        if (currentTarget == null) return NodeState.Failure;

        MoveToPosition(currentTarget.position);

        if (IsAtPosition(currentTarget.position))
        {
            StopMovement();
            return NodeState.Success;
        }

        return NodeState.Running;
    }

    public override NodeState AttackTarget()
    {
        if (hasPlayerCommand && mapSelected && currentMapSpawn != null)
        {
            ReleaseTarget();
            return NodeState.Failure;
        }

        if (currentTarget == null)
            return NodeState.Failure;

        if (!currentTarget.TryGetComponent<BaseCharacterController>(out var enemy))
            return NodeState.Failure;

        float dist = Vector2.Distance(transform.position, currentTarget.position);
        if (dist > attackRange)
            return NodeState.Failure;

        if (CanAttack())
        {
            // Try to use skills first
            if (TryUseSkill())
            {
                lastAttackTime = Time.time;
                return NodeState.Running;
            }

            // Fallback to normal attack
            PerformAttack(enemy);
        }

        if (enemy.IsDead())
        {
            ReleaseTarget();
            return NodeState.Success;
        }

        if (IsHealthLow())
        {
            ReleaseTarget();
            return NodeState.Failure;
        }

        return NodeState.Running;
    }

    private bool TryUseSkill()
    {
        if (skills.Count == 0) return false;

        int index = Random.Range(0, skills.Count);
        AbilityInstance skill = skills[index];

        if (skill.CanUse())
        {
            skill.Use(transform, currentTarget.transform);
            return true;
        }

        return false;
    }

    public void ReleaseTarget()
    {
        if (currentTarget != null && currentTarget.TryGetComponent<EnemyController>(out var enemy))
        {
            if (enemy.currentAttacker == this)
                enemy.currentAttacker = null;
        }
        currentTarget = null;
    }
    #endregion

    #region Healing
    public NodeState GoToTownAndHeal()
    {
        if (townTransform == null) return NodeState.Failure;

        MoveToPosition(townTransform.position);

        if (IsAtPosition(townTransform.position))
        {
            if (healthBar.currentHealth < characterData.Health)
            {
                healthBar.Heal(5);
                Debug.Log($"[Hunter] Healed at town. Health: {healthBar.currentHealth}/{characterData.Health}");
            }
            mapSelected = false;
            Debug.Log($"[Hunter] Finished healing, set mapSelected = false");
            StopMovement();
            return NodeState.Success;
        }

        return NodeState.Running;
    }
    #endregion

    #region UI & Inventory
    public void DisplayItemsToUI(HunterInventory hunterInventory)
    {
        UIInventoryPage.Instance.ShowInventory(hunterInventory);
    }

    public HunterData GetHunterData()
    {
        return characterData as HunterData;
    }

    public void ClickHunter()
    {
        MenuManager.instance.OpenScreen("Diologue");
        SelectedThisHunter();
        SelectedHunter.Instance.SelectedHunterObj(this);
    }
    #endregion
}
