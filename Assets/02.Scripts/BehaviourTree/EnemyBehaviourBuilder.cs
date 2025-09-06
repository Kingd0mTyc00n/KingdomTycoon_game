using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviourBuilder : MonoBehaviour
{
    public BehaviourTreeRunner runner;
    private EnemyController enemy;

    void Start()
    {
        if (runner == null) runner = GetComponent<BehaviourTreeRunner>();

        // Initialize enemy reference
        enemy = GetComponent<EnemyController>();
        if (enemy == null)
        {
            Debug.LogError($"[EnemyBehaviourBuilder] No EnemyController found on {gameObject.name}");
            return;
        }

        // Enemy behavior tree: Simple patrol and attack pattern
        var combatSeq = new Sequence(new List<Node> {
            new ActionNode(() => enemy.FindTarget()),
            new ActionNode(() => enemy.MoveToTarget()),
            new ActionNode(() => enemy.AttackTarget())
        });

        var patrolSeq = new Sequence(new List<Node> {
            new ActionNode(() => enemy.IdleMovement()),
            new WaitNode(3f) // Wait longer between patrol points
        });

        // Priority: Combat first, then patrol
        runner.Root = new Selector(new List<Node> { combatSeq, patrolSeq });
    }
}
