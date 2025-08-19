using System.Collections;
using UnityEngine;

namespace TopDownShooter
{
    public class SimpleSpawner_D : MonoBehaviour
    {
        [Header("Spawning Setup")]
        [Tooltip("A list of all possible enemies that can be spawned.")]
        [SerializeField] private GameObject[] enemyPrefabs;
        [SerializeField] private Transform[] spawnPoints;

        [Header("Wave Settings")]
        [SerializeField] private int enemiesPerWave = 5;
        [SerializeField] private float timeBetweenWaves = 5f;
        [SerializeField] private float timeBetweenSpawns = 1f;

        private int enemiesAlive;
        private int currentWaveNumber = 1;

        private void Start()
        {
            StartCoroutine(SpawnLoop());
        }

        private IEnumerator SpawnLoop()
        {
            while (true) // This will loop forever for endless waves.
            {
                yield return StartCoroutine(SpawnWave());

                // Wait until all enemies from the wave are defeated.
                while (enemiesAlive > 0)
                {
                    yield return null;
                }

                Debug.Log("Wave " + currentWaveNumber + " cleared!");
                yield return new WaitForSeconds(timeBetweenWaves);

                currentWaveNumber++;
                // Increase difficulty for the next wave.
                enemiesPerWave += 3;
            }
        }

        private IEnumerator SpawnWave()
        {
            enemiesAlive = enemiesPerWave;
            Debug.Log("Starting Wave " + currentWaveNumber + ". Spawning " + enemiesAlive + " enemies.");

            for (int i = 0; i < enemiesAlive; i++)
            {
                // Pick a random enemy and a random spawn point.
                GameObject randomEnemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
                Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

                // Create the enemy.
                GameObject enemyInstance = Instantiate(randomEnemyPrefab, randomSpawnPoint.position, Quaternion.identity);

                // Give the new enemy a reference to this spawner.
                // Note: You may need to get a different component depending on the enemy type.
                if (enemyInstance.GetComponent<EnemyAI_D>() != null)
                {
                    enemyInstance.GetComponent<EnemyAI_D>().Initialize(this);
                }
                else if (enemyInstance.GetComponent<SaboteurAI_D>() != null)
                {
                    // You would need to add an Initialize function to SaboteurAI_D as well.
                    // enemyInstance.GetComponent<SaboteurAI_D>().Initialize(this);
                }

                yield return new WaitForSeconds(timeBetweenSpawns);
            }
        }

        // This function is called by an enemy right before it is destroyed.
        public void EnemyDied()
        {
            enemiesAlive--;
        }
    }
}