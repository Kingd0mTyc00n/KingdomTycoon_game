using UnityEngine;

public class DestroyParticle : MonoBehaviour
{
    public void Destroy()
    {
        Destroy(transform.parent.gameObject);
    }
}
