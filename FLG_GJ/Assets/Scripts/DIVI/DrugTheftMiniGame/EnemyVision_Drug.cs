using UnityEngine;

[RequireComponent(typeof(EnemyPatrol_Drug))]
public class EnemyVision_Drug : MonoBehaviour
{
    [Header("Vision Settings")]
    public float viewRadius = 6f;
    [Range(1f, 360f)] public float viewAngle = 70f;
    public LayerMask obstacleMask;
    public LayerMask targetMask;

    [Header("Vision Cone Reference")]
    public ProceduralVisionCone visionCone;
    public Color idleColor = new Color(1f, 1f, 0f, 0.25f);
    public Color detectedColor = new Color(1f, 0f, 0f, 0.35f);

    // --- Private State ---
    private Transform player;
    private bool playerDetected = false;
    private EnemyPatrol_Drug patrolScript;
    private Material visionConeMaterial;

    void Start()
    {
        patrolScript = GetComponent<EnemyPatrol_Drug>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Find the vision cone and get its material for color changes
        if (visionCone == null)
        {
            visionCone = GetComponentInChildren<ProceduralVisionCone>();
        }
        if (visionCone != null)
        {
            visionConeMaterial = visionCone.GetComponent<MeshRenderer>().material;
            visionConeMaterial.color = idleColor;
        }

        if (player == null)
            Debug.LogError("[EnemyVision] No Player found with tag 'Player'.");
    }

    void Update()
    {
        bool sees = SeesPlayer();

        // Change the material color based on detection status
        if (visionConeMaterial != null)
        {
            visionConeMaterial.color = sees ? detectedColor : idleColor;
        }

        // Rotate the vision cone to match the enemy's facing direction
        if (visionCone != null && patrolScript != null)
        {
            Vector2 forward = patrolScript.FacingDirection;
            float angle = Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg;
            // The procedural mesh points UP, so we need a -90 degree offset to align it with the logic
            visionCone.transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
        }

        // Handle detection logic
        if (sees && !playerDetected)
        {
            playerDetected = true;
            Debug.Log("[EnemyVision] Player detected. Triggering Game Over.");
        }
        else if (!sees && playerDetected)
        {
            GameManager_Drug.Instance?.OnPlayerCaught();

            Debug.Log("[EnemyVision] Lost sight of player.");
            playerDetected = false;
        }
    }

    bool SeesPlayer()
    {
        if (player == null) return false;

        Vector2 forward = patrolScript.FacingDirection;
        Vector2 dirToTarget = (player.position - transform.position).normalized;
        float dist = Vector2.Distance(transform.position, player.position);

        if (dist > viewRadius) return false;

        float angleToTarget = Vector2.Angle(forward, dirToTarget);
        if (angleToTarget > viewAngle * 0.5f) return false;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, dirToTarget, dist, obstacleMask);
        if (hit.collider != null) return false;

        return true;
    }

    // --- The Gizmo and DirFromAngle functions are for editor visualization ---
    // They are not needed for the in-game cone but are useful for debugging
    void OnDrawGizmosSelected()
    {
        // ... (Your Gizmo code can remain here for debugging in the Scene view) ...
    }

    Vector2 DirFromAngle(float angleDegrees, Vector2 forward)
    {
        // ... (Your Gizmo helper function) ...
        float baseAngle = Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg;
        float angle = (baseAngle + angleDegrees) * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }
}