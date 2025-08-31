using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EnemyPatrol_Drug : MonoBehaviour
{
    [Header("Patrol Settings")]
    [Tooltip("Waypoints the enemy will patrol between. Drag empty GameObjects here in order.")]
    public Transform[] waypoints;
    public float speed = 2f;
    public float waypointTolerance = 0.2f;

    // Public property to share the current facing direction with other scripts.
    public Vector2 FacingDirection { get; private set; }

    private Animator anim;
    private int currentIndex = 0;
    private int direction = 1;

    private void Start()
    {
        anim = GetComponent<Animator>();
        // Initialize facing direction to down by default.
        FacingDirection = Vector2.down;

        if (waypoints.Length > 0)
        {
            anim.SetBool("IsMoving", true);
        }
        else
        {
            anim.SetBool("IsMoving", false);
            // If not moving, set the initial idle direction for the animator.
            anim.SetFloat("MoveX", FacingDirection.x);
            anim.SetFloat("MoveY", FacingDirection.y);
        }
    }

    void Update()
    {
        PatrolMovement();
    }

    private void PatrolMovement()
    {
        if (waypoints.Length == 0) return;

        Transform target = waypoints[currentIndex];

        Vector2 moveDirection = (target.position - transform.position).normalized;

        // Update the public FacingDirection property if the enemy is moving.
        if (moveDirection != Vector2.zero)
        {
            FacingDirection = moveDirection;
        }

        // Update the Animator's directional parameters.
        anim.SetFloat("MoveX", FacingDirection.x);
        anim.SetFloat("MoveY", FacingDirection.y);

        // Move the character.
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        // Check if close enough to the waypoint to switch to the next one.
        if (Vector2.Distance(transform.position, target.position) < waypointTolerance)
        {
            currentIndex += direction;

            // Reverse direction at the ends of the patrol path.
            if (currentIndex >= waypoints.Length)
            {
                currentIndex = waypoints.Length - 2;
                direction = -1;
            }
            else if (currentIndex < 0)
            {
                currentIndex = 1;
                direction = 1;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if (waypoints != null && waypoints.Length > 1)
        {
            for (int i = 0; i < waypoints.Length - 1; i++)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            }
        }
    }
}