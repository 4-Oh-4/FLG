using UnityEngine;

public class CameraFollow_Drug : MonoBehaviour
{
    public Transform target; // The player
    public float smoothSpeed = 0.125f;
    public Vector3 offset;   // Distance between camera and player

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // Keep camera facing forward
        transform.rotation = Quaternion.identity;
    }
}
