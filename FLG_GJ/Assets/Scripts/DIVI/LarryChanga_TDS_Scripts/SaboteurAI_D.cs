using UnityEngine;

// AMENDED: Added the namespace to match your other scripts.
namespace TopDownShooter
{
    public class SaboteurAI_D : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private float attackDamage = 10f;
        [SerializeField] private float attackRange = 1f;
        [SerializeField] private float attackCoolDown = 2f;
        private float lastAttackTime = -1f;

        // Private references for the player target.
        private Transform playerTarget;
        private Health_D playerHealth;
        private Rigidbody2D saboteurRb;

        private void Start()
        {
            saboteurRb = GetComponent<Rigidbody2D>();

            // This AI's only goal is to find the GameObject with the "Player" tag.
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerTarget = playerObj.transform;
                playerHealth = playerObj.GetComponent<Health_D>();
            }
            else
            {
                // If the player can't be found, this enemy will be disabled to prevent errors.
                Debug.LogError(gameObject.name + ": Saboteur AI cannot find Player! Make sure the Player is tagged 'Player'.");
                this.enabled = false;
            }
        }

        private void FixedUpdate()
        {
            // If there's no target, do nothing.
            if (playerTarget == null) return;

            // Calculate the distance to the player.
            float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);

            if (distanceToPlayer <= attackRange)
            {
                // If in range, stop moving and try to attack.
                saboteurRb.linearVelocity = Vector2.zero;
                Attack();
            }
            else
            {
                // If not in range, move towards the player.
                Vector2 direction = (playerTarget.position - transform.position).normalized;
                saboteurRb.linearVelocity = direction * moveSpeed;
            }
        }

        void Attack()
        {
            // Check if the attack is off cooldown.
            if (Time.time >= lastAttackTime + attackCoolDown)
            {
                Debug.Log(gameObject.name + " is attacking the Player!");

                if (playerHealth != null)
                {
                    // Call the TakeDamage function on the player's Health_D script.
                    playerHealth.TakeDamage(attackDamage);
                }

                // Reset the attack timer.
                lastAttackTime = Time.time;
            }
        }
    }
}