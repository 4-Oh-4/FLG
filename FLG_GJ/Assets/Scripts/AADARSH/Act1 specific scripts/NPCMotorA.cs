// NpcMotorA.cs
using UnityEngine;

[RequireComponent(typeof(Animator))] // ADDED: Ensures an Animator is always present
public class NpcMotorA : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;

    // ADDED: A variable to hold our Animator component
    private Animator animator;

    private Vector3 targetPosition;
    private bool isMoving = false;

    // ADDED: An Awake function to get the Animator component automatically
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!isMoving)
        {
            // The animator's "IsMoving" is already false here, so no extra code is needed.
            return;
        }

        // --- ADDED: Animation Direction Logic ---
        // Calculate the direction vector from our current position to the target
        Vector2 direction = (targetPosition - transform.position).normalized;

        // Update the Animator's directional parameters
        animator.SetFloat("MoveX", direction.x);
        animator.SetFloat("MoveY", direction.y);
        // ------------------------------------

        // Move towards the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Check if we have arrived at the destination
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            isMoving = false;
            transform.position = targetPosition; // Snap to the final position

            // CHANGED: Tell the Animator we have stopped moving
            animator.SetBool("IsMoving", false);
        }
    }

    /// <summary>
    /// Public method to command this NPC to move to a new position.
    /// </summary>
    public void MoveTo(Vector3 newPosition)
    {
        targetPosition = newPosition;
        isMoving = true;

        // CHANGED: Tell the Animator we have started moving
        animator.SetBool("IsMoving", true);
    }

    /// <summary>
    /// Public method to command this NPC to stop moving.
    /// </summary>
    public void Stop()
    {
        isMoving = false;

        // CHANGED: Tell the Animator we have stopped moving
        animator.SetBool("IsMoving", false);
    }
}