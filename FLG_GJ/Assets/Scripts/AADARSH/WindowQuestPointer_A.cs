using UnityEngine;
using UnityEngine.UI;

public class WindowQuestPointer_A : MonoBehaviour {
    public RectTransform arrowUI;     // Assign your UI arrow (RectTransform)
    public Transform target;          // Quest target in world
    public Camera mainCamera;         // Usually Camera.main
    public Vector2 screenPadding = new Vector2(50, 50);

    void Update() {
        if (target == null) return;
        Vector3 screenPos = mainCamera.WorldToScreenPoint(target.position);

        bool isOffscreen = screenPos.z < 0
            || screenPos.x < screenPadding.x
            || screenPos.x > Screen.width - screenPadding.x
            || screenPos.y < screenPadding.y
            || screenPos.y > Screen.height - screenPadding.y;

        if (isOffscreen) {
            arrowUI.gameObject.SetActive(true);

            // Clamp position to screen edges
            Vector3 clamped = screenPos;
            clamped.x = Mathf.Clamp(clamped.x, screenPadding.x, Screen.width - screenPadding.x);
            clamped.y = Mathf.Clamp(clamped.y, screenPadding.y, Screen.height - screenPadding.y);
            arrowUI.position = clamped;

            // Compute direction angle
            Vector3 dir = (screenPos - arrowUI.position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            arrowUI.rotation = Quaternion.Euler(0, 0, angle - 90f);
        } else {
            arrowUI.gameObject.SetActive(false);
        }
    }
}
