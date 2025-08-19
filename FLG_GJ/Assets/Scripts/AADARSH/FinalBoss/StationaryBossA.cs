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
            DoFinisherAttack();

            yield return new WaitForSeconds(1f); // short pause for effect

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
        if (player == null) return;

        float minDistanceFromPlayer = 0.5f; // closest to player
        float maxDistanceFromPlayer = 2f;   // farthest from player
        float safeDistanceFromBoss = 2f;    // keep bombs away from boss

        Vector2 spawnPos;
        int safetyCounter = 0;

        do {
            // pick a random angle around the player
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float distance = Random.Range(minDistanceFromPlayer, maxDistanceFromPlayer);

            spawnPos = (Vector2)player.position + new Vector2(
                Mathf.Cos(angle) * distance,
                Mathf.Sin(angle) * distance
            );

            safetyCounter++;
            if (safetyCounter > 20) break; // prevent infinite loop
        }
        while (Vector2.Distance(spawnPos, transform.position) < safeDistanceFromBoss);

        Instantiate(bombPrefab, spawnPos, Quaternion.identity);
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
    void DoFinisherAttack() {
        float healthPercent = (float)currentHealth / maxHealth;

        if (healthPercent > 0.66f) {
            // ?? Easy finisher (radial bullets)
            RadialBurst(20, projectileSpeed);
        } else if (healthPercent > 0.33f) {
            // ? Medium finisher (bomb rain + faster bullets)
            RadialBurst(30, projectileSpeed * 1.5f);
            BombRain(8);
        } else {
            // ?? Hard finisher (lots of bombs + fast bullet hell)
            RadialBurst(35, projectileSpeed * 2f);
            BombRain(14);
        }
    }
    void RadialBurst(int count, float speed) {
        float angleStep = 360f / count;

        for (int i = 0; i < count; i++) {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            GameObject bullet = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.linearVelocity = dir * speed;
        }
    }
    void BombRain(int count, Transform boss) {
        if (boss == null) return;

        float radius = 2f; // distance from boss, tweak as needed

        for (int i = 0; i < count; i++) {
            // Evenly distribute bombs in a circle
            float angle = i * Mathf.PI * 2f / count;

            Vector2 spawnPos = (Vector2)boss.position + new Vector2(
                Mathf.Cos(angle) * radius,
                Mathf.Sin(angle) * radius
            );

            Instantiate(bombPrefab, spawnPos, Quaternion.identity);
        }
    }




}
