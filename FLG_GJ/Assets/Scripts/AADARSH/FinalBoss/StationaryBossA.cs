using UnityEngine;
using System.Collections;
using TMPro;

public class StationaryBossA : MonoBehaviour
{
    [Header("Boss Settings")]
    [SerializeField] private int maxHealth = 10;
    private int currentHealth;
    private bool isVulnerable = false;

    // --- NEW --- Settings for the hit flash effect
    [SerializeField] private Color hitColor = Color.red;
    [SerializeField] private float hitFlashDuration = 0.15f;

    [Header("Wave Settings")]
    [SerializeField] private float waveDuration = 10f;
    [SerializeField] private float vulnerableDuration = 3f;
    private bool isAttacking = false;

    [Header("Projectiles")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 5f;
    [SerializeField] private float projectileInterval = 1f;

    [Header("Bombs")]
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private float bombInterval = 3f;

    [Header("Animation")]
    [Tooltip("The Animator component for the boss.")]
    [SerializeField] private Animator anim;

    [Header("UI")]
    [Tooltip("The UI GameObject that tells the player to attack.")]
    [SerializeField] private GameObject attackIndicatorUI;
    [Tooltip("How fast the UI indicator flashes (in seconds).")]
    [SerializeField] private float flashInterval = 0.25f;

    private TextMeshProUGUI attackIndicatorText;
    private Coroutine flashingCoroutine;

    // --- NEW --- Variables to handle the sprite and colors
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Coroutine hitFlashCoroutine;

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentHealth = maxHealth;

        // --- NEW --- Get the SpriteRenderer and store its original color
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        else
        {
            Debug.LogError("No SpriteRenderer found on the boss GameObject!");
        }

        if (anim == null)
        {
            anim = GetComponent<Animator>();
        }

        if (attackIndicatorUI != null)
        {
            attackIndicatorText = attackIndicatorUI.GetComponent<TextMeshProUGUI>();
            if (attackIndicatorText == null)
            {
                Debug.LogError("Attack Indicator UI does not have a TextMeshProUGUI component!");
            }
            attackIndicatorUI.SetActive(false);
        }

        StartCoroutine(BossRoutine());
    }

    void Update()
    {
        // No changes in Update()
        if (player != null && isAttacking)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                anim.SetFloat("AimX", Mathf.Sign(direction.x));
                anim.SetFloat("AimY", 0);
            }
            else
            {
                anim.SetFloat("AimX", 0);
                anim.SetFloat("AimY", Mathf.Sign(direction.y));
            }
        }
        else if (!isAttacking)
        {
            anim.SetFloat("AimX", 0);
            anim.SetFloat("AimY", 0);
        }
    }

    // --- MODIFIED --- This function now triggers the hit flash effect
    public void TakeDamage(int dmg)
    {
        if (!isVulnerable || currentHealth <= 0) return;

        currentHealth -= dmg;
        Debug.Log("Boss HP: " + currentHealth);

        // --- NEW --- Stop any previous hit flash and start a new one
        if (spriteRenderer != null)
        {
            if (hitFlashCoroutine != null)
            {
                StopCoroutine(hitFlashCoroutine);
            }
            hitFlashCoroutine = StartCoroutine(HitFlash());
        }


        if (currentHealth <= 0)
        {
            Debug.Log("Boss defeated!");
            if (attackIndicatorUI != null)
            {
                attackIndicatorUI.SetActive(false);
            }
            StopAllCoroutines();
            Destroy(gameObject);
        }
    }

    // --- NEW --- Coroutine to handle the hit flash visual effect
    private IEnumerator HitFlash()
    {
        // Change color to the hit color
        spriteRenderer.color = hitColor;

        // Wait for the specified duration
        yield return new WaitForSeconds(hitFlashDuration);

        // Revert to the original color
        spriteRenderer.color = originalColor;

        // Set the coroutine reference to null once it's finished
        hitFlashCoroutine = null;
    }


    private IEnumerator BossRoutine()
    {
        while (currentHealth > 0)
        {
            // Attack phase
            isVulnerable = false;
            if (flashingCoroutine != null)
            {
                StopCoroutine(flashingCoroutine);
                flashingCoroutine = null;
            }
            if (attackIndicatorUI != null)
            {
                attackIndicatorUI.SetActive(false);
            }

            isAttacking = true;
            anim.SetBool("IsShooting", true);
            InvokeRepeating(nameof(FireProjectiles), 0f, projectileInterval);
            InvokeRepeating(nameof(SpawnBomb), 1f, bombInterval);

            yield return new WaitForSeconds(waveDuration);

            CancelInvoke(nameof(FireProjectiles));
            CancelInvoke(nameof(SpawnBomb));
            isAttacking = false;
            anim.SetBool("IsShooting", false);
            DoFinisherAttack();

            yield return new WaitForSeconds(1f);

            // Vulnerable phase
            isVulnerable = true;
            if (attackIndicatorUI != null)
            {
                attackIndicatorUI.SetActive(true);
                flashingCoroutine = StartCoroutine(FlashIndicator());
            }

            Debug.Log("Boss is vulnerable!");
            yield return new WaitForSeconds(vulnerableDuration);
        }
    }

    private IEnumerator FlashIndicator()
    {
        while (true)
        {
            attackIndicatorText.enabled = !attackIndicatorText.enabled;
            yield return new WaitForSeconds(flashInterval);
        }
    }

    #region Attack Methods
    // No changes to any of the attack methods
    void FireProjectiles()
    {
        if (player == null) return;
        Vector2 dir = (player.position - transform.position).normalized;
        GameObject bullet = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.linearVelocity = dir * projectileSpeed;
    }

    void SpawnBomb()
    {
        if (player == null) return;
        float minDistanceFromPlayer = 0.5f;
        float maxDistanceFromPlayer = 2f;
        float safeDistanceFromBoss = 2f;
        Vector2 spawnPos;
        int safetyCounter = 0;
        do
        {
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float distance = Random.Range(minDistanceFromPlayer, maxDistanceFromPlayer);
            spawnPos = (Vector2)player.position + new Vector2(Mathf.Cos(angle) * distance, Mathf.Sin(angle) * distance);
            safetyCounter++;
            if (safetyCounter > 20) break;
        } while (Vector2.Distance(spawnPos, transform.position) < safeDistanceFromBoss);
        Instantiate(bombPrefab, spawnPos, Quaternion.identity);
    }

    void DoFinisherAttack()
    {
        float healthPercent = (float)currentHealth / maxHealth;
        if (healthPercent > 0.66f)
        {
            RadialBurst(20, projectileSpeed);
        }
        else if (healthPercent > 0.33f)
        {
            RadialBurst(30, projectileSpeed * 1.5f);
            BombRain(5, transform);
        }
        else
        {
            RadialBurst(35, projectileSpeed * 2f);
            BombRain(8, transform);
        }
    }

    void RadialBurst(int count, float speed)
    {
        float angleStep = 360f / count;
        for (int i = 0; i < count; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            GameObject bullet = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.linearVelocity = dir * speed;
        }
    }

    void BombRain(int count, Transform boss)
    {
        if (boss == null) return;
        float radius = 3.5f;
        for (int i = 0; i < count; i++)
        {
            float angle = i * Mathf.PI * 2f / count;
            Vector2 spawnPos = (Vector2)boss.position + new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
            Instantiate(bombPrefab, spawnPos, Quaternion.identity);
        }
    }
    #endregion
}