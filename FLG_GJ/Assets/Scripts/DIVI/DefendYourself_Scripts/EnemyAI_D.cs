using System.Collections;
using UnityEngine;

namespace DefendYourself
{
    public class EnemyAI_D : MonoBehaviour
    {
        // ## Core stats ##
        [Header("Stats")]
        [SerializeField] private float health = 100f;
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private float attackDamage = 5f;
        [SerializeField] private float criticalDamage = 10f;
        [SerializeField] private float criticalChance = 0.05f; // 5% chance

        // ## AI Behaviour ##
        [Header("AI Behaviour")]
        [SerializeField] private float attackRange = 1.5f;
        [SerializeField] private float stoppingDistance = 1.2f;
        [SerializeField] private float decisionCooldown = 1.0f;

        // ## Reactive Defense ##
        [Header("Reactive Defense")]
        [SerializeField] private float spamThreshold = 1.0f;
        [SerializeField][Range(0, 1)] private float retreatChance = 0.33f;
        [SerializeField][Range(0, 1)] private float spamBlockChance = 0.6f;

        // ## Sound Effects ##
        [Header("Sound Effects")]
        [SerializeField] private AudioClip[] effortClips;
        [SerializeField] private AudioClip[] punchImpactClips;
        [SerializeField] private AudioClip[] hurtClips;
        [SerializeField] private AudioClip[] blockClips;
        [SerializeField] private AudioClip criticalHitClip;

        // ## Private State & Component References ##
        private Rigidbody2D rb;
        private Animator anim;
        private AudioSource audioSource;
        private UIManager_D uiManager;
        private Transform player;
        private PlayerCombat_D playerCombat;
        private Vector3 initialScale;
        private float distanceToPlayer;
        private float decisionTimer = 0f;
        private float lastPlayerAttackTime = -1f;
        private bool isBlocking = false;
        private bool isRetreating = false;
        private bool isAttacking = false;

        private void Start()
        {
            // Get all necessary components attached to this GameObject
            rb = GetComponent<Rigidbody2D>();
            initialScale = transform.localScale;
            anim = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
            uiManager = FindAnyObjectByType<UIManager_D>();

            // Find the player and get a reference to their script
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
                playerCombat = playerObject.GetComponent<PlayerCombat_D>();
            }
            else
            {
                Debug.LogError("AI Error: Player object not found! Make sure your player is tagged 'Player'.");
                this.enabled = false;
            }

            // Initialize the UI with starting health
            if (uiManager != null)
            {
                uiManager.UpdateEnemyHealth(health, 100f);
            }
        }

        private void Update()
        {
            if (player == null || isAttacking) return; // Stop AI logic if there's no player or we are in mid-attack

            // Always face the player
            FacePlayer();

            distanceToPlayer = Vector2.Distance(transform.position, player.position);
            decisionTimer -= Time.deltaTime;

            // --- AI Movement Logic ---
            // Do not move if currently retreating or blocking
            if (!isRetreating && !isBlocking)
            {
                if (distanceToPlayer > stoppingDistance)
                {
                    ApproachPlayer();
                }
                else
                {
                    StopMovement();
                }
            }

            // --- AI Combat Logic ---
            // Decide on an action if the player is in range and the AI is ready
            if (distanceToPlayer <= attackRange && decisionTimer <= 0 && !isBlocking && !isRetreating)
            {
                MakeCombatDecision();
                decisionTimer = decisionCooldown;
            }

            // Update the animator with the current speed
            anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        }

        public void OnPlayerAttack()
        {
            // This function is called by the PLAYER when they start an attack

            // Don't react if already busy
            if (isAttacking || isBlocking || isRetreating)
            {
                return;
            }

            // Check if the player's attack is a "spam"
            bool isSpam = (Time.time - lastPlayerAttackTime) < spamThreshold;
            lastPlayerAttackTime = Time.time;

            if (isSpam)
            {
                // If it's a spam, there's a high chance to block
                if (Random.value < spamBlockChance)
                {
                    Debug.Log("AI: Detected spam, blocking!");
                    StartCoroutine(BlockCoroutine());
                }
            }
            else
            {
                // If it's a sudden attack, there's a chance to back away
                if (Random.value < retreatChance)
                {
                    Debug.Log("AI: Sudden attack, backing off!");
                    StartCoroutine(BackAwayCoroutine());
                }
            }
        }

