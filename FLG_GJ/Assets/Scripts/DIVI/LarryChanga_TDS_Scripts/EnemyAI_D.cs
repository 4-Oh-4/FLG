using System.Collections;
using UnityEngine;

namespace TopDownShooter
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyAI_D : MonoBehaviour
    {
        public enum TargetType { Player, Package }
        [SerializeField] private TargetType targetType = TargetType.Package;

        [Header("Stats")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float attackDamage = 20f;
        [SerializeField] private float attackRange = 1.2f;
        [SerializeField] private float stoppingDistance = 1f;
        [SerializeField] private float attackCoolDown = 1.5f;
        [SerializeField] private float attackDuration = 0.5f;

        [Header("Loot")]
        [SerializeField] private GameObject ammoDropPrefab;
        [SerializeField][Range(0, 1)] private float ammoDropChance = 0.75f;

        // --- Private References & State ---
        private float currentHealth;
        private Transform target;
        private PlayerController_D playerController;
        private Package_D package;
        private Rigidbody2D enemyRb;
        private WaveSpawner_D spawner;
        private float lastAttackTime = -1f;
        private bool isAttacking = false;



        // This function is called by the spawner when this enemy is created.
        public void Initialize(WaveSpawner_D spawnerRef)
        {
            spawner = spawnerRef;
        }

        private void Awake()
        {
            enemyRb = GetComponent<Rigidbody2D>();
        }

        // Initializes components and finds the AI's target.
        private void OnEnable()
        {
            
            currentHealth = maxHealth;

            // If this is a Rusher, randomize its target.
            if (gameObject.name.Contains("Rusher_D"))
            {
                if (Random.value < 0.5f) { targetType = TargetType.Player; }
                else { targetType = TargetType.Package; }
            }

            // Find and store a reference to the chosen target.
            if (targetType == TargetType.Player)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null)
                {
                    target = playerObj.transform;
                    playerController = playerObj.GetComponent<PlayerController_D>();
                }
            }
            else // Target is the Package
            {
                GameObject packageObj = GameObject.Find("Package_D");
                if (packageObj != null)
                {
                    target = packageObj.transform;
                    package = packageObj.GetComponent<Package_D>();
                }
            }
        }

        // Handles AI movement and attack logic at a fixed time step.
        private void FixedUpdate()
        {
            if (target == null || isAttacking)
            {
                if (isAttacking) enemyRb.linearVelocity = Vector2.zero;
                return;
            }

            float distanceToTarget = Vector2.Distance(transform.position, target.position);

            if (distanceToTarget > stoppingDistance)
            {
                Vector2 direction = (target.position - transform.position).normalized;
                enemyRb.linearVelocity = direction * moveSpeed;
            }
            else
            {
                enemyRb.linearVelocity = Vector2.zero;
            }

            if (distanceToTarget <= attackRange)
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

            // After the delay, check if the target is still in range and deal damage.
            if (target != null && Vector2.Distance(transform.position, target.position) <= attackRange)
            {
                if (targetType == TargetType.Player && playerController != null)
                {
                    playerController.TakeDamage(attackDamage);
                }
                else if (targetType == TargetType.Package && package != null)
                {
                    package.TakeDamage(attackDamage);
                }
            }
            isAttacking = false;
        }

        // Called by the player's bullets to deal damage to this enemy.
        public void TakeDamage(float damage)
        {
            if (currentHealth <= 0) return;
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        // Handles death logic, including loot drops.
        void Die()
        {
            
                // Tell the spawner that we have died.
            if (spawner != null)
            {
                spawner.OnEnemyDied();
            }

                // ... (your loot drop logic) ...

            if (Random.value <= ammoDropChance)
            {
                if (ammoDropPrefab != null)
                {
                    Instantiate(ammoDropPrefab, transform.position, Quaternion.identity);
                }
            }

            gameObject.SetActive(false);
        }
    }
}