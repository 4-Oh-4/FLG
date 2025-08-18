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
        [SerializeField] private float attackRange = 1f;
        [SerializeField] private float attackCoolDown = 1.5f;

        private Transform target;
        private Health_D targetHealth;
        private Rigidbody2D enemyRb;
        private float lastAttackTime = -1f;

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
            if (target == null) return;

            float distanceToTarget = Vector2.Distance(transform.position, target.position);

            if (distanceToTarget <= attackRange)
            {
                enemyRb.linearVelocity = Vector2.zero;
                Attack();
            }
            else
            {
                Vector2 direction = (target.position - transform.position).normalized;
                enemyRb.linearVelocity = direction * moveSpeed;
            }
        }

        void Attack()
        {
            if (Time.time >= lastAttackTime + attackCoolDown)
            {
                if (targetHealth != null)
                {
                    targetHealth.TakeDamage(attackDamage);
                }
                lastAttackTime = Time.time;
            }
        }
    }
}