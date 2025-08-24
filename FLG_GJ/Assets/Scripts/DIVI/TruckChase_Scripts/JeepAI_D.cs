using UnityEngine;
using TruckChase;

[RequireComponent(typeof(Rigidbody2D))]
public class JeepAI_D : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float maxHealth = 60f;
    [SerializeField] private float forwardSpeed = 3f;

    [Header("Combat")]
    [SerializeField] private GameObject rocketPrefab;
    // AMENDED: Removed the firePoint variable.
    [SerializeField] private float fireRate = 2f;

    private float currentHealth;
    private float nextFireTime = 0f;
    private WaveSpawner_D spawner;

    public void Initialize(WaveSpawner_D spawnerRef)
    {
        spawner = spawnerRef;
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        transform.position += Vector3.up * forwardSpeed * Time.deltaTime;
        if (transform.position.y > 12f) Die();

        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        // AMENDED: Now instantiates the rocket from this object's center position.
        if (rocketPrefab != null)
        {
            Instantiate(rocketPrefab, transform.position, Quaternion.identity);
        }
    }

    public void TakeDamage(float damage)
    {
        if (currentHealth <= 0) return;
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (!enabled) return;

        if (spawner != null)
        {
            spawner.OnEnemyDied();
        }

        enabled = false;
        Destroy(gameObject);
    }
}