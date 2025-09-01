using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement_A : MonoBehaviour
{
    [SerializeField] float movementSpeed = 5f;

    private Vector2 movementInput;
    private Rigidbody2D rb;
    private Animator animator; // <-- ADDED: Reference for the Animator
    public bool canMove = true;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); // <-- ADDED: Get the Animator component
    }

    void Update()
    {
        if (animator == null) return;
        // --- ADDED: Animation Control Logic ---

        // 1. Calculate the movement speed
        float speed = movementInput.magnitude;
        animator.SetFloat("Speed", speed);

        // 2. If we are moving, update the direction parameters
        if (speed > 0.1f)
        {
            // We send the raw input direction to the blend tree.
            // The blend tree will automatically find the closest animation (Up, Down, Left, or Right).
            animator.SetFloat("InputX", movementInput.x);
            animator.SetFloat("InputY", movementInput.y);
        }
    }

    // I renamed this function to OnMovement for clarity with the Input System
    public void OnMovement(InputAction.CallbackContext callbackContext)
    {
        movementInput = callbackContext.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        // Apply physics movement
        if (canMove)
            rb.linearVelocity = movementInput * movementSpeed;
        else rb.linearVelocity = Vector2.zero;
    }
}