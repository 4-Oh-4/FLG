using System.Collections;
using UnityEngine;

public class BottleSpawnerA : MonoBehaviour {
    [SerializeField] private GameObject bottle;
    [SerializeField] private float minForce = 2f;
    [SerializeField] private float maxForce = 3f;
    [SerializeField] private float minAngle = -15f;
    [SerializeField] private float maxAngle = 15f;
    [SerializeField] private float waveDuration = 2f;   // total time per wave
    [SerializeField] private int totalWaves = 5;        // maximum waves allowed

    private Collider2D spawnArea;
    private int currentWave = 0; // tracks which wave we are on

    void Start() {
        spawnArea = GetComponent<Collider2D>();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
            if (currentWave < totalWaves) {
                currentWave++;
                StartCoroutine(SpawnWave(currentWave));
            }
        }
    }

    private IEnumerator SpawnWave(int waveNumber) {
        int bottleCount = 2 * waveNumber + 1; // Wave 1=3, Wave 2=5, etc.
        float delayBetweenSpawns = waveDuration / bottleCount;

        for (int i = 0; i < bottleCount; i++) {
            SpawnBottle();
            yield return new WaitForSeconds(delayBetweenSpawns);
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
