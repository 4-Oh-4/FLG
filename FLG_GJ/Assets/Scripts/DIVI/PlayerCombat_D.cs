using System.Collections;
using UnityEngine;

public class PlayerCombat_D : MonoBehaviour
{
    // ## Core Stats ##
    [SerializeField] private float health = 100f;

    // ## Movement ##
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private float moveInput;

    // ## Combat Stats ##
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float criticalDamage = 25f;
    [SerializeField] private float criticalChance = 0.1f; // 10% chance
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayers;

    // ## Combat States ##
    private bool isBlocking = false;
    private bool isAttacking = false;
    [SerializeField] private float attackDuration = 0.3f;

    private Animator anim;

    // ## UI Reference ##
    private UIManager_D uiManager;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        uiManager = FindAnyObjectByType<UIManager_D>();
        uiManager.UpdatePlayerHealth(health, 100f);
    }

    void Update()
    {
        isBlocking = Input.GetKey(KeyCode.LeftControl);
        anim.SetBool("IsBlocking", isBlocking);

        if (!isBlocking && !isAttacking)
        {
            moveInput = Input.GetAxisRaw("Horizontal");
        }
        else
        {
            moveInput = 0;
        }
        anim.SetFloat("Speed", Mathf.Abs(moveInput));

        if (Input.GetKeyDown(KeyCode.Space) && !isBlocking && !isAttacking)
        {
            StartCoroutine(AttackCoroutine());
        }
    }

    void FixedUpdate()
    {
        // FIX: Using .linearVelocity for consistency
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    private IEnumerator AttackCoroutine()
    {
        anim.SetTrigger("Punch");
        isAttacking = true;

        Collider2D hitEnemy = Physics2D.OverlapCircle(attackPoint.position, attackRange, enemyLayers);

        float damageToDeal = attackDamage;
        if (Random.value < criticalChance)
        {
            // THE FIX: Was incorrectly assigning criticalChance instead of criticalDamage
            damageToDeal = criticalDamage;
            Debug.Log("Player Landed a Critical HIT!");
        }

        if (hitEnemy != null)
        {
            Debug.Log("We hit " + hitEnemy.name);
            hitEnemy.GetComponent<EnemyAI_D>().TakeDamage(damageToDeal);
        }

        yield return new WaitForSeconds(attackDuration);
        isAttacking = false;
    }

    public void TakeDamage(float damage)
    {
        if (isBlocking)
        {
            Debug.Log("Player blocked the attack!");
            // We could add a block animation trigger here if desired
            return;
        }

        // NEW: Trigger the hurt animation if damage is taken
        anim.SetTrigger("Hurt");
        health -= damage;
        uiManager.UpdatePlayerHealth(health, 100f);
        Debug.Log("Player health:" + health);

        if (health <= 0)
        {
            Debug.Log("Player Defeated!");
            // NEW: Trigger the death animation
            anim.SetTrigger("Defeated");

            // NEW: Disable components to stop interactions after death
            this.enabled = false; // Disables this script (stops player input)
            GetComponent<Collider2D>().enabled = false; // Prevents enemy from bumping into the body
            rb.bodyType = RigidbodyType2D.Kinematic; // Stops physics from affecting the body
            rb.linearVelocity = Vector2.zero; // Immediately stops all movement

            // In a full game, you would call a GameManager here to show a "Game Over" screen
        }
    }


    // Draws a red circle in the Scene view to show the player's attackRange
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}