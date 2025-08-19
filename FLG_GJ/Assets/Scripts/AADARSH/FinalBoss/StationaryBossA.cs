using UnityEngine;

public class StationaryBossA : MonoBehaviour {
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
        InvokeRepeating(nameof(FireProjectiles), 1f, projectileInterval);
        InvokeRepeating(nameof(SpawnBomb), 2f, bombInterval);
    }

    void FireProjectiles() {
        // Example: Fire towards player
        if (player == null) return;

        Vector2 dir = (player.position - transform.position).normalized;
        GameObject bullet = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.linearVelocity = dir * projectileSpeed;
    }

    void SpawnBomb() {
        // Example: Spawn at random spot in arena
        Vector2 pos = new Vector2(
            Random.Range(-arenaSize.x / 2, arenaSize.x / 2),
            Random.Range(-arenaSize.y / 2, arenaSize.y / 2)
        );
        Instantiate(bombPrefab, pos, Quaternion.identity);
    }
}
