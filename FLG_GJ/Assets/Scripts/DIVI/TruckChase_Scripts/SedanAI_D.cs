using System.Collections;
using UnityEngine;
using TruckChase;

[RequireComponent(typeof(Rigidbody2D))]
public class SedanAI_D : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float maxHealth = 75f;
    [Header("Movement")]
    [SerializeField] private float approachSpeed = 3.0f;
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
    private float driftFrequency, driftMagnitude;

    public void Initialize(WaveSpawner_D spawnerRef) { spawner = spawnerRef; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        currentHealth = maxHealth;
        isAttacking = false;
        lorryTarget = GameObject.Find("Lorry")?.transform;
        driftFrequency = Random.Range(0.8f, 1.5f);
        driftMagnitude = Random.Range(1f, 2f);
    }

    private void FixedUpdate()
    {
        if (lorryTarget == null || isAttacking) return;

        if (Vector3.Distance(transform.position, lorryTarget.position) <= attackDistance)
        {
            StartCoroutine(AttackCycle());
        }
        else if (transform.position.y < yLimit)
        {
            Vector2 velocity = new Vector2(0, approachSpeed);
            velocity.x = Mathf.Sin(Time.time * driftFrequency) * driftMagnitude;
            rb.linearVelocity = velocity;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private IEnumerator AttackCycle()
    {
        isAttacking = true;

        Vector2 ramDirection = (lorryTarget.position - transform.position).normalized;
        rb.linearVelocity = ramDirection * ramSpeed;
        yield return new WaitForSeconds(0.5f);

        rb.linearVelocity = Vector2.down * retreatSpeed;
        yield return new WaitForSeconds(1.2f);

        rb.linearVelocity = Vector2.zero;
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
        if (spawner != null) spawner.OnEnemyDied();
        gameObject.SetActive(false);
    }
}