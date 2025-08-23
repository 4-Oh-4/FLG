using UnityEngine;

namespace TreeEditor
{
    public class LorryMovement : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 8f;
        [Tooltip("How far the lorry can move from the center.")]
        [SerializeField] private float moveRange = 8f;

        private Rigidbody2D rb;
        private float moveInput;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        void Update()
        {
            // Get input from A/D or Left/Right Arrow keys.
            moveInput = Input.GetAxis("Horizontal");
        }

        private void FixedUpdate()
        {
            // Apply horizontal velocity to the lorry's rigidbody.
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, 0f);

            // Clamp the lorry's position to keep it on the screen.
            Vector3 clampedPosition = transform.position;
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, -moveRange, moveRange);
            transform.position = clampedPosition;
        }
    }
}