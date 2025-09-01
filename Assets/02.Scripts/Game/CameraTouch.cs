using UnityEngine;

public class CameraTouch : MonoBehaviour
{
    public float dragSpeed = 1f;     
    public Vector2 minBounds;            
    public Vector2 maxBounds;             

    private Vector3 touchStart;

    void Update()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                touchStart = Camera.main.ScreenToWorldPoint(touch.position);
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector3 direction = touchStart - Camera.main.ScreenToWorldPoint(touch.position);
                Vector3 newPos = Camera.main.transform.position + direction * dragSpeed;

                newPos.x = Mathf.Clamp(newPos.x, minBounds.x, maxBounds.x);
                newPos.y = Mathf.Clamp(newPos.y, minBounds.y, maxBounds.y);
                newPos.z = Camera.main.transform.position.z; 
                Camera.main.transform.position = newPos;
            }
        }
    }
}
