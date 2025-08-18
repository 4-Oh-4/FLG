using System.Collections;
using UnityEngine;

public class BottleSpawnerA : MonoBehaviour {
    [SerializeField] private GameObject bottle;
    [SerializeField] private float minForce = 2f;
    [SerializeField] private float maxForce = 3f;
    [SerializeField] private float minAngle = -15f;
    [SerializeField] private float maxAngle = 15f;
    [SerializeField] private float waveDuration = 2f;   // total time for each wave
    [SerializeField] private int totalWaves = 5;        // number of waves
    private Collider2D spawnArea;

    void Start() {
        spawnArea = GetComponent<Collider2D>();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
            StopAllCoroutines();
            StartCoroutine(SpawnWaves());
        }
    }
    
    private IEnumerator SpawnWaves() {
        for (int wave = 1; wave <= totalWaves; wave++) {
            int bottleCount = 2 * wave + 1; // 1st wave = 3, 2nd = 5, etc.
            float delayBetweenSpawns = waveDuration / bottleCount;

            for (int i = 0; i < bottleCount; i++) {
                SpawnBottle();
                yield return new WaitForSeconds(delayBetweenSpawns);
            }

            // optional pause between waves
            yield return new WaitForSeconds(5f);
        }
    }

    private void SpawnBottle() {
        Vector3 pos = new Vector3(
            Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x),
            Random.Range(spawnArea.bounds.min.y, spawnArea.bounds.max.y),
            0f
        );

        Quaternion rotation = Quaternion.Euler(0f, 0f, Random.Range(minAngle, maxAngle));
        GameObject prefab = Instantiate(bottle, pos, rotation);

        float force = Random.Range(minForce, maxForce);
        prefab.GetComponent<Rigidbody2D>().AddForce(prefab.transform.up * force, ForceMode2D.Impulse);
    }
}
