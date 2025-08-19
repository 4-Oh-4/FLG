using UnityEngine;

namespace TopDownShooter
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController_D : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 5f;

        [Header("Combat")]
        [SerializeField] private int maxAmmo = 60;
        [SerializeField] private float attackDamage = 25f;
        [SerializeField] private LayerMask enemyLayers;
        [SerializeField] private GameObject shotEffectPrefab;

        // --- Private References & State ---
        private Rigidbody2D playerRb;
        private Camera mainCam;
        private Vector2 moveInput;
        private Vector2 mousePos;
        private int currentAmmo;

        private void Start()
        {
            playerRb = GetComponent<Rigidbody2D>();
            mainCam = Camera.main;
            currentAmmo = maxAmmo;
        }

        private void Update()
        {
            HandleInput();
        }

        private void FixedUpdate()
        {
            HandleMovement();
            HandleRotation();
        }

        private void HandleInput()
        {
            moveInput.x = Input.GetAxisRaw("Horizontal");
            moveInput.y = Input.GetAxisRaw("Vertical");
            mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

            if (Input.GetMouseButtonDown(0))
            {
                Shoot();
            }
        }

        private void HandleMovement()
        {
            playerRb.MovePosition(playerRb.position + moveInput.normalized * moveSpeed * Time.fixedDeltaTime);
        }

        private void HandleRotation()
        {
            Vector2 lookDir = mousePos - playerRb.position;
            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
            playerRb.rotation = angle;
        }

        // AMENDED: This function has been rewritten for accuracy.
        private void Shoot()
        {
            if (currentAmmo <= 0) return;

            currentAmmo--;

            // --- Calculate Shot Origin and Direction ---
            // The direction is simply from our position to the mouse position.
            Vector2 shotDirection = (mousePos - (Vector2)transform.position).normalized;
            // The origin is a point slightly in front of the player's center.
            Vector3 shotOrigin = transform.position + (Vector3)shotDirection * 0.5f;

            // --- Fire the Raycast ---
            RaycastHit2D hitInfo = Physics2D.Raycast(shotOrigin, shotDirection, Mathf.Infinity, enemyLayers);

            // --- Create the Visual Effect ---
            if (shotEffectPrefab != null)
            {
                Vector3 endPoint = hitInfo.collider != null ? (Vector3)hitInfo.point : shotOrigin + (Vector3)shotDirection * 100f;
                GameObject shotEffect = Instantiate(shotEffectPrefab, Vector3.zero, Quaternion.identity);
                shotEffect.GetComponent<ShotEffect_D>().SetPoints(shotOrigin, endPoint);
            }

            // --- Deal Damage ---
            if (hitInfo.collider != null)
            {
                Debug.Log("Hit: " + hitInfo.collider.name);
                Health_D targetHealth = hitInfo.collider.GetComponent<Health_D>();
                if (targetHealth != null)
                {
                    targetHealth.TakeDamage(attackDamage);
                }
            }
        }

        // Sets the player's current ammo to the maximum amount.
        public void ReplenishAmmo()
        {
            currentAmmo = maxAmmo;
            Debug.Log("Ammo replenished to max! Current: " + currentAmmo);
        }
    }
}