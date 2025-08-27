using System.Collections.Generic;
using UnityEngine;

public class HunterBehaviourBuilder : MonoBehaviour
{
    public BehaviourTreeRunner runner;
    private HunterController hunter;

    void Start()
    {
        hunter = GetComponent<HunterController>();
        if (runner == null) runner = GetComponent<BehaviourTreeRunner>();

        var healSeq = new Sequence(new List<Node> {
            new ConditionNode(() => hunter.IsHealthLow()),
            new ActionNode(() => hunter.GoToTownAndHeal())
        });

        var huntSeq = new Sequence(new List<Node> {
            new ActionNode(() => hunter.WaitForMapSelection()),
            new ActionNode(() => hunter.MoveToMapSpawn()),
            new ActionNode(() => hunter.FindNearestMonster()),
            new ActionNode(() => hunter.MoveToMonster()),
            new ActionNode(() => hunter.AttackMonster())
        });

        runner.Root = new Selector(new List<Node> { healSeq, huntSeq });
    }
}
