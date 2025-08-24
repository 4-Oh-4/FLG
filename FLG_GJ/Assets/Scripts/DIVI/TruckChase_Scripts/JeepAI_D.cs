using UnityEngine;
using TruckChase;

[RequireComponent(typeof(Rigidbody2D))]
public class JeepAI_D : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float maxHealth = 60f;

    [Header("Movement")]
    [SerializeField] private float forwardSpeed = 3f;
    [SerializeField] private float yLimit = 5f;
    [SerializeField] private float xBounds = 8f;

    [Header("Combat")]
    [SerializeField] private GameObject rocketPrefab;
    [SerializeField] private float fireRate = 2.5f;

    private float currentHealth;
    private float nextFireTime = 0f;
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

        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }

        // Enforce horizontal boundaries.
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -xBounds, xBounds);
        transform.position = clampedPosition;
    }

    void Shoot()
    {
        if (rocketPrefab != null)
        {
            Instantiate(rocketPrefab, transform.position, Quaternion.identity);
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