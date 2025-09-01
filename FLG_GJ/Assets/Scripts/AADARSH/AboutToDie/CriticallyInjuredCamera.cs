using UnityEngine;

public class CriticallyInjuredCamera : MonoBehaviour {
    [Header("Camera Follow")]
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private Vector3 offset; // You can still use offset for X and Y adjustments

    [Header("Injury Effect Settings")]
    [Tooltip("The total time in seconds from injury until the screen is fully black.")]
    [SerializeField] private float injuryDuration = 20f;
    [Tooltip("The maximum shake intensity at the end of the countdown.")]
    [SerializeField] private float maxShakeIntensity = 0.5f;
    [Tooltip("The speed of the camera shake.")]
    [SerializeField] private float shakeSpeed = 5f;
    [Tooltip("The maximum Z-axis tilt in degrees at the end of the countdown.")]
    [SerializeField] private float maxTiltAmount = 5f;

    [Header("Required Components")]
    [SerializeField] private CanvasGroup fadeOverlay; // Drag your UI Image with CanvasGroup here

    private bool isInjured = false;
    private float injuryTimer = 0f;
    private float injuryProgress = 0f; // A value from 0 to 1 representing injury severity

    private Vector3 followVelocity = Vector3.zero;
    private float perlinSeedX, perlinSeedY, perlinSeedZ;

    void Start() {
        // Basic checks to ensure everything is set up
        if (target == null || fadeOverlay == null) {
            Debug.LogError("CriticallyInjuredCamera: Target or Fade Overlay is not set in the Inspector!");
            this.enabled = false;
            return;
        }

        // Set random seeds for varied and smooth Perlin noise shake/tilt
        perlinSeedX = Random.Range(0f, 100f);
        perlinSeedY = Random.Range(0f, 100f);
        perlinSeedZ = Random.Range(0f, 100f); // For tilting
        StartCriticalInjury();
    }

    void LateUpdate() {
        // --- Step 1: ALWAYS perform the smooth follow (Corrected for 2D) ---
        // We construct a new desired position using the target's X/Y and the camera's OWN Z.
        Vector3 desiredPosition = new Vector3(
            target.position.x + offset.x,
            target.position.y + offset.y,
            transform.position.z // This locks the Z-axis
        );

        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref followVelocity, smoothSpeed);

        // --- Step 2: If injured, apply additional effects ON TOP of the follow position ---
        if (isInjured) {
            HandleInjuryEffects();
        } else {
            // If not injured, ensure the camera is perfectly upright.
            transform.rotation = Quaternion.identity;
        }
    }

    private void HandleInjuryEffects() {
        // 1. Update timer and progress
        injuryTimer += Time.deltaTime;
        injuryProgress = Mathf.Clamp01(injuryTimer / injuryDuration);

        // 2. Calculate current effect intensities based on injuryProgress
        float currentShake = maxShakeIntensity * injuryProgress;
        float currentTilt = maxTiltAmount * injuryProgress;

        // 3. Apply effects
        // Shake: We add the shake offset to the already calculated follow position.
        // The Z component of shakeOffset is already 0, so this is safe for 2D.
        float xNoise = (Mathf.PerlinNoise(perlinSeedX, Time.time * shakeSpeed) * 2) - 1;
        float yNoise = (Mathf.PerlinNoise(perlinSeedY, Time.time * shakeSpeed) * 2) - 1;
        Vector3 shakeOffset = new Vector3(xNoise, yNoise, 0) * currentShake;
        transform.position += shakeOffset; // Use += to add shake to the base position

        // Tilt
        float tiltNoise = (Mathf.PerlinNoise(perlinSeedZ, Time.time * shakeSpeed * 0.5f) * 2) - 1;
        float tiltAngle = tiltNoise * currentTilt;
        transform.rotation = Quaternion.Euler(0, 0, tiltAngle);

        // Fade
        fadeOverlay.alpha = injuryProgress;

        // When the timer is done, you might want to trigger a game over state.
        if (injuryProgress >= 1.0f) {
            // Example: Disable player movement completely
            var playerMovement = target.GetComponent<MonoBehaviour>();
            if (playerMovement != null && playerMovement.GetType().Name.Contains("PlayerMovement")) {
                playerMovement.enabled = false;
            }
        }
    }

    public void StartCriticalInjury() {
        if (!isInjured) {
            isInjured = true;
            injuryTimer = 0f; // Reset timer
        }
    }
}