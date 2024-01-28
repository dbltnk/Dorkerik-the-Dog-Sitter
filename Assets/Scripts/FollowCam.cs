using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform target;
    public float smoothing = 5.0f; // The higher the value, the faster the camera will follow the target
    public Vector3 offset = new Vector3(0, 3, -6); // Offset from the target
    public float fixedY = 3.0f; // Fixed Y position for the camera

    void FixedUpdate()
    {
        if (target)
        {
            Vector3 targetCamPos = new Vector3(target.position.x, fixedY, target.position.z) + offset;
            transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
            transform.LookAt(target);
        }
    }
}