using UnityEngine;

namespace TopDownShooter
{
    public class CameraFollow_D : MonoBehaviour
    {
        // The Object the camera will follow (our Player)
        public Transform target;

        // How smoothly the camera cathes up to its target
        public float smoothSpeed = 0.125f;

        // An offset from the target's position
        public Vector3 offset;

        private void LateUpdate()
        {
            // Ensure we have a target to follow
            if (target != null)
            {
                // The desired position for the camera
                Vector3 desiredPosition = target.position + offset;

                // Linearly interpolate from the current position to the desired position
                Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

                // Apply the new position to the camera
                transform.position = smoothedPosition;
            }
        }
    }
}
