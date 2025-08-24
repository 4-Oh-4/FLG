using UnityEngine;

namespace TruckChase
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class BikeAI_D : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] private float maxHealth = 10f;

        [Header("Movement")]
        [SerializeField] private float forwardSpeed = 6f;
        [SerializeField] private float zigZagFrequency = 4f;
        [SerializeField] private float zigZagMagnitude = 3f;

        private float currentHealth;
        private WaveSpawner_D spawner;

        // This is called by the spawner when the bike is created.
        public void Initialize(WaveSpawner_D spawnerRef)
        {
            spawner = spawnerRef;
        }

        private void Start()
        {
            currentHealth = maxHealth;
        }

        private void Update()
        {
            // Moves the bike upwards.
            transform.position += Vector3.up * forwardSpeed * Time.deltaTime;
            // Adds the horizontal zig-zag motion.
            transform.position += Vector3.right * Mathf.Sin(Time.time * zigZagFrequency) * zigZagMagnitude * Time.deltaTime;

            // If the bike goes off the top of the screen, it dies.
            if (transform.position.y > 12f)
            {
                Die();
            }
        }

        // This is called by projectiles when they hit the bike.
        public void TakeDamage(float damage)
        {
            if (currentHealth <= 0) return;

            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        // This handles the bike's death.
        void Die()
        {
            // If we are already "dead" (e.g., off-screen), don't run this again.
            if (!enabled) return;

            // Tell the spawner that we have died.
            if (spawner != null)
            {
                spawner.OnEnemyDied();
            }

            // Mark this script as disabled and destroy the GameObject.
            enabled = false;
            Destroy(gameObject);
        }
    }
}