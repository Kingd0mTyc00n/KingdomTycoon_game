using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequence : Node
{
    private List<Node> nodes;
    private int current = 0;

    public Sequence(List<Node> nodes)
    {
        this.nodes = nodes;
    }
    public override NodeState Tick()
    {
        while (current < nodes.Count)
        {
            var result = nodes[current].Tick();

            if (result == NodeState.Running)
            {
                state = NodeState.Running;
                return state;
            }

            if (result == NodeState.Failure)
            {
                current = 0;
                ResetNode();
                state = NodeState.Failure;
                return state;
            }

            current++;
        }

        current = 0;
        ResetNode();
        state = NodeState.Success;
        return state;
    }

    private void ResetNode()
    {
        foreach (var c in nodes) c.Reset();
    }

    public override void Reset()
    {
        base.Reset();
        current = 0;
        ResetNode();
    }
}

public class Selector : Node
{
    private List<Node> nodes;
    private int current = 0;

    public Selector(List<Node> nodes)
    {
        this.nodes = nodes;
    }

    public override NodeState Tick()
    {
        while (current < nodes.Count)
        {
            Debug.Log("Selector Tick: " + current);
            var result = nodes[current].Tick();

            if (result == NodeState.Running)
            {
                state = NodeState.Running;
                return state;
            }

            current++;
            if (result == NodeState.Success)
            {
                current = 0;
                ResetNode();
                state = NodeState.Success;
                return state;
            }
        }

        current = 0;
        ResetNode();
        state = NodeState.Failure;
        return state;
    }

    private void ResetNode()
    {
        foreach (var c in nodes) c.Reset();
    }

    public override void Reset()
    {
        base.Reset();
        current = 0;
        ResetNode();
    }
}
public class Repeater : Node
{
    private Node child;

    public Repeater(Node child)
    {
        this.child = child;
    }

    public override NodeState Tick()
    {
        var result = child.Tick();
        if (result != NodeState.Running)
        {
            child.Reset();
        }
        return NodeState.Running;
    }

    public override void Reset()
    {
        base.Reset();
        child.Reset();
    }
}