        void MakeCombatDecision()
        {
            // Choose a random action based on percentages
            float randomChance = Random.value;
            if (randomChance <= 0.50f) { StartCoroutine(BackAwayCoroutine()); }
            else if (randomChance <= 0.75f) { AttackPlayer(); }
            else if (randomChance <= 0.90f) { StartCoroutine(BlockCoroutine()); }
            else { Debug.Log("Enemy Waits ..."); }
        }

        void ApproachPlayer()
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);
        }

        void StopMovement()
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }

        void FacePlayer()
        {
            if (player.position.x > transform.position.x)
            {
                transform.localScale = new Vector3(initialScale.x, initialScale.y, initialScale.z);
            }
            else if (player.position.x < transform.position.x)
            {
                transform.localScale = new Vector3(-initialScale.x, initialScale.y, initialScale.z);
            }
        }

        void AttackPlayer()
        {
            if (isAttacking || isBlocking || isRetreating) return;
            StartCoroutine(AttackCoroutine());
        }

        IEnumerator AttackCoroutine()
        {
            isAttacking = true;
            StopMovement(); // Stop moving to play the attack animation
            anim.SetTrigger("Punch");
            PlayRandomSound(effortClips);

            yield return new WaitForSeconds(0.2f);

            if (playerCombat != null && Vector2.Distance(transform.position, player.position) <= attackRange)
            {
                bool isCritical = Random.value < criticalChance;
                float damageToDeal = isCritical ? criticalDamage : attackDamage;
                if (isCritical) Debug.Log("Enemy Landed a CRITICAL HIT!");
                playerCombat.TakeDamage(damageToDeal, isCritical);
            }

            yield return new WaitForSeconds(0.3f);
            isAttacking = false;
        }

        IEnumerator BlockCoroutine()
        {
            isBlocking = true;
            StopMovement();
            anim.SetBool("IsBlocking", true);
            Debug.Log("Enemy is blocking!");
            yield return new WaitForSeconds(1.0f);
            isBlocking = false;
            anim.SetBool("IsBlocking", false);
        }

        IEnumerator BackAwayCoroutine()
        {
            isRetreating = true;
            float direction = (transform.position.x > player.position.x) ? 1 : -1;
            rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
            yield return new WaitForSeconds(0.4f);
            isRetreating = false;
        }

        public void TakeDamage(float damage, bool isCritical)
        {
            // This function is called by the PLAYER when they attack US

            if (isBlocking)
            {
                Debug.Log("Enemy blocked the attack!");
                anim.SetTrigger("Block");
                PlayRandomSound(blockClips);
                return;
            }

            anim.SetTrigger("Hurt");
            PlayRandomSound(hurtClips);

            if (isCritical)
            {
                if (criticalHitClip != null) audioSource.PlayOneShot(criticalHitClip);
            }
            else
            {
                PlayRandomSound(punchImpactClips);
            }

            health -= damage;
            if (uiManager != null) uiManager.UpdateEnemyHealth(health, 100f);

            if (health <= 0)
            {
                Die();
            }
        }

        void Die()
        {
            Debug.Log("Enemy Defeated!");
            anim.SetTrigger("Defeated");
            this.enabled = false;
            GetComponent<Collider2D>().enabled = false;
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.linearVelocity = Vector2.zero;
            Destroy(gameObject, 2f);
        }

        private void PlayRandomSound(AudioClip[] clips)
        {
            if (clips != null && clips.Length > 0)
            {
                int randomIndex = Random.Range(0, clips.Length);
                if (clips[randomIndex] != null)
                {
                    audioSource.PlayOneShot(clips[randomIndex]);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, stoppingDistance);
        }
    }
}