using UnityEngine;
using TruckChase;

[RequireComponent(typeof(Rigidbody2D))]
public class BikeAI_D : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float maxHealth = 10f;

    [Header("Movement")]
    [SerializeField] private float forwardSpeed = 6f;
    [Tooltip("The Y-position on screen where the bike stops advancing.")]
    [SerializeField] private float yLimit = 4.5f;

    [Header("Ramming Behavior")]
    [Tooltip("Time in seconds before the bike tries to ram.")]
    [SerializeField] private float timeUntilRam = 8f;
    [SerializeField] private float ramSpeedMultiplier = 2f;
    [SerializeField] private float ramDamage = 25f;

    // Private variables for unique movement and state
    private float zigZagFrequency;
    private float zigZagMagnitude;
    private float currentHealth;
    private WaveSpawner_D spawner;
    private Transform lorryTarget;
    private bool isRamming = false;

    // Called by the spawner when this enemy is created.
    public void Initialize(WaveSpawner_D spawnerRef) { spawner = spawnerRef; }

    private void Start()
    {
        currentHealth = maxHealth;
        lorryTarget = GameObject.Find("Lorry")?.transform;

        // Randomize movement patterns for each bike instance.
        zigZagFrequency = Random.Range(3f, 5f);
        zigZagMagnitude = Random.Range(2.5f, 4f);

        // Start the countdown to activate ramming mode.
        Invoke(nameof(ActivateRamMode), timeUntilRam);
    }

    private void Update()
    {
        if (!isRamming)
        {
            // --- Normal Zig-Zag Behavior ---
            if (transform.position.y < yLimit)
            {
                transform.position += Vector3.up * forwardSpeed * Time.deltaTime;
            }
            transform.position += Vector3.right * Mathf.Sin(Time.time * zigZagFrequency) * zigZagMagnitude * Time.deltaTime;
        }
        else if (lorryTarget != null)
        {
            // --- Ramming Behavior ---
            Vector3 direction = (lorryTarget.position - transform.position).normalized;
            transform.position += direction * forwardSpeed * Time.deltaTime;
        }

        // Enforce horizontal boundaries.
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -8f, 8f); // Assuming xBounds of 8
        transform.position = clampedPosition;
    }

    // Activates the final suicide charge.
    void ActivateRamMode()
    {
        isRamming = true;
        forwardSpeed *= ramSpeedMultiplier;
    }

    // Called on physical collision with another object.
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