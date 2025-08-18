using UnityEngine;

// AMENDED: Added the namespace to match the other scripts.
namespace TopDownShooter
{
    public class PlayerController_D : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 5f;

        [Header("Combat")]
        [SerializeField] private int maxAmmo = 30;
        private int currentAmmo;
        [SerializeField] private Transform attackPoint;

        private Rigidbody2D playerRb;
        private Vector2 moveInput;
        private Vector2 mousePos;
        private Camera mainCam;

        private void Start()
        {
            playerRb = GetComponent<Rigidbody2D>();
            mainCam = Camera.main;
            currentAmmo = maxAmmo;
        }

        private void Update()
        {
            moveInput.x = Input.GetAxisRaw("Horizontal");
            moveInput.y = Input.GetAxisRaw("Vertical");
            mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

            if (Input.GetMouseButtonDown(0))
            {
                Shoot();
            }
        }

        private void FixedUpdate()
        {
            playerRb.MovePosition(playerRb.position + moveInput.normalized * moveSpeed * Time.fixedDeltaTime);
            Vector2 lookDir = mousePos - playerRb.position;
            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
            playerRb.rotation = angle;
        }

        void Shoot()
        {
            if (currentAmmo > 0)
            {
                currentAmmo--;
                Debug.Log("Ammo: " + currentAmmo);

                RaycastHit2D hitInfo = Physics2D.Raycast(attackPoint.position, attackPoint.up);

                if (hitInfo.collider != null)
                {
                    Debug.Log("Hit: " + hitInfo.collider.name);

                    Health_D targetHealth = hitInfo.collider.GetComponent<Health_D>();
                    if (targetHealth != null)
                    {
                        targetHealth.TakeDamage(25f);
                    }
                }
            }
            else
            {
                Debug.Log("Out of ammo !");
            }
        }
    }
}