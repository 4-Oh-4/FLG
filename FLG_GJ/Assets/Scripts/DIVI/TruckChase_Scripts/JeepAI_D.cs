using UnityEngine;
using TruckChase;

[RequireComponent(typeof(Rigidbody2D))]
public class JeepAI_D : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float maxHealth = 100f;
    [Header("Movement")]
    [SerializeField] private float forwardSpeed = 2.5f;
    [SerializeField] private float yLimit = 5f;
    [Header("Combat")]
    [SerializeField] private GameObject rocketPrefab;
    [SerializeField] private float fireRate = 2.5f;

    private float currentHealth, nextFireTime = 0f;
    private WaveSpawner_D spawner;
    private Rigidbody2D rb;

    public void Initialize(WaveSpawner_D spawnerRef) { spawner = spawnerRef; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        currentHealth = maxHealth;
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = transform.position.y < yLimit ? new Vector2(0, forwardSpeed) : Vector2.zero;
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
            ObjectPooler_D.Instance.SpawnFromPool("Rocket_D", transform.position, Quaternion.identity);
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