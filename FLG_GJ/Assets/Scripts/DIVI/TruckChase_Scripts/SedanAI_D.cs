using UnityEngine;

namespace TruckChase
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class SedanAI_D : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] private float maxHealth = 30f;

        [Header("Movement")]
        [SerializeField] private float forwardSpeed = 4f;
        [SerializeField] private float ramSpeedMultiplier = 2.5f;
        [SerializeField] private float ramDistance = 5f;

        private float currentHealth;
        private WaveSpawner_D spawner;
        private Transform lorryTarget;
        private bool isRamming = false;

        // Called by the spawner.
        public void Initialize(WaveSpawner_D spawnerRef)
        {
            spawner = spawnerRef;
        }

        private void Start()
        {
            currentHealth = maxHealth;
            // Find the lorry once at the start.
            GameObject lorryObject = GameObject.Find("Lorry");
            if (lorryObject != null)
            {
                lorryTarget = lorryObject.transform;
            }
        }

        private void Update()
        {
            // --- Ramming Logic ---
            // AMENDED: This 'if' now checks if we are NOT already ramming.
            if (!isRamming && lorryTarget != null)
            {
                // Check if we are close enough to the lorry.
                if (Vector3.Distance(transform.position, lorryTarget.position) <= ramDistance)
                {
                    // Activate ramming mode and increase speed only once.
                    isRamming = true;
                    forwardSpeed *= ramSpeedMultiplier;
                }
            }

            // --- Movement ---
            transform.position += Vector3.up * forwardSpeed * Time.deltaTime;

            if (transform.position.y > 12f)
            {
                Die();
            }
        }

        // Called by projectiles.
        public void TakeDamage(float damage)
        {
            if (currentHealth <= 0) return;
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        // Handles death.
        void Die()
        {
            if (!enabled) return;

            if (spawner != null)
            {
                spawner.OnEnemyDied();
            }

            enabled = false;
            Destroy(gameObject);
        }
    }
}