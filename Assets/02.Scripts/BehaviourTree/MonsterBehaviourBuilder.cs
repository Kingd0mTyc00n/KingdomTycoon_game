using System.Collections.Generic;
using UnityEngine;

public class MonsterBehaviourBuilder : MonoBehaviour
{
    public BehaviourTreeRunner runner;
    private CharacterController monster;

    void Start()
    {
        monster = GetComponent<CharacterController>();
        if (runner == null) runner = GetComponent<BehaviourTreeRunner>();
        var roamSeq = new Sequence(new List<Node> {
            new ActionNode(() => monster.IdleMovement()),
            new WaitNode(2f)
        });
        var huntSeq = new Sequence(new List<Node> {
            new ActionNode(() => monster.FindNearestTarget()),
            new ActionNode(() => monster.MoveToTarget()),
            new ActionNode(() => monster.AttackTarget())
        });

        runner.Root = new Selector(new List<Node> {roamSeq ,huntSeq });
    }
}
