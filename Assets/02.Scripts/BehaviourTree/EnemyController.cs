using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyController : BaseCharacterController
{
    [Header("Enemy Specific")]
    public Transform townTransform;
    public PolygonCollider2D territoryZone;

    public HunterController currentAttacker;

    [Header("Patrol")]
    private Vector2 roamTarget;
    private bool hasRoamTarget = false;

    protected void Start()
    {

        // Auto-setup territory zone from town
        if (townTransform != null)
        {
            territoryZone = townTransform.GetComponent<PolygonCollider2D>();
        }

        healthBar = GetComponentInChildren<HealthBar>();
        characterObj = GetComponent<PlayerObj>();
    }

    public void SetTown(Transform town)
    {
        townTransform = town;
        if (townTransform != null)
        {
            territoryZone = townTransform.GetComponent<PolygonCollider2D>();
        }
    }

    #region Patrol Movement
    public override NodeState IdleMovement()
    {
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
            Debug.Log("[Enemy] Arrived patrol target");
            return NodeState.Success; // Enemy continues patrolling
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

    #region Combat - Enemy specific
    public override NodeState FindTarget()
    {
        GameObject[] hunters = GameObject.FindGameObjectsWithTag("Hunter");

        // Filter hunters within territory
        if (territoryZone != null)
        {
            hunters = hunters
                .Where(h => territoryZone.OverlapPoint(h.transform.position))
                .ToArray();
        }

        if (hunters.Length == 0)
            return NodeState.Failure;

        var availableHunters = hunters
            .Select(h => h.GetComponent<HunterController>())
            .Where(hc => hc != null && hc.currentTarget == null)
            .OrderBy(hc => Vector2.Distance(transform.position, hc.transform.position))
            .ToList();

        if (availableHunters.Count == 0)
            return NodeState.Failure;

        var chosenHunter = availableHunters.First();
        currentTarget = chosenHunter.transform;
        chosenHunter.currentTarget = this.transform;

        return NodeState.Success;
    }

    public void ReleaseTarget()
    {
        if (currentTarget != null && currentTarget.TryGetComponent<HunterController>(out var hunter))
        {
            if (hunter.currentTarget == this.transform)
                hunter.currentTarget = null;
        }
        currentTarget = null;
    }

    public override NodeState MoveToTarget()
    {
        if (currentTarget == null) return NodeState.Failure;

        // Check if target is still in territory
        if (territoryZone != null && !territoryZone.OverlapPoint(currentTarget.position))
        {
            currentTarget = null;
            return NodeState.Failure;
        }

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
        if (currentTarget == null)
            return NodeState.Failure;

        if (!currentTarget.TryGetComponent<BaseCharacterController>(out var hunter))
            return NodeState.Failure;

        float dist = Vector2.Distance(transform.position, currentTarget.position);
        if (dist > attackRange)
            return NodeState.Failure;

        // Check if still in territory
        if (territoryZone != null && !territoryZone.OverlapPoint(currentTarget.position))
        {
            ReleaseTarget();
            return NodeState.Failure;
        }

        if (CanAttack())
        {
            PerformAttack(hunter);
        }

        if (hunter.IsDead())
        {
            ReleaseTarget();
            return NodeState.Success;
        }

        return NodeState.Running;
    }
    #endregion

    #region Death & Items
    protected override void Die()
    {
        if (characterData is EnemyData enemyData)
        {
            DropItems(enemyData.ItemsIsDrop);
            NotifySpawner();
        }

        base.Die();
    }

    private void DropItems(List<ItemData> items)
    {
        if (items == null || items.Count == 0) return;

        foreach (var itemData in items)
        {
            GameObject item = Instantiate(GameResources.instance.dropItem, transform.position, Quaternion.identity);
            item.GetComponent<Item>().SetData(itemData);

            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            Rigidbody2D rb = item.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.AddForce(randomDirection * forceAmount, ForceMode2D.Impulse);
                rb.linearDamping = 3f;
            }

            // Auto pickup by killer
            if (currentTarget != null && currentTarget.TryGetComponent<PickUpSystem>(out var pickupSystem))
            {
                pickupSystem.PickUpItem(item.GetComponent<Item>());
            }
        }
    }

    private void NotifySpawner()
    {
        var spawner = townTransform?.GetComponent<EnemySpawner>();
        spawner?.OnEnemyDied();
    }
    #endregion

    #region Territory Management
    public bool IsInTerritory(Vector3 position)
    {
        return territoryZone == null || territoryZone.OverlapPoint(position);
    }

    public void SetTerritory(PolygonCollider2D territory)
    {
        territoryZone = territory;
    }
    #endregion
}
