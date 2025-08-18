using UnityEngine;

namespace TopDownShooter
{
    public class CrosshairController_D : MonoBehaviour
    {
        private void Start()
        {
            // Hide the hardware cursor
            Cursor.visible = false;
        }

        private void Update()
        {
            // Get the current mouse position
            Vector3 mousePos = Input.mousePosition;

            // Clamp the mouse position to ensure it stays within the screen boundaries
            mousePos.x = Mathf.Clamp(mousePos.x, 0, Screen.width);
            mousePos.y = Mathf.Clamp(mousePos.y, 0, Screen.height);

            // Apply the Clamped position to the crosshair
            transform.position = mousePos;
        }
    }
}
