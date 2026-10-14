using UnityEngine;

public class Camerafollow_A : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Transform playerTransform;
    //[SerializeField]float smoothSpeed = 0;

    [Tooltip("Seconds the camera takes to catch up with the player. 0 = locked to the player (the old behaviour).")]
    [SerializeField] float smoothTime = 0.12f;
    [Tooltip("Jumps bigger than this (doors, stairs, teleports, loading a save) snap instead of gliding.")]
    [SerializeField] float snapDistance = 3f;

    private Vector3 velocity;
    private bool placed;
    private CriticallyInjuredCamera injuredCamera;

    void Start()
    {
        // AboutToDieScene drives the same camera with its own shaky follow; leave that scene as it was.
        injuredCamera = GetComponent<CriticallyInjuredCamera>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void LateUpdate() {
        Vector3 targetPos = new Vector3(playerTransform.position.x,playerTransform.position.y,transform.position.z);

        //Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPos, smoothSpeed);
        bool smooth = smoothTime > 0f && placed && (injuredCamera == null || !injuredCamera.isActiveAndEnabled)
            && ((Vector2)(targetPos - transform.position)).sqrMagnitude <= snapDistance * snapDistance;
        if (smooth) {
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
        } else {
            transform.position = targetPos;
            velocity = Vector3.zero;
            placed = true;
        }
    }
}
