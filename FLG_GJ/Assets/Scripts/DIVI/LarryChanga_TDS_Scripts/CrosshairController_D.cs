using UnityEngine;

namespace TopDownShooter
{
    public class CrosshairController_D : MonoBehaviour
    {
        private void Start()
        {
            // Hide the default hardware mouse cursor so we only see our custom crosshair.
            Cursor.visible = false;
        }

        private void Update()
        {
            // Get the current mouse position on the screen.
            Vector3 mousePos = Input.mousePosition;

            // Clamp the position to ensure the crosshair cannot go outside the game window.
            mousePos.x = Mathf.Clamp(mousePos.x, 0, Screen.width);
            mousePos.y = Mathf.Clamp(mousePos.y, 0, Screen.height);

            // Apply the clamped position to this UI element's transform.
            transform.position = mousePos;
        }
    }
}