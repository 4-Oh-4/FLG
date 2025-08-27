using UnityEngine;
using UnityEngine.UI; // Required for Image component

public class WindowQuestPointer_A : MonoBehaviour {
    [Header("UI References")]
    [Tooltip("Assign your UI arrow/circle Image component here. This is what displays the sprite.")]
    public Image pointerImage; // Changed from RectTransform to Image
    public Transform target;          // Quest target in world
    public Camera mainCamera;         // Usually Camera.main
    public Vector2 screenPadding = new Vector2(50, 50);

    [Header("Pointer Sprites")]
    [Tooltip("The sprite to use when the target is OFF-SCREEN (e.g., an arrow).")]
    public Sprite offscreenSprite;
    [Tooltip("The sprite to use when the target is ON-SCREEN (e.g., a circle or dot).")]
    public Sprite onscreenSprite;

    // We'll keep track of the current state to avoid unnecessary sprite changes
    private bool wasOffscreenLastFrame = false;

    void Start() {
        // Safety checks
        if (pointerImage == null) {
            Debug.LogError("Pointer Image (UI) is not assigned in WindowQuestPointer_A!", this);
            enabled = false; // Disable the script if critical component is missing
            return;
        }
        if (offscreenSprite == null || onscreenSprite == null) {
            Debug.LogWarning("One or both pointer sprites are not assigned. The pointer might not change visuals.", this);
        }
        if (mainCamera == null) {
            mainCamera = Camera.main; // Try to find the main camera if not assigned
            if (mainCamera == null) {
                Debug.LogError("Main Camera not assigned and Camera.main not found!", this);
                enabled = false;
                return;
            }
        }

        // Initially hide the pointer
        pointerImage.gameObject.SetActive(false);
    }

    void Update() {
        if (target == null) {
            // If there's no target, ensure the pointer is hidden.
            if (pointerImage.gameObject.activeSelf) {
                pointerImage.gameObject.SetActive(false);
            }
            return;
        }

        Vector3 screenPos = mainCamera.WorldToScreenPoint(target.position);

        // Check if the target is behind the camera (screenPos.z < 0)
        // or outside the padded screen area.
        bool isOffscreen = screenPos.z < 0 ||
                           screenPos.x < screenPadding.x ||
                           screenPos.x > Screen.width - screenPadding.x ||
                           screenPos.y < screenPadding.y ||
                           screenPos.y > Screen.height - screenPadding.y;

        if (isOffscreen) {
            // Only change sprite if it was on-screen last frame, or if it's the first time off-screen.
            if (!wasOffscreenLastFrame && offscreenSprite != null) {
                pointerImage.sprite = offscreenSprite;
            }
            pointerImage.gameObject.SetActive(true); // Always ensure it's active when off-screen.

            // Ensure z-coordinate is positive for correct clamping if target is behind camera
            Vector3 clampedScreenPos = screenPos;
            if (clampedScreenPos.z < 0) clampedScreenPos.z = 0;

            // Clamp position to screen edges
            clampedScreenPos.x = Mathf.Clamp(clampedScreenPos.x, screenPadding.x, Screen.width - screenPadding.x);
            clampedScreenPos.y = Mathf.Clamp(clampedScreenPos.y, screenPadding.y, Screen.height - screenPadding.y);

            pointerImage.rectTransform.position = clampedScreenPos;

            // Compute direction angle (only when off-screen)
            // The direction should be from the center of the screen towards the target,
            // or from the clamped position towards the target.
            Vector3 dir = (screenPos - pointerImage.rectTransform.position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            pointerImage.rectTransform.rotation = Quaternion.Euler(0, 0, angle - 90f); // Adjust by -90 if arrow points up by default

            wasOffscreenLastFrame = true;
        } else // Target is ON-SCREEN
          {
            // Only change sprite if it was off-screen last frame, or if it's the first time on-screen.
            if (wasOffscreenLastFrame && onscreenSprite != null) {
                pointerImage.sprite = onscreenSprite;
                // When on-screen, you might not want it to rotate, so reset rotation.
                pointerImage.rectTransform.rotation = Quaternion.identity;
            }
            pointerImage.gameObject.SetActive(true); // Always ensure it's active when on-screen

            // Position the pointer directly over the target when it's on-screen.
            pointerImage.rectTransform.position = screenPos;

            wasOffscreenLastFrame = false;
        }
    }
}