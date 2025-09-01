using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement_A : MonoBehaviour
{
    [SerializeField] float movementSpeed = 5f;

    private Vector2 movementInput;
    private Rigidbody2D rb;
    private Animator animator;
    public bool canMove = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (animator == null) return;

        float speed = movementInput.magnitude;
        animator.SetFloat("Speed", speed);

        if (speed > 0.1f)
        {
            animator.SetFloat("InputX", movementInput.x);
            animator.SetFloat("InputY", movementInput.y);
        }
    }

    // This function is called by the Player Input component.
    public void OnMovement(InputAction.CallbackContext callbackContext)
    {
        movementInput = callbackContext.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        if (canMove)
            rb.linearVelocity = movementInput * movementSpeed;
        else
            rb.linearVelocity = Vector2.zero;
    }
}