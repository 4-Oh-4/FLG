using UnityEngine;
using TruckChase;

[RequireComponent(typeof(Rigidbody2D))]
public class JeepAI_D : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float maxHealth = 80f;

    [Header("Movement")]
    [SerializeField] private float forwardSpeed = 2.8f;
    [SerializeField] private float yLimit = 5f;

    [Header("Combat")]
    [SerializeField] private GameObject rocketPrefab;
    [SerializeField] private float fireRate = 2.5f;

    private float currentHealth;
    private float nextFireTime = 0f;
    private WaveSpawner_D spawner;
    private Rigidbody2D rb;

    public void Initialize(WaveSpawner_D spawnerRef) { spawner = spawnerRef; }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    private void FixedUpdate()
    {
        if (transform.position.y < yLimit)
        {
            rb.linearVelocity = new Vector2(0, forwardSpeed);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void Update()
    {
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
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