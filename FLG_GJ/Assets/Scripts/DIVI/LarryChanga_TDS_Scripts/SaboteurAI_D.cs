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

        private float currentHealth;
        private Transform playerTarget;
        private PlayerController_D playerController;
        private Rigidbody2D saboteurRb;
        private WaveSpawner_D spawner;
        private float lastAttackTime = -1f;
        private bool isAttacking = false;

        public void Initialize(WaveSpawner_D spawnerRef) { spawner = spawnerRef; }

        private void Awake()
        {
            saboteurRb = GetComponent<Rigidbody2D>();
        }

        private void OnEnable()
        {
            currentHealth = maxHealth;
            isAttacking = false;

            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerTarget = playerObj.transform;
                playerController = playerObj.GetComponent<PlayerController_D>();
            }
            else
            {
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

            if (distanceToPlayer > stoppingDistance)
            {
                Vector2 direction = (playerTarget.position - transform.position).normalized;
                saboteurRb.linearVelocity = direction * moveSpeed; // Corrected to velocity
            }
            else
            {
                saboteurRb.linearVelocity = Vector2.zero; // Corrected to velocity
            }

            if (distanceToPlayer <= attackRange)
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
            yield return new WaitForSeconds(attackDuration);

            if (playerTarget != null && Vector2.Distance(transform.position, playerTarget.position) <= attackRange)
            {
                if (playerController != null)
                {
                    playerController.TakeDamage(attackDamage);
                }
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