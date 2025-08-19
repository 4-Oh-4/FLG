using System.Collections;
using UnityEngine;

namespace TopDownShooter
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Health_D))]
    public class SaboteurAI_D : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private float attackDamage = 10f;
        [SerializeField] private float attackRange = 1.2f;
        [SerializeField] private float stoppingDistance = 1f;
        [SerializeField] private float attackCoolDown = 2f;
        [SerializeField] private float attackDuration = 0.5f;

        private Transform playerTarget;
        private Health_D playerHealth;
        private Rigidbody2D saboteurRb;
        private float lastAttackTime = -1f;
        private bool isAttacking = false;

        private void Start()
        {
            saboteurRb = GetComponent<Rigidbody2D>();

            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerTarget = playerObj.transform;
                playerHealth = playerObj.GetComponent<Health_D>();
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
                saboteurRb.linearVelocity = direction * moveSpeed;
            }
            else
            {
                saboteurRb.linearVelocity = Vector2.zero;
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
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(attackDamage);
                }
            }

            isAttacking = false;
        }

        // AMENDED: Added Gizmos for clarity.
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, stoppingDistance);
        }
    }
}