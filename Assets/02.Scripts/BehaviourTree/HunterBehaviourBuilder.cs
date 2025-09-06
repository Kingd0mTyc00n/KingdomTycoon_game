using System.Collections.Generic;
using UnityEngine;

public class HunterBehaviourBuilder : MonoBehaviour
{
    public BehaviourTreeRunner runner;
    private HunterController hunter;

    void Start()
    {
        if (runner == null)
            runner = GetComponent<BehaviourTreeRunner>();

        // Initialize hunter reference
        hunter = GetComponent<HunterController>();

        var healSeq = new Sequence(new List<Node> {
            new ConditionNode(() => hunter.IsHealthLow()),
            new ActionNode(() => hunter.GoToTownAndHeal())
        });
        var roamSeq = new Sequence(new List<Node> {
            new ActionNode(() => hunter.IdleMovement()),
            new WaitNode(2f)
        });
        var huntSeq = new Sequence(new List<Node> {
            new ActionNode(() => hunter.WaitForMapSelection()),
            new ActionNode(() => hunter.MoveToMapSpawn()),
            new ActionNode(() => hunter.FindTarget()),
            new ActionNode(() => hunter.MoveToTarget()),
            new ActionNode(() => hunter.AttackTarget()),
        });

        runner.Root = new Selector(new List<Node> { healSeq, roamSeq, huntSeq });
    }
}
