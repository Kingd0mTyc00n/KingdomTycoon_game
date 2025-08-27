using System;

public class ConditionNode : Node
{
    private Func<bool> condition;

    public ConditionNode(Func<bool> condition)
    {
        this.condition = condition;
    }

    public override NodeState Tick()
    {
        return condition() ? NodeState.Success : NodeState.Failure;
    }
}

public class ActionNode : Node
{
    private Func<NodeState> action;

    public ActionNode(Func<NodeState> action)
    {
        this.action = action;
    }

    public override NodeState Tick()
    {
        return action();
    }
}