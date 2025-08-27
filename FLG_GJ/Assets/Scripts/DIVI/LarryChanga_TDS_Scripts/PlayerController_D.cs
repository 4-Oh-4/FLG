using UnityEngine;

namespace TopDownShooter
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController_D : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] private float maxHealth = 200f;
        [SerializeField] private float moveSpeed = 5f;

        [Header("Combat")]
        [SerializeField] private int maxAmmo = 60;
        [SerializeField] private float attackDamage = 25f;
        [SerializeField] private LayerMask enemyLayers;
        [SerializeField] private GameObject shotEffectPrefab;

        private float currentHealth;
        private int currentAmmo;
        private Rigidbody2D playerRb;
        private Camera mainCam;
        private Vector2 moveInput;
        private Vector2 mousePos;

        private void Start()
        {
            playerRb = GetComponent<Rigidbody2D>();
            mainCam = Camera.main;
            currentHealth = maxHealth;
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
            if (currentAmmo <= 0) return;
            currentAmmo--;

            Vector2 shotDirection = ((Vector2)mousePos - (Vector2)transform.position).normalized;
            RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, shotDirection, Mathf.Infinity, enemyLayers);

            if (shotEffectPrefab != null)
            {
                Vector3 endPoint = hitInfo.collider != null ? (Vector3)hitInfo.point : (Vector3)transform.position + (Vector3)shotDirection * 100f;
                // Use the object pooler to create the shot effect.
                GameObject shotEffect = ObjectPooler_D.Instance.SpawnFromPool("ShotEffect", Vector3.zero, Quaternion.identity);
                shotEffect.GetComponent<ShotEffect_D>().SetPoints(transform.position, endPoint);
            }

            if (hitInfo.collider != null)
            {
                // Damage logic now checks for both enemy types.
                if (hitInfo.collider.TryGetComponent<EnemyAI_D>(out var standardEnemy)) standardEnemy.TakeDamage(attackDamage);
                else if (hitInfo.collider.TryGetComponent<SaboteurAI_D>(out var saboteurEnemy)) saboteurEnemy.TakeDamage(attackDamage);
            }
        }

        public void TakeDamage(float damage)
        {
            if (currentHealth <= 0) return;
            currentHealth -= damage;
            if (currentHealth <= 0) Die();
        }

        public void ReplenishAmmo()
        {
            currentAmmo = maxAmmo;
        }

        void Die()
        {
            this.enabled = false;
        }
    }
}