using UnityEngine;

public class BehaviourTreeRunner : MonoBehaviour
{
    public Node Root;

    void Update()
    {
        if (Root != null)
            Root.Tick();
    }
}