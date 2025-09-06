using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public Transform townTransform;
    private Vector2 roamTarget;
    private bool hasRoamTarget = false;
    private bool isResting = false;
    public float arriveTolerance = 0.1f;
    public float restTime = 2f;
    public PlayerObj characterObj;

    void Start()
    {
        characterObj = GetComponent<PlayerObj>();
        StartCoroutine(IdleMovementCoroutine());
    }

    private IEnumerator IdleMovementCoroutine()
    {
        while (true)
        {
            NodeState result = IdleMovement();

            if (result == NodeState.Success)
            {
                isResting = true;
                characterObj.SetMovePos(transform.position);
                characterObj.SetIdle();
                Debug.Log("[Player] Resting for " + restTime + " seconds");
                yield return new WaitForSeconds(restTime);
                isResting = false;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    public NodeState IdleMovement()
    {
        if (isResting) return NodeState.Running;

        if (townTransform == null) return NodeState.Failure;

        var poly = townTransform.GetComponent<PolygonCollider2D>();
        if (poly == null) return NodeState.Failure;

        if (!hasRoamTarget)
        {
            if (!TryGetRandomPointInPolygon(poly, out roamTarget))
                return NodeState.Failure;

            hasRoamTarget = true;
            characterObj.SetMovePos(roamTarget);
            Debug.Log($"[Player] Moving to new target: {roamTarget}");
        }

        if (Vector2.Distance(transform.position, roamTarget) <= arriveTolerance)
        {
            hasRoamTarget = false;
            Debug.Log("[Player] Arrived at target, starting rest");
            return NodeState.Success;
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
}
