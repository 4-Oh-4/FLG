using UnityEngine;

namespace TopDownShooter
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Health_D))]
    public class SaboteurAI_D : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private float attackDamage = 10f;
        [SerializeField] private float attackRange = 1f;
        [SerializeField] private float attackCoolDown = 2f;

        private Transform playerTarget;
        private Health_D playerHealth;
        private Rigidbody2D saboteurRb;
        private float lastAttackTime = -1f;

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
            if (playerTarget == null) return;

            float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);

            if (distanceToPlayer <= attackRange)
            {
                saboteurRb.linearVelocity = Vector2.zero;
                Attack();
            }
            else
            {
                Vector2 direction = (playerTarget.position - transform.position).normalized;
                saboteurRb.linearVelocity = direction * moveSpeed;
            }
        }

        void Attack()
        {
            if (Time.time >= lastAttackTime + attackCoolDown)
            {
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(attackDamage);
                }
                lastAttackTime = Time.time;
            }
        }
    }
}