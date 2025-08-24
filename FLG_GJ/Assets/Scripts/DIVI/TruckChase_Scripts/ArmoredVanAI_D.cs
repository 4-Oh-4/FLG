using UnityEngine;
using TruckChase;

[RequireComponent(typeof(Rigidbody2D))]
public class ArmoredVanAI_D : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float maxHealth = 150f;
    [SerializeField] private float forwardSpeed = 2f;

    private float currentHealth;
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