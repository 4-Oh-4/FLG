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
    [SerializeField] private Transform firePoint;
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
        transform.position += Vector3.up * forwardSpeed * Time.deltaTime;
        if (transform.position.y > 12f) Destroy(gameObject);

        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        if (rocketPrefab != null && firePoint != null)
        {
            Instantiate(rocketPrefab, firePoint.position, Quaternion.identity);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        if (spawner != null) spawner.OnEnemyDied();
        Destroy(gameObject);
    }
}