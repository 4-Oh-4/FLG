using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement_A : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] float movementSpeed = 1f;
    Vector2 movementInput;
    private Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void movement(InputAction.CallbackContext callbackContext) {
        //Debug.Log(callbackContext);
        movementInput = callbackContext.ReadValue<Vector2>();

    }
    void FixedUpdate() {
        // Apply movement
        rb.linearVelocity = movementInput * movementSpeed;
    }
}
