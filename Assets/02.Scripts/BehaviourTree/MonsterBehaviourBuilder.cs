using System.Collections.Generic;
using UnityEngine;

public class MonsterBehaviourBuilder : MonoBehaviour
{
    public BehaviourTreeRunner runner;
    private CharacterController hunter;

    void Start()
    {
        hunter = GetComponent<CharacterController>();
        if (runner == null) runner = GetComponent<BehaviourTreeRunner>();
        var roamSeq = new Sequence(new List<Node> {
            new ActionNode(() => hunter.IdleMovement()),
            new WaitNode(2f)
        });
        var huntSeq = new Sequence(new List<Node> {
            new ActionNode(() => hunter.FindNearestEnemy()),
            new ActionNode(() => hunter.MoveToEnemy()),
            new ActionNode(() => hunter.AttackEnemy())
        });

        runner.Root = new Selector(new List<Node> {roamSeq ,huntSeq });
    }
}
