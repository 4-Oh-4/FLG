using System.Collections;
using UnityEngine;
using TruckChase;

[RequireComponent(typeof(Rigidbody2D))]
public class SedanAI_D : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float maxHealth = 100f;

    [Header("Movement")]
    [SerializeField] private float approachSpeed = 2.5f;
    [SerializeField] private float ramSpeed = 10f;
    [SerializeField] private float retreatSpeed = 5f;
    [SerializeField] private float yLimit = 4.0f;

    [Header("Attack Behavior")]
    [SerializeField] private float attackDistance = 5f;
    [SerializeField] private float retreatDistance = 5f;
    [SerializeField] private float postRamWaitTime = 4f;
    [SerializeField] private float ramDamage = 15f;

    private float currentHealth;
    private WaveSpawner_D spawner;
    private Transform lorryTarget;
    private Rigidbody2D rb;
    private bool isAttacking = false;

    public void Initialize(WaveSpawner_D spawnerRef) { spawner = spawnerRef; }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        lorryTarget = GameObject.Find("Lorry")?.transform;
    }

    private void FixedUpdate()
    {
        if (lorryTarget == null || isAttacking) return;

        // --- Approach Logic ---
        // AMENDED: The Sedan now tries to align horizontally with the lorry.
        Vector2 targetPosition = new Vector2(lorryTarget.position.x, transform.position.y);
        Vector2 newPosition = Vector2.MoveTowards(transform.position, targetPosition, approachSpeed * Time.fixedDeltaTime);

        // Only move upwards if below the Y-limit.
        if (transform.position.y < yLimit)
        {
            newPosition.y += approachSpeed * Time.fixedDeltaTime;
        }

        rb.MovePosition(newPosition);

        // Check if we are close enough to start the attack cycle.
        if (Vector3.Distance(transform.position, lorryTarget.position) <= attackDistance)
        {
            StartCoroutine(AttackCycle());
        }
    }

    private IEnumerator AttackCycle()
    {
        isAttacking = true;

        // --- Ram Phase ---
        // AMENDED: Lock on to the lorry's position at the start of the ram.
        Vector3 ramTargetPosition = lorryTarget.position;
        while (Vector3.Distance(transform.position, ramTargetPosition) > 0.1f)
        {
            // Charge in a straight line towards the locked position.
            transform.position = Vector3.MoveTowards(transform.position, ramTargetPosition, ramSpeed * Time.deltaTime);
            yield return null;
        }

        // --- Retreat Phase ---
        Vector3 retreatTargetPos = transform.position + Vector3.down * retreatDistance;
        while (Vector3.Distance(transform.position, retreatTargetPos) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, retreatTargetPos, retreatSpeed * Time.deltaTime);
            yield return null;
        }

        // --- Wait Phase ---
        yield return new WaitForSeconds(postRamWaitTime);

        isAttacking = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<LorryHealth_D>(out var lorryHealth))
        {
            lorryHealth.TakeDamage(ramDamage);
        }
    }

    public void TakeDamage(float damage)
    {
        if (currentHealth <= 0) return;
        currentHealth -= damage;
        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        if (!enabled) return;
        if (spawner != null) spawner.OnEnemyDied();
        enabled = false;
        Destroy(gameObject);
    }
}