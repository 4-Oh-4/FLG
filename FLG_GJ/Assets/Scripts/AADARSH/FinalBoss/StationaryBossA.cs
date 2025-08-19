using UnityEngine;

public class StationaryBossA : MonoBehaviour {
    [Header("Boss Settings")]
    [SerializeField] private int maxHealth = 10;
    private int currentHealth;
    private bool isVulnerable = false;

    [Header("Wave Settings")]
    [SerializeField] private float waveDuration = 10f;      // attack duration
    [SerializeField] private float vulnerableDuration = 3f; // time player can attack
    private bool isAttacking = false;

    [Header("Projectiles")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 5f;
    [SerializeField] private float projectileInterval = 1f;

    [Header("Bombs")]
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private float bombInterval = 3f;
    [SerializeField] private Vector2 arenaSize = new Vector2(10, 10);

    private Transform player;

    void Start() {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentHealth = maxHealth;
        StartCoroutine(BossRoutine());
    }

    private System.Collections.IEnumerator BossRoutine() {
        while (currentHealth > 0) {
            // ?? Attack phase
            isVulnerable = false;
            gameObject.GetComponent<Renderer>().material.color = Color.red;

            isAttacking = true;
            InvokeRepeating(nameof(FireProjectiles), 0f, projectileInterval);
            InvokeRepeating(nameof(SpawnBomb), 1f, bombInterval);

            yield return new WaitForSeconds(waveDuration);

            // stop attacks
            CancelInvoke(nameof(FireProjectiles));
            CancelInvoke(nameof(SpawnBomb));
            isAttacking = false;

            // ?? Vulnerable phase
            isVulnerable = true;
            gameObject.GetComponent<Renderer>().material.color = Color.green;
            Debug.Log("Boss is vulnerable!");
            yield return new WaitForSeconds(vulnerableDuration);

            // repeat loop
        }

        Debug.Log("Boss defeated!");
        // add death animation/effects here
        Destroy(gameObject);
    }

    void FireProjectiles() {
        if (player == null) return;
        Vector2 dir = (player.position - transform.position).normalized;
        GameObject bullet = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.linearVelocity = dir * projectileSpeed;
    }

    void SpawnBomb() {
        Vector2 pos = new Vector2(
            Random.Range(-arenaSize.x / 2, arenaSize.x / 2),
            Random.Range(-arenaSize.y / 2, arenaSize.y / 2)
        );
        Instantiate(bombPrefab, pos, Quaternion.identity);
    }

    public void TakeDamage(int dmg) {
        if (!isVulnerable) return; // can only damage during vulnerable phase

        currentHealth -= dmg;
        Debug.Log("Boss HP: " + currentHealth);

        if (currentHealth <= 0) {
            StopAllCoroutines();
            Debug.Log("Boss defeated!");
            Destroy(gameObject);
        }
        StopAllCoroutines();
        StartCoroutine(BossRoutine());
    }
}
