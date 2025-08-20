using System.Collections;
using System.Collections.Generic; // Required for using Lists
using UnityEngine;

namespace TopDownShooter
{
    public class WaveSpawner_D : MonoBehaviour
    {
        [Header("Setup")]
        [SerializeField] private WaveData_D[] waves;
        [SerializeField] private Transform[] spawnPoints;

        private int enemiesAlive = 0;
        private int currentWaveIndex = 0;

        private void Start()
        {
            StartCoroutine(SpawnLoop());
        }

        private IEnumerator SpawnLoop()
        {
            while (currentWaveIndex < waves.Length)
            {
                WaveData_D currentWave = waves[currentWaveIndex];
                yield return StartCoroutine(SpawnWave(currentWave));

                while (enemiesAlive > 0)
                {
                    yield return null;
                }

                Debug.Log("Wave " + (currentWaveIndex + 1) + " cleared!");

                if (currentWaveIndex < waves.Length - 1)
                {
                    yield return new WaitForSeconds(currentWave.timeUntilNextWave);
                }

                currentWaveIndex++;
            }

            Debug.Log("All waves cleared! You win!");
        }

        // This coroutine has been rewritten to work with the Object Pooler.
        private IEnumerator SpawnWave(WaveData_D wave)
        {
            Debug.Log("Starting Wave " + (currentWaveIndex + 1));

            // 1. Create a single list of all enemies to spawn.
            List<GameObject> enemiesToSpawn = new List<GameObject>();
            foreach (var enemyGroup in wave.enemyGroups)
            {
                for (int i = 0; i < enemyGroup.count; i++)
                {
                    enemiesToSpawn.Add(enemyGroup.enemyPrefab);
                }
            }

            // 2. Shuffle the list for a random spawn order.
            for (int i = 0; i < enemiesToSpawn.Count; i++)
            {
                int randomIndex = Random.Range(i, enemiesToSpawn.Count);
                GameObject temp = enemiesToSpawn[i];
                enemiesToSpawn[i] = enemiesToSpawn[randomIndex];
                enemiesToSpawn[randomIndex] = temp;
            }

            // 3. Spawn the enemies from the shuffled list using the pool.
            foreach (var enemyPrefab in enemiesToSpawn)
            {
                Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

                // AMENDED: This is the correct line. It uses 'enemyPrefab.name' as the tag.
                GameObject enemyInstance = ObjectPooler_D.Instance.SpawnFromPool(enemyPrefab.name, randomSpawnPoint.position, Quaternion.identity);

                if (enemyInstance != null)
                {
                    enemiesAlive++;

                    // Initialize the enemy so it can report its death.
                    if (enemyInstance.GetComponent<EnemyAI_D>() != null)
                    {
                        enemyInstance.GetComponent<EnemyAI_D>().Initialize(this);
                    }
                    else if (enemyInstance.GetComponent<SaboteurAI_D>() != null)
                    {
                        enemyInstance.GetComponent<SaboteurAI_D>().Initialize(this);
                    }
                }

                // Use the spawn interval from the wave data.
                if (wave.enemyGroups.Length > 0)
                {
                    yield return new WaitForSeconds(wave.enemyGroups[0].spawnInterval);
                }
            }
        }

        public void OnEnemyDied()
        {
            enemiesAlive--;
        }
    }
}