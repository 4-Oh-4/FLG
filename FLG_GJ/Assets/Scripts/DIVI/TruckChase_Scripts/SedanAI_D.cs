using System.Collections;
using UnityEngine;
using TruckChase;

[RequireComponent(typeof(Rigidbody2D))]
public class SedanAI_D : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float maxHealth = 30f;

    [Header("Movement")]
    [SerializeField] private float approachSpeed = 4f;
    [SerializeField] private float ramSpeed = 10f;
    [SerializeField] private float retreatSpeed = 5f;
    [Tooltip("The Y-position on screen where the enemy stops advancing.")]
    [SerializeField] private float yLimit = 4.0f;
    [Tooltip("The horizontal limit for movement.")]
    [SerializeField] private float xBounds = 8f;

    [Header("Attack Behavior")]
    [SerializeField] private float attackDistance = 5f;
    [SerializeField] private float retreatDistance = 5f;
    [SerializeField] private float postRamWaitTime = 4f;
    [SerializeField] private float ramDamage = 15f;

    private float currentHealth;
    private WaveSpawner_D spawner;
    private Transform lorryTarget;
    private bool isAttacking = false;

    public void Initialize(WaveSpawner_D spawnerRef) { spawner = spawnerRef; }

    private void Start()
    {
        currentHealth = maxHealth;
        lorryTarget = GameObject.Find("Lorry")?.transform;
    }

    private void Update()
    {
        if (lorryTarget == null) return;

        if (!isAttacking)
        {
            if (transform.position.y < yLimit)
            {
                transform.position += Vector3.up * approachSpeed * Time.deltaTime;
            }

            if (Vector3.Distance(transform.position, lorryTarget.position) <= attackDistance)
            {
                StartCoroutine(AttackCycle());
            }
        }

        // Enforce horizontal boundaries.
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -xBounds, xBounds);
        transform.position = clampedPosition;
    }

    private IEnumerator AttackCycle()
    {
        isAttacking = true;

        // --- Ram Phase ---
        Vector3 ramDirection = (lorryTarget.position - transform.position).normalized;
        float ramDuration = 0.5f;
        float timer = 0;
        while (timer < ramDuration)
        {
            transform.position += ramDirection * ramSpeed * Time.deltaTime;
            timer += Time.deltaTime;
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