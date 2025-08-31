// NPCController.cs

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))] // Ensures an Animator is always present
public class NPCController : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("The speed at which the NPC moves between waypoints.")]
    [SerializeField] private float moveSpeed = 2f;

    [Header("Stationary Settings")]
    [Tooltip("The initial direction the NPC should face if they have no waypoints. (0, -1) is down, (0, 1) is up, etc.")]
    [SerializeField] private Vector2 initialFacingDirection = new Vector2(0, -1);

    [Header("Patrol Route")]
    [Tooltip("The list of waypoints for the NPC to patrol. Leave empty for a stationary NPC.")]
    public WayPoints_D[] waypoints;

    // --- Private State Variables ---
    private int currentWaypointIndex = 0;
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();

        if (waypoints != null && waypoints.Length > 0)
        {
            // NPC has a patrol route, start the coroutine.
            StartCoroutine(PatrolRoutine());
        }
        else
        {
            // NPC is stationary, set its initial idle direction.
            UpdateAnimatorDirection(initialFacingDirection);
        }
    }

    private IEnumerator PatrolRoutine()
    {
        while (true)
        {
            WayPoints_D currentWaypoint = waypoints[currentWaypointIndex];
            Transform targetTransform = currentWaypoint.waypointTransform;

            // --- Start Moving ---
            anim.SetBool("IsMoving", true);

            while (Vector3.Distance(transform.position, targetTransform.position) > 0.1f)
            {
                // Calculate direction and update the animator
                Vector2 direction = (targetTransform.position - transform.position).normalized;
                UpdateAnimatorDirection(direction);

                // Move the NPC
                transform.position = Vector3.MoveTowards(transform.position, targetTransform.position, moveSpeed * Time.deltaTime);
                yield return null;
            }

            // --- Arrived at Waypoint, Stop and Wait ---
            anim.SetBool("IsMoving", false);
            yield return new WaitForSeconds(currentWaypoint.waitTime);

            // Move to the next waypoint
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    // A helper function to update the animator's direction parameters
    private void UpdateAnimatorDirection(Vector2 direction)
    {
        anim.SetFloat("MoveX", direction.x);
        anim.SetFloat("MoveY", direction.y);
    }
}