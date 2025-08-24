using UnityEngine;
using TruckChase;

[RequireComponent(typeof(Rigidbody2D))]
public class ArmoredVanAI_D : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float maxHealth = 150f;

    [Header("Movement")]
    [SerializeField] private float forwardSpeed = 2f;
    [SerializeField] private float yLimit = 4f;
    [SerializeField] private float xBounds = 8f;

    private float currentHealth;
    private WaveSpawner_D spawner;

    public void Initialize(WaveSpawner_D spawnerRef) { spawner = spawnerRef; }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (transform.position.y < yLimit)
        {
            transform.position += Vector3.up * forwardSpeed * Time.deltaTime;
        }

        // Enforce horizontal boundaries.
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -xBounds, xBounds);
        transform.position = clampedPosition;
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