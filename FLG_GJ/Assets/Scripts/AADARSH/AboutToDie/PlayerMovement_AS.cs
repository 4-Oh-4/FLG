using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement_AS : MonoBehaviour {
    [SerializeField] float movementSpeed = 5f;

    private Vector2 movementInput;
    private Rigidbody2D rb;
    private Animator animator;
    public bool canMove = true;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update() {
        // --- Input Handling (Legacy) ---
        // 1. Get raw axis values from the Input Manager.
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // 2. We start with a zero vector and build our desired movement.
        Vector2 processedInput = Vector2.zero;

        // 3. Apply the logic: up or right, one at a time.
        // We prioritize vertical (up) movement.
        if (verticalInput > 0) {
            // If the player presses "up", ignore all horizontal input.
            processedInput.y = verticalInput;
        }
        // This only runs if the player is NOT pressing "up".
        else if (horizontalInput > 0) {
            // If the player presses "right", set the horizontal movement.
            processedInput.x = horizontalInput;
        }

        // 4. Set the final calculated value to our class variable.
        movementInput = processedInput;


        // --- Animation Handling (No changes needed here) ---
        if (animator == null) return;

        float speed = movementInput.magnitude;
        animator.SetFloat("Speed", speed);

        if (speed > 0.1f) {
            animator.SetFloat("InputX", movementInput.x);
            animator.SetFloat("InputY", movementInput.y);
        }
    }

    // This method is from the new Input System and is no longer needed.
    // public void OnMovement(InputAction.CallbackContext callbackContext) { ... }

    void FixedUpdate() {
        // Physics logic doesn't need to change. It correctly uses the
        // movementInput vector calculated in Update().
        if (canMove)
            rb.linearVelocity = movementInput * movementSpeed;
        else
            rb.linearVelocity = Vector2.zero;
    }
}