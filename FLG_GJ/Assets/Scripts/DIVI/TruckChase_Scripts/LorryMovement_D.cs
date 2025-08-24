using UnityEngine;

namespace TruckChase
{
    public class LorryMovement_D : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 8f;
        [SerializeField] private float moveRange = 8f;

        private Rigidbody2D rb;
        private float moveInput;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        void Update()
        {
            moveInput = Input.GetAxis("Horizontal");
        }

        private void FixedUpdate()
        {
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, 0f);

            Vector3 clampedPosition = transform.position;
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, -moveRange, moveRange);
            transform.position = clampedPosition;
        }
    }
}