using System.Collections;
using UnityEngine;

public class EnemyAI_D : MonoBehaviour
{
    // ## Core stats ##
    [SerializeField] private float health = 100f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float attackDamage = 5f;

    // ## AI Behaviour ##
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float stoppingDistance = 1.2f;
    private Transform player;
    private PlayerCombat_D playerCombat;

    // ## State & Timers ##
    private float distanceToPlayer;
    private bool isBlocking = false;
    private float decisionTimer = 0f;
    [SerializeField] private float decisionCooldown = 1.5f;

    // ## Components ##
    private Rigidbody2D rb;
    private Vector3 initialScale;
    private Animator anim; // ANIM: Reference to the Animator component

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        initialScale = transform.localScale;
        anim = GetComponent<Animator>(); // ANIM: Get the Animator component

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
    }

    private void Update()
    {
        if (player == null) return;

        FacePlayer();
        distanceToPlayer = Vector2.Distance(transform.position, player.position);
        decisionTimer -= Time.deltaTime;

        // --- AI Logic ---
        if (distanceToPlayer > stoppingDistance)
        {
            ApproachPlayer();
        }
        else
        {
            StopMovement();
        }

        if (distanceToPlayer <= attackRange && decisionTimer <= 0)
        {
            MakeCombatDecision();
            decisionTimer = decisionCooldown;
        }

        // ANIM: Set the speed parameter for Idle/Run animations
        anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
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

    void MakeCombatDecision()
    {
        int randomDecision = Random.Range(0, 3);
        switch (randomDecision)
        {
            case 0: AttackPlayer(); break;
            case 1: StartCoroutine(BlockCoroutine()); break;
            case 2: Debug.Log("Enemy Waits ..."); break;
        }
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

    void AttackPlayer()
    {
        if (playerCombat != null)
        {
            anim.SetTrigger("Punch"); // ANIM: Trigger the punch animation
            Debug.Log("Enemy Attacks!");
            playerCombat.TakeDamage(attackDamage);
        }
    }

    IEnumerator BlockCoroutine()
    {
        isBlocking = true;
        anim.SetBool("IsBlocking", true); // ANIM: Set blocking animation state to true
        Debug.Log("Enemy is blocking!");

        yield return new WaitForSeconds(1.0f);

        isBlocking = false;
        anim.SetBool("IsBlocking", false); // ANIM: Set blocking animation state to false
    }

    public void TakeDamage(float damage)
    {
        if (isBlocking)
        {
            Debug.Log("Enemy blocked the attack!");
            return;
        }

        anim.SetTrigger("Hurt"); // ANIM: Trigger the get-hit animation
        health -= damage;
        Debug.Log("Enemy Health: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Enemy Defeated!");
        anim.SetTrigger("Defeated"); // ANIM: Trigger the death animation

        // ANIM: Disable AI and physics to stop interactions after death
        this.enabled = false;
        GetComponent<Collider2D>().enabled = false;
        rb.bodyType = RigidbodyType2D.Kinematic; // Correct and modern
        rb.linearVelocity = Vector2.zero;

        // Destroy the GameObject after 2 seconds to allow the animation to play
        Destroy(gameObject, 2f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);
    }
}