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

        private float currentHealth;
        private Transform target;
        private PlayerController_D playerController;
        private Package_D package;
        private Rigidbody2D enemyRb;
        private Animator animator;
        private WaveSpawner_D spawner;
        private float lastAttackTime = -1f;
        private bool isAttacking = false;

        public void Initialize(WaveSpawner_D spawnerRef) { spawner = spawnerRef; }

        private void Awake()
        {
            enemyRb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            currentHealth = maxHealth;
            isAttacking = false;

            if (gameObject.name.Contains("Rusher_D"))
            {
                if (Random.value < 0.5f) { targetType = TargetType.Player; }
                else { targetType = TargetType.Package; }
            }

            if (targetType == TargetType.Player)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null)
                {
                    target = playerObj.transform;
                    playerController = playerObj.GetComponent<PlayerController_D>();
                }
            }
            else
            {
                GameObject packageObj = GameObject.Find("Package_D");
                if (packageObj != null)
                {
                    target = packageObj.transform;
                    package = packageObj.GetComponent<Package_D>();
                }
            }
        }

        private void FixedUpdate()
        {
            if (target == null || isAttacking)
            {
                if (isAttacking) enemyRb.linearVelocity = Vector2.zero;
                return;
            }

            Vector2 direction = (target.position - transform.position).normalized;
            float distanceToTarget = Vector2.Distance(transform.position, target.position);

            if (distanceToTarget > stoppingDistance)
            {
                
                enemyRb.linearVelocity = direction * moveSpeed; // Corrected to velocity
            }
            else
            {
                enemyRb.linearVelocity = Vector2.zero; // Corrected to velocity
            }

            Vector2 moveDirection = enemyRb.linearVelocity.normalized;
            animator.SetFloat("InputX", moveDirection.x);
            animator.SetFloat("InputY", moveDirection.y);

            if (distanceToTarget <= attackRange)
            {
                Attack();
            }
        }

        void Attack()
        {
            if (Time.time >= lastAttackTime + attackCoolDown && !isAttacking)
            {
                lastAttackTime = Time.time;
                StartCoroutine(AttackCoroutine());
            }
        }

        IEnumerator AttackCoroutine()
        {
            isAttacking = true;

            if (target != null)
            {
                // Set direction so the enemy punches towards the target
                Vector2 directionToTarget = (target.position - transform.position).normalized;
                animator.SetFloat("InputX", directionToTarget.x);
                animator.SetFloat("InputY", directionToTarget.y);

                // Trigger the attack animation
                animator.SetTrigger("Attack");
            }

            yield return new WaitForSeconds(attackDuration);

            if (target != null && Vector2.Distance(transform.position, target.position) <= attackRange)
            {
                if (targetType == TargetType.Player && playerController != null) playerController.TakeDamage(attackDamage);
                else if (targetType == TargetType.Package && package != null) package.TakeDamage(attackDamage);
            }
            isAttacking = false;
        }

        public void TakeDamage(float damage)
        {
            if (currentHealth <= 0) return;
            currentHealth -= damage;
            if (currentHealth <= 0) Die();
        }

        void Die()
        {
            if (spawner != null) spawner.OnEnemyDied();
            if (Random.value <= ammoDropChance && ammoDropPrefab != null)
            {
                // Use the object pooler to spawn loot.
                ObjectPooler_D.Instance.SpawnFromPool(ammoDropPrefab.name, transform.position, Quaternion.identity);
            }
            gameObject.SetActive(false);
        }
    }
}