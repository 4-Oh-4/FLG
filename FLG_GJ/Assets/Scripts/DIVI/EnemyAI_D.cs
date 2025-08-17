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
    private bool isRetreating = false; // NEW: State to check if we are backing away
    private float decisionTimer = 0f;
    [SerializeField] private float decisionCooldown = 1.0f; // Lowered default for faster action

    // ## Components ##
    private Rigidbody2D rb;
    private Vector3 initialScale;
    private Animator anim;

    // ## UI Reference ##
    private UIManager_D uiManager;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        initialScale = transform.localScale;
        anim = GetComponent<Animator>();
        uiManager = FindAnyObjectByType<UIManager_D>();

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

        uiManager.UpdateEnemyHealth(health, 100f);
    }

    private void Update()
    {
        if (player == null) return;

        FacePlayer();
        distanceToPlayer = Vector2.Distance(transform.position, player.position);
        decisionTimer -= Time.deltaTime;

        // MODIFIED: AI will not try to approach if it's in the middle of a retreat
        if (distanceToPlayer > stoppingDistance && !isRetreating)
        {
            ApproachPlayer();
        }
        else if (!isRetreating) // Don't stop movement if we are actively retreating
        {
            StopMovement();
        }

        if (distanceToPlayer <= attackRange && decisionTimer <= 0)
        {
            MakeCombatDecision();
            decisionTimer = decisionCooldown;
        }

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
        // MODIFIED: Changed range from 3 to 4 to add a new "Back Away" option
        int randomDecision = Random.Range(0, 4);
        switch (randomDecision)
        {
            case 0: AttackPlayer(); break;
            case 1: StartCoroutine(BlockCoroutine()); break;
            case 2: Debug.Log("Enemy Waits ..."); break;
            case 3: StartCoroutine(BackAwayCoroutine()); break; // NEW: Call the retreat coroutine
        }
    }

    // NEW: Coroutine for the back away action
    IEnumerator BackAwayCoroutine()
    {
        Debug.Log("Enemy is backing away!");
        isRetreating = true; // Set the retreating state to true

        // Determine the direction away from the player
        float direction = (transform.position.x > player.position.x) ? 1 : -1;

        // Apply velocity to move backward
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);

        // Wait for a short duration
        yield return new WaitForSeconds(0.4f);

        // Stop retreating so the main AI logic can take over again
        isRetreating = false;
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
            anim.SetTrigger("Punch");
            Debug.Log("Enemy Attacks!");
            playerCombat.TakeDamage(attackDamage);
        }
    }

    IEnumerator BlockCoroutine()
    {
        isBlocking = true;
        anim.SetBool("IsBlocking", true);
        Debug.Log("Enemy is blocking!");

        yield return new WaitForSeconds(1.0f);

        isBlocking = false;
        anim.SetBool("IsBlocking", false);
    }

    public void TakeDamage(float damage)
    {
        if (isBlocking)
        {
            Debug.Log("Enemy blocked the attack!");
            return;
        }

        anim.SetTrigger("Hurt");
        health -= damage;
        uiManager.UpdateEnemyHealth(health, 100f);
        Debug.Log("Enemy Health: " + health);

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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);
    }
}