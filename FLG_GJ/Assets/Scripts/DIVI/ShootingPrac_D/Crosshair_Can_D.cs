using TruckChase;
using UnityEngine;

public class Crosshair_Can_D : MonoBehaviour
{
    [SerializeField] private float hitRadius = 0.35f;
    [SerializeField] private LayerMask canLayer; // set to "Can" in Inspector
    [SerializeField] private KeyCode shootKey = KeyCode.Mouse0;


    private void Start()
    {
        Cursor.visible = false;
    }
    void Update()
    {


        // Get the current mouse position on the screen.
        Vector3 mousePos = Input.mousePosition;

        // Clamp the position to ensure the crosshair cannot go outside the game window.
        mousePos.x = Mathf.Clamp(mousePos.x, 0, Screen.width);
        mousePos.y = Mathf.Clamp(mousePos.y, 0, Screen.height);

        // Apply the clamped position to this UI element's transform.
        transform.position = mousePos;

        if (Input.GetKeyDown(shootKey) || Input.GetKeyDown(KeyCode.Space))
        {
            var hits = Physics2D.OverlapCircleAll(transform.position, hitRadius, canLayer);
            if (hits.Length > 0)
            {
                // Prefer the closest can if several overlap
                Collider2D best = hits[0];
                float bestDist = Mathf.Infinity;
                foreach (var h in hits)
                {
                    float d = (h.transform.position - transform.position).sqrMagnitude;
                    if (d < bestDist) { bestDist = d; best = h; }
                }

                var can = best.GetComponent<Can_D>();
                if (can != null)
                {
                    can.Hit();
                    GameManager_SP.Instance.OnCanShot(); // optional score hook
                }
            }
            else
            {
                // Miss – optional feedback
            }
        }
    }

    // Optional: draw aim radius in Scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, hitRadius);
    }
}
