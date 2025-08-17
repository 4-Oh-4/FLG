using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCombat_D : MonoBehaviour
{
    // ## Core Stats ##
    [Header("Stats")]
    [SerializeField] private float health = 100f;
    [SerializeField] private float moveSpeed = 5f;
    private float moveInput;

    // ## Combat Stats ##
    [Header("Combat")]
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float criticalDamage = 25f;
    [SerializeField] private float criticalChance = 0.1f; // 10% chance
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private float attackDuration = 0.3f;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayers;

    // ## Sound Effects ##
    [Header("Sound Effects")]
    [SerializeField] private AudioClip[] effortClips; // AMENDED: Corrected typo from "effortClip"
    [SerializeField] private AudioClip[] punchImpactClips;
    [SerializeField] private AudioClip[] hurtClips;
    [SerializeField] private AudioClip[] blockClips;
    [SerializeField] private AudioClip criticalHitClip;

    // ## Private State & Component References ##
    private Rigidbody2D rb;
    private Animator anim;
    private AudioSource audioSource;
    private UIManager_D uiManager;
    private EnemyAI_D enemyAI;
    private bool isBlocking = false;
    private bool isAttacking = false;

    void Start()
    {
        // Get all necessary components attached to this GameObject
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        // Find other objects in the scene
        uiManager = FindAnyObjectByType<UIManager_D>();

        // AMENDED: Made finding the enemy more efficient
        EnemyAI_D foundEnemy = FindAnyObjectByType<EnemyAI_D>();
        if (foundEnemy != null)
        {
            enemyAI = foundEnemy;
        }

        // Initialize the UI with starting health
        if (uiManager != null)
        {
            uiManager.UpdatePlayerHealth(health, 100f);
        }
    }

    void Update()
    {
        // --- Input Handling ---

        // Check for blocking input
        isBlocking = Input.GetKey(KeyCode.LeftControl);
        anim.SetBool("IsBlocking", isBlocking);

        // Handle movement input only if not busy with another action
        if (!isBlocking && !isAttacking)
        {
            moveInput = Input.GetAxisRaw("Horizontal");
        }
        else
        {
            moveInput = 0;
        }
        anim.SetFloat("Speed", Mathf.Abs(moveInput));

        // Handle attack input
        if (Input.GetKeyDown(KeyCode.Space) && !isBlocking && !isAttacking)
        {
            StartCoroutine(AttackCoroutine());
        }
    }

    void FixedUpdate()
    {
        // Apply physics-based movement
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    private IEnumerator AttackCoroutine()
    {
        // Alert the enemy that an attack is starting so it can react
        if (enemyAI != null)
        {
            enemyAI.OnPlayerAttack();
        }

        // Start the attack state
        isAttacking = true;
        anim.SetTrigger("Punch");
        PlayRandomSound(effortClips); // Play attack effort sound

        yield return new WaitForSeconds(0.1f); // Wait for animation to extend

        // Check if the attack hit anything on the enemy layer
        Collider2D hitEnemy = Physics2D.OverlapCircle(attackPoint.position, attackRange, enemyLayers);

        if (hitEnemy != null)
        {
            // Determine if the hit is a critical
            bool isCritical = Random.value < criticalChance;
            float damageToDeal = isCritical ? criticalDamage : attackDamage;

            if (isCritical) Debug.Log("Player Landed a CRITICAL HIT!");

            // Call the enemy's TakeDamage function, passing all required info
            hitEnemy.GetComponent<EnemyAI_D>().TakeDamage(damageToDeal, isCritical);
        }

        // Wait for the attack animation to finish
        yield return new WaitForSeconds(attackDuration - 0.1f);
        isAttacking = false;
    }

    public void TakeDamage(float damage, bool isCritical)
    {
        // This function is called by the ENEMY when it attacks US

        if (isBlocking)
        {
            Debug.Log("Player blocked the attack!");
            anim.SetTrigger("Block");
            PlayRandomSound(blockClips);
            return; // Exit, no damage taken
        }

        // Play feedback for getting hit
        anim.SetTrigger("Hurt");
        PlayRandomSound(hurtClips);

        // Play the correct impact sound (critical or normal)
        if (isCritical)
        {
            if (criticalHitClip != null) audioSource.PlayOneShot(criticalHitClip);
        }
        else
        {
            PlayRandomSound(punchImpactClips);
        }

        // Apply damage and update UI
        health -= damage;
        if (uiManager != null) uiManager.UpdatePlayerHealth(health, 100f);

        // Check for defeat
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player Defeated!");
        anim.SetTrigger("Defeated");

        // Disable all interactions
        this.enabled = false;
        GetComponent<Collider2D>().enabled = false;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
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
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}