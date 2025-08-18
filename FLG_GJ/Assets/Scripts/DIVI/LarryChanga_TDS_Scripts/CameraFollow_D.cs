using UnityEngine;

namespace TopDownShooter
{
    public class CameraFollow_D : MonoBehaviour
    {
        [Tooltip("The object the camera should follow (the Player).")]
        public Transform target;
        [Tooltip("How quickly the camera catches up to its target. Smaller is slower.")]
        public float smoothSpeed = 0.125f;
        [Tooltip("An offset from the target's position. Keep Z at -10 for a 2D camera.")]
        public Vector3 offset;

        // LateUpdate is used to ensure the camera moves *after* the player has moved in a frame.
        private void LateUpdate()
        {
            if (target != null)
            {
                // Calculate the desired position for the camera.
                Vector3 desiredPosition = target.position + offset;
                // Smoothly interpolate from the camera's current position to the desired position.
                Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
                // Apply the new position.
                transform.position = smoothedPosition;
            }
        }
    }
}