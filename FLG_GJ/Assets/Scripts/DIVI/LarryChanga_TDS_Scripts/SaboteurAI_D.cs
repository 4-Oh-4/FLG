using System.Collections;
using UnityEngine;

namespace TopDownShooter
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class SaboteurAI_D : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] private float maxHealth = 75f;
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private float attackDamage = 10f;
        [SerializeField] private float attackRange = 1.2f;
        [SerializeField] private float stoppingDistance = 1f;
        [SerializeField] private float attackCoolDown = 2f;
        [SerializeField] private float attackDuration = 0.5f;

        [Header("Loot")]
        [SerializeField] private GameObject ammoDropPrefab;
        [SerializeField][Range(0, 1)] private float ammoDropChance = 0.75f;

        // --- Private References & State ---
        private float currentHealth;
        private Transform playerTarget;
        private PlayerController_D playerController;
        private Rigidbody2D saboteurRb;
        private SimpleSpawner_D spawner;
        private float lastAttackTime = -1f;
        private bool isAttacking = false;

        // Called by the spawner to give this enemy a reference to it.
        public void Initialize(SimpleSpawner_D spawnerRef)
        {
            spawner = spawnerRef;
        }

        private void Start()
        {
            saboteurRb = GetComponent<Rigidbody2D>();
            currentHealth = maxHealth;

            // This AI's only goal is to find the player.
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerTarget = playerObj.transform;
                playerController = playerObj.GetComponent<PlayerController_D>();
            }
            else
            {
                // If no player is found, disable this enemy.
                this.enabled = false;
            }
        }

        private void FixedUpdate()
        {
            if (playerTarget == null || isAttacking)
            {
                if (isAttacking) saboteurRb.linearVelocity = Vector2.zero;
                return;
            }

            float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);

            // Move towards the player if not in stopping range.
            if (distanceToPlayer > stoppingDistance)
            {
                Vector2 direction = (playerTarget.position - transform.position).normalized;
                saboteurRb.linearVelocity = direction * moveSpeed;
            }
            else
            {
                saboteurRb.linearVelocity = Vector2.zero;
            }

            // Attack if in attack range.
            if (distanceToPlayer <= attackRange)
            {
                Attack();
            }
        }

        // Starts the attack coroutine if not on cooldown.
        void Attack()
        {
            if (Time.time >= lastAttackTime + attackCoolDown && !isAttacking)
            {
                lastAttackTime = Time.time;
                StartCoroutine(AttackCoroutine());
            }
        }

        // Handles the timed attack sequence.
        IEnumerator AttackCoroutine()
        {
            isAttacking = true;
            yield return new WaitForSeconds(attackDuration);

            // Check if player is still in range after the attack wind-up.
            if (playerTarget != null && Vector2.Distance(transform.position, playerTarget.position) <= attackRange)
            {
                if (playerController != null)
                {
                    playerController.TakeDamage(attackDamage);
                }
            }

            isAttacking = false;
        }

        // Called by the player's bullets to deal damage.
        public void TakeDamage(float damage)
        {
            if (currentHealth <= 0) return;

            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        // Handles death logic.
        void Die()
        {
            // Tell the spawner that we have died.
            if (spawner != null)
            {
                spawner.EnemyDied();
            }

            // Check if we should drop ammo.
            if (Random.value <= ammoDropChance)
            {
                if (ammoDropPrefab != null)
                {
                    Instantiate(ammoDropPrefab, transform.position, Quaternion.identity);
                }
            }

            // Destroy this enemy GameObject.
            Destroy(gameObject);
        }
    }
}