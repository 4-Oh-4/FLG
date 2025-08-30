using UnityEngine;

public class Crosshair_Can_D : MonoBehaviour
{
    [Header("Crosshair Settings")]
    [SerializeField] private float hitRadius = 0.35f;
    [SerializeField] private LayerMask canLayer;   // set this to "Can" in Inspector
    [SerializeField] private KeyCode shootKey = KeyCode.Mouse0;

    private Camera mainCam;

    private void Start()
    {
        Cursor.visible = false;
        mainCam = Camera.main;
    }

    private void Update()
    {
        // Move crosshair in UI space (Canvas overlay mode)
        Vector3 mousePos = Input.mousePosition;
        mousePos.x = Mathf.Clamp(mousePos.x, 0, Screen.width);
        mousePos.y = Mathf.Clamp(mousePos.y, 0, Screen.height);
        transform.position = mousePos;

        if (Input.GetKeyDown(shootKey) || Input.GetKeyDown(KeyCode.Space))
        {
            // Convert crosshair (UI position) to world space for physics check
            Vector3 worldPos = mainCam.ScreenToWorldPoint(mousePos);
            worldPos.z = 0f;

            Collider2D[] hits = Physics2D.OverlapCircleAll(worldPos, hitRadius, canLayer);
            if (hits.Length > 0)
            {
                // Pick closest can
                Collider2D best = hits[0];
                float bestDist = Mathf.Infinity;
                foreach (var h in hits)
                {
                    float d = (h.transform.position - worldPos).sqrMagnitude;
                    if (d < bestDist) { bestDist = d; best = h; }
                }

                var can = best.GetComponent<Can_D>();
                if (can != null)
                {
                    can.Hit(); // Can handles destroying itself
                    GameManager_SP.Instance.OnCanShot(); // score hook
                }
            }
            else
            {
                // Optional: feedback for miss
            }
        }
    }

    // Debug: draw aim radius
    private void OnDrawGizmosSelected()
    {
        if (!mainCam) mainCam = Camera.main;
        Vector3 worldPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0f;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(worldPos, hitRadius);
    }
}
