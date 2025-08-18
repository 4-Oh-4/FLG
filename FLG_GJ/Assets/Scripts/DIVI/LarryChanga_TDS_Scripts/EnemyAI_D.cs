using UnityEngine;

// All scripts for this game will be in the TopDownShooter namespace.
namespace TopDownShooter
{
    public class EnemyAI_D : MonoBehaviour
    {
        public enum TargetType { Player, Package }
        [SerializeField] private TargetType targetType = TargetType.Package;

        [Header("Stats")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float attackDamage = 20f;
        [SerializeField] private float attackRange = 1f;
        [SerializeField] private float attackCoolDown = 1.5f;
        private float lastAttackTime = -1f;

        private Transform target;
        private Health_D targetHealth;
        private Rigidbody2D enemyRb;

        private void Start()
        {
            enemyRb = GetComponent<Rigidbody2D>();

            // The GameObject name must contain "Rusher_D" for this to work.
            if (gameObject.name.Contains("Rusher_D"))
            {
                // 50/50 chance to target the Player or the Package.
                if (Random.value < 0.5f)
                {
                    targetType = TargetType.Player;
                }
                else
                {
                    targetType = TargetType.Package;
                }
            }

            if (targetType == TargetType.Player)
            {
                // The Player GameObject must have the "Player" tag.
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null)
                {
                    target = playerObj.transform;
                    targetHealth = playerObj.GetComponent<Health_D>();
                }
                else
                {
                    Debug.LogError(gameObject.name + ": AI cannot find Player! Make sure the Player is tagged 'Player'.");
                }
            }
            else // Target is the Package.
            {
                // The Package GameObject must be named exactly "Package_D".
                GameObject packageObj = GameObject.Find("Package_D");
                if (packageObj != null)
                {
                    target = packageObj.transform;
                    targetHealth = packageObj.GetComponent<Health_D>();
                }
                else
                {
                    Debug.LogError(gameObject.name + ": AI cannot find Package! Make sure it is named 'Package_D'.");
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
                Debug.Log(gameObject.name + " is Attacking " + target.name);

                if (targetHealth != null)
                {
                    targetHealth.TakeDamage(attackDamage);
                }

                lastAttackTime = Time.time;
            }
        }
    }
}