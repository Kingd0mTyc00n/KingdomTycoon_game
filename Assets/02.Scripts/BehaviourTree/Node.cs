using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum NodeState
{
    Running,
    Success,
    Failure
}
public abstract class Node
{
    public NodeState state = NodeState.Running;
    public abstract NodeState Tick();
    public virtual void Reset()
    {
        state = NodeState.Running;
    }
}
