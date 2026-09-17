using UnityEngine;

/// <summary>
/// A simple minimap camera that follows the player from above and renders to a RenderTexture.
/// The RenderTexture is displayed in a RawImage on the HUD canvas.
///
/// SETUP INSTRUCTIONS (Unity Editor):
/// 1. Create a new Camera as a child of the Player (or as a standalone object).
/// 2. Set this camera to Orthographic, rotation (0,0,0), higher up on Z (e.g., z = -10 for 2D).
/// 3. Create a RenderTexture asset (e.g., 256x256 or 512x512).
/// 4. Assign that RenderTexture to this camera's "Target Texture" field.
/// 5. In your HUD Canvas, create a RawImage and assign the same RenderTexture to it.
/// 6. Optionally, create a "Minimap" layer and set this camera to only render that layer
///    plus whatever layers you want visible on the minimap.
/// 7. Add this script to the minimap camera GameObject.
/// </summary>
public class MinimapCamera : MonoBehaviour {
    [Header("Follow Target")]
    [Tooltip("The player's Transform. The minimap camera will follow this.")]
    [SerializeField] private Transform target;

    [Header("Camera Settings")]
    [Tooltip("How zoomed out the minimap is. Higher = shows more area.")]
    [SerializeField] private float zoomLevel = 20f;

    [Tooltip("Minimum zoom level (most zoomed in).")]
    [SerializeField] private float minZoom = 10f;

    [Tooltip("Maximum zoom level (most zoomed out).")]
    [SerializeField] private float maxZoom = 40f;

    [Header("Player Icon")]
    [Tooltip("Optional: A small sprite that sits at the center of the minimap to represent the player.")]
    [SerializeField] private GameObject playerIcon;

    private Camera minimapCam;

    private void Awake() {
        minimapCam = GetComponent<Camera>();
        if (minimapCam == null) {
            Debug.LogError("[MinimapCamera] No Camera component found on this GameObject!", this);
            enabled = false;
            return;
        }

        // Ensure it's orthographic for a clean 2D top-down view.
        minimapCam.orthographic = true;
        minimapCam.orthographicSize = zoomLevel;
    }

    private void LateUpdate() {
        if (target == null) return;

        // Follow the player's XY position, keep the camera's Z.
        Vector3 newPos = new Vector3(target.position.x, target.position.y, transform.position.z);
        transform.position = newPos;
    }

    /// <summary>
    /// Change the minimap zoom level at runtime.
    /// </summary>
    /// <param name="newZoom">The new orthographic size.</param>
    public void SetZoom(float newZoom) {
        zoomLevel = Mathf.Clamp(newZoom, minZoom, maxZoom);
        if (minimapCam != null) {
            minimapCam.orthographicSize = zoomLevel;
        }
    }

    /// <summary>
    /// Zoom in by a step amount.
    /// </summary>
    public void ZoomIn(float step = 2f) {
        SetZoom(zoomLevel - step);
    }

    /// <summary>
    /// Zoom out by a step amount.
    /// </summary>
    public void ZoomOut(float step = 2f) {
        SetZoom(zoomLevel + step);
    }
}
