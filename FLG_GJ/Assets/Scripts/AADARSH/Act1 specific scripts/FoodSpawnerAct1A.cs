using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FoodSpawnerAct1A : MonoBehaviour {
    [Header("Spawning Configuration")]
    [SerializeField] private GameObject foodPrefab;
    [SerializeField] private float spawnInterval = 20f;

    private Collider2D spawnAreaCollider;

    // --- ADDED ---
    // A variable to keep track of the running coroutine so we can stop it.
    private Coroutine spawningCoroutine;
    private bool isSpawning = false;

    void Awake() {
        spawnAreaCollider = GetComponent<Collider2D>();
    }

    // --- CHANGED ---
    // Start() is no longer needed to begin the coroutine, but we'll keep it for the safety check.
    void Start() {
        if (foodPrefab == null) {
            Debug.LogError("Food Prefab is not assigned in the FoodSpawner script!", this.gameObject);
        }
    }

    // --- ADDED: Public Control Methods ---

    /// <summary>
    /// Starts the spawning process. Called by the GameManager.
    /// </summary>
    public void StartSpawning() {
        // Only start if it's not already spawning to prevent multiple loops.
        if (!isSpawning) {
            Debug.Log("Starting food spawning...");
            isSpawning = true;
            // Start the coroutine and store a reference to it.
            spawningCoroutine = StartCoroutine(SpawnFoodRoutine());
        }
    }

    /// <summary>
    /// Stops the spawning process. Called by the GameManager.
    /// </summary>
    public void StopSpawning() {
        if (isSpawning) {
            Debug.Log("Stopping food spawning...");
            isSpawning = false;
            // If the coroutine is running, stop it.
            if (spawningCoroutine != null) {
                StopCoroutine(spawningCoroutine);
            }
        }
    }

    // --- (The rest of the script is the same) ---

    private IEnumerator SpawnFoodRoutine() {
        while (true) {
            yield return new WaitForSeconds(spawnInterval);
            Vector2 spawnPosition = GetRandomPointInCollider();
            Instantiate(foodPrefab, spawnPosition, Quaternion.identity);
        }
    }

    private Vector2 GetRandomPointInCollider() {
        Bounds colliderBounds = spawnAreaCollider.bounds;
        int safetyNet = 0;
        while (safetyNet < 100) {
            float randomX = Random.Range(colliderBounds.min.x, colliderBounds.max.x);
            float randomY = Random.Range(colliderBounds.min.y, colliderBounds.max.y);
            Vector2 randomPoint = new Vector2(randomX, randomY);

            if (spawnAreaCollider.OverlapPoint(randomPoint)) {
                return randomPoint;
            }
            safetyNet++;
        }
        Debug.LogWarning("Could not find a point within the collider after 100 attempts. Spawning at the center.", this.gameObject);
        return colliderBounds.center;
    }
}