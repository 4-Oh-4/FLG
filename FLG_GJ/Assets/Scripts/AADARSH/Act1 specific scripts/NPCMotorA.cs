// NpcMotor.cs
using UnityEngine;

public class NpcMotorA : MonoBehaviour {
    [SerializeField] private float moveSpeed = 2f;

    // An Animator is optional but recommended for walking animations
    // [SerializeField] private Animator animator; 

    private Vector3 targetPosition;
    private bool isMoving = false;

    void Update() {
        if (!isMoving) {
            // if (animator != null) animator.SetBool("IsWalking", false);
            return;
        }

        // Move towards the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Check if we have arrived at the destination
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f) {
            isMoving = false;
            transform.position = targetPosition; // Snap to the final position
        }
    }

    /// <summary>
    /// Public method to command this NPC to move to a new position.
    /// </summary>
    public void MoveTo(Vector3 newPosition) {
        targetPosition = newPosition;
        isMoving = true;
        // if (animator != null) animator.SetBool("IsWalking", true);
    }

    /// <summary>
    /// Public method to command this NPC to stop moving.
    /// </summary>
    public void Stop() {
        isMoving = false;
        // if (animator != null) animator.SetBool("IsWalking", false);
    }
}