using FishNet.Example.Scened;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform target;
    public Vector3 offset;
    public float smoothSpeed = 0.125f;
    private bool isFollowTarget = false;

    public void SetCurrentHunter(Transform hunter)
    {
        isFollowTarget = true;
        target = hunter;
    }

    public void CameraFree()
    {
        isFollowTarget = false;
    }

    void LateUpdate()
    {
        if (isFollowTarget)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}
