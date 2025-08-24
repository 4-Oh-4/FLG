using UnityEngine;
using TruckChase;

[RequireComponent(typeof(Rigidbody2D))]
public class ArmoredVanAI_D : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float maxHealth = 300f;
    [Header("Movement")]
    [SerializeField] private float forwardSpeed = 1.2f;
    [SerializeField] private float yLimit = 4f;

    private float currentHealth;
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
        rb.velocity = transform.position.y < yLimit ? new Vector2(0, forwardSpeed) : Vector2.zero;
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