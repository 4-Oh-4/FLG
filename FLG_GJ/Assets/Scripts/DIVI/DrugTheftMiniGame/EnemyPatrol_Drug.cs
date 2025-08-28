using UnityEngine;

public class EnemyPatrol_Drug : MonoBehaviour
{
    [Header("Patrol Settings")]
    [Tooltip("Waypoints the enemy will patrol between. Drag empty GameObjects here in order.")]
    public Transform[] waypoints;
    public float speed = 2f;
    public float waypointTolerance = 0.2f;

    private int currentIndex = 0;
    private int direction = 1; // 1 = forward, -1 = backward

    void Update()
    {
        PatrolMovement();
    }

    private void PatrolMovement()
    {
        if (waypoints.Length == 0) return;

        Transform target = waypoints[currentIndex];
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        // Rotate enemy to face movement direction
        Vector2 dir = target.position - transform.position;
        if (dir != Vector2.zero)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90f); // adjust offset for your sprite
        }

        // Check if close to waypoint
        if (Vector2.Distance(transform.position, target.position) < waypointTolerance)
        {
            currentIndex += direction;

            // Reverse direction at ends
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

    // Draw only patrol path
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
