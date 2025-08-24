using UnityEngine;
using TruckChase;

[RequireComponent(typeof(Rigidbody2D))]
public class ArmoredVanAI_D : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float maxHealth = 250f;

    [Header("Movement")]
    [SerializeField] private float forwardSpeed = 1.5f;
    [SerializeField] private float yLimit = 4f;

    private float currentHealth;
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