using UnityEngine;
using TruckChase;

[RequireComponent(typeof(Rigidbody2D))]
public class BikeAI_D : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float maxHealth = 25f;

    [Header("Movement")]
    [SerializeField] private float forwardSpeed = 4.5f;
    [SerializeField] private float yLimit = 4.5f;

    [Header("Ramming Behavior")]
    [SerializeField] private float timeUntilRam = 8f;
    [SerializeField] private float ramSpeedMultiplier = 2f;
    [SerializeField] private float ramDamage = 25f;

    private float zigZagFrequency;
    private float zigZagMagnitude;
    private float currentHealth;
    private WaveSpawner_D spawner;
    private Transform lorryTarget;
    private Rigidbody2D rb;
    private bool isRamming = false;

    public void Initialize(WaveSpawner_D spawnerRef) { spawner = spawnerRef; }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        lorryTarget = GameObject.Find("Lorry")?.transform;
        zigZagFrequency = Random.Range(3f, 5f);
        zigZagMagnitude = Random.Range(2.5f, 4f);
        Invoke(nameof(ActivateRamMode), timeUntilRam);
    }

    private void FixedUpdate()
    {
        if (isRamming)
        {
            if (lorryTarget != null)
            {
                Vector2 direction = (lorryTarget.position - transform.position).normalized;
                rb.linearVelocity = direction * forwardSpeed;
            }
        }
        else
        {
            float verticalVelocity = (transform.position.y < yLimit) ? forwardSpeed : 0;
            float horizontalVelocity = Mathf.Sin(Time.time * zigZagFrequency) * zigZagMagnitude;
            rb.linearVelocity = new Vector2(horizontalVelocity, verticalVelocity);
        }
    }

    void ActivateRamMode()
    {
        isRamming = true;
        forwardSpeed *= ramSpeedMultiplier;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isRamming && collision.gameObject.TryGetComponent<LorryHealth_D>(out var lorryHealth))
        {
            lorryHealth.TakeDamage(ramDamage);
            Die();
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