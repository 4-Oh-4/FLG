using UnityEngine;

[RequireComponent(typeof(EnemyPatrol_Drug))]
public class EnemyVision_Drug : MonoBehaviour
{
    [Header("Vision Settings")]
    public float viewRadius = 6f;
    [Range(1f, 360f)] public float viewAngle = 70f;
    public LayerMask obstacleMask;  // <-- Only real obstacles here (e.g. "Objects" or "Walls")
    public LayerMask targetMask;

    [Header("Vision Sprite (Child)")]
    public SpriteRenderer visionConeSprite;
    public Color idleColor = new Color(1f, 1f, 0f, 0.25f);
    public Color detectedColor = new Color(1f, 0f, 0f, 0.35f);

    private Transform player;
    private bool playerDetected = false;

    void Start()
    {
        if (visionConeSprite == null)
        {
            var child = transform.Find("VisionCone");
            if (child) visionConeSprite = child.GetComponent<SpriteRenderer>();
        }

        if (visionConeSprite) visionConeSprite.color = idleColor;

        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
            Debug.LogError("[EnemyVision] No Player found with tag 'Player'.");
    }

    void Update()
    {
        bool sees = SeesPlayer();

        if (visionConeSprite)
            visionConeSprite.color = sees ? detectedColor : idleColor;

        if (sees && !playerDetected)
        {
            playerDetected = true;
            Debug.Log("[EnemyVision] Player detected. Triggering Game Over.");
            GameManager_Drug.Instance?.OnPlayerCaught();
        }
        else if (!sees && playerDetected)
        {
            Debug.Log("[EnemyVision] Lost sight of player.");
            playerDetected = false;
        }
    }

    bool SeesPlayer()
    {
        if (player == null) return false;

        Vector2 forward = transform.up; // Vision forward = local Y
        Vector2 dirToTarget = (player.position - transform.position).normalized;
        float dist = Vector2.Distance(transform.position, player.position);

        // Step 1: Radius check
        if (dist > viewRadius) return false;

        // Step 2: Angle check
        float angleToTarget = Vector2.Angle(forward, dirToTarget);
        if (angleToTarget > viewAngle * 0.5f) return false;

        // Step 3: Raycast check (only against obstacleMask)
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dirToTarget, dist, obstacleMask);

        if (hit.collider != null)
        {
            // Something on obstacleMask is blocking
            Debug.Log("[EnemyVision] Vision blocked by: " + hit.collider.name);
            return false;
        }

        // No obstacle in the way  player is visible
        Debug.DrawLine(transform.position, player.position, Color.green);
        return true;
    }

    void OnDrawGizmosSelected()
    {
        Vector2 forward = transform.up; // Vision forward = local Y

        int segments = 30;
        float halfAngle = viewAngle * 0.5f;
        Vector3 prevPoint = transform.position + (Vector3)DirFromAngle(-halfAngle, forward) * viewRadius;

        for (int i = 1; i <= segments; i++)
        {
            float step = Mathf.Lerp(-halfAngle, halfAngle, i / (float)segments);
            Vector3 nextPoint = transform.position + (Vector3)DirFromAngle(step, forward) * viewRadius;

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, nextPoint);

            Gizmos.color = new Color(1, 0.5f, 0, 0.3f);
            Gizmos.DrawLine(prevPoint, nextPoint);

            prevPoint = nextPoint;
        }

        // Cyan line = vision forward
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)forward * viewRadius);
    }

    Vector2 DirFromAngle(float angleDegrees, Vector2 forward)
    {
        float baseAngle = Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg;
        float angle = (baseAngle + angleDegrees) * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }
}
