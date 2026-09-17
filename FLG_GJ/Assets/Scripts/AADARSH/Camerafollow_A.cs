using UnityEngine;

public class Camerafollow_A : MonoBehaviour
{
    public Transform playerTransform;
    [SerializeField] float smoothSpeed = 8f;

    private void LateUpdate() {
        if (playerTransform == null) return;

        Vector3 targetPos = new Vector3(playerTransform.position.x, playerTransform.position.y, transform.position.z);
        
        // Smooth follow using Lerp — feels polished instead of snapping
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
}
