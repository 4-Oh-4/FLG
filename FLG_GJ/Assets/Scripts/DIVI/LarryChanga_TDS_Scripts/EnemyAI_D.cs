using System.Collections;
using UnityEngine;

namespace TopDownShooter
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Health_D))]
    public class EnemyAI_D : MonoBehaviour
    {
        public enum TargetType { Player, Package }
        [SerializeField] private TargetType targetType = TargetType.Package;

        [Header("Stats")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float attackDamage = 20f;
        [SerializeField] private float attackRange = 1.2f;
        [SerializeField] private float stoppingDistance = 1f;
        [SerializeField] private float attackCoolDown = 1.5f;
        [SerializeField] private float attackDuration = 0.5f;

        private Transform target;
        private Health_D targetHealth;
        private Rigidbody2D enemyRb;
        private float lastAttackTime = -1f;
        private bool isAttacking = false;

        private void Start()
        {
            enemyRb = GetComponent<Rigidbody2D>();



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
                    targetHealth = playerObj.GetComponent<Health_D>();
                }
            }
            else
            {
                GameObject packageObj = GameObject.Find("Package_D");
                if (packageObj != null)
                {
                    target = packageObj.transform;
                    targetHealth = packageObj.GetComponent<Health_D>();
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

            if (target != null && Vector2.Distance(transform.position, target.position) <= attackRange)
            {
                if (targetHealth != null)
                {
                    targetHealth.TakeDamage(attackDamage);
                }
            }

            isAttacking = false;
        }

        // AMENDED: Added Gizmos for clarity.
        private void OnDrawGizmosSelected()
        {
            // Draw a red wire sphere to visualize the Attack Range.
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);

            // Draw a blue wire sphere to visualize the Stopping Distance.
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, stoppingDistance);
        }
    }
}