using UnityEngine;

public class WaitNode : Node
{
    private float waitTime;
    private float startTime;
    private bool started = false;

    public WaitNode(float seconds)
    {
        waitTime = seconds;
    }

    public override NodeState Tick()
    {
        Debug.Log("WaitNode Tick:" + (Time.time - startTime));
        if (!started)
        {
            started = true;
            startTime = Time.time;
        }

        if (Time.time - startTime >= waitTime)
        {
            started = false;
            return NodeState.Success;
        }

        return NodeState.Running;
    }

    public override void Reset()
    {
        base.Reset();
        started = false;
    }
}
