using System.Collections;
using System.Collections.Generic; // Required for using Lists
using UnityEngine;

namespace TruckChase
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

        // The main coroutine that manages the progression from one wave to the next.
        private IEnumerator SpawnLoop()
        {
            while (currentWaveIndex < waves.Length)
            {
                WaveData_D currentWave = waves[currentWaveIndex];
                yield return StartCoroutine(SpawnWave(currentWave));

                // Pause here until all enemies from the current wave are defeated.
                while (enemiesAlive > 0)
                {
                    yield return null;
                }

                Debug.Log("Wave " + (currentWaveIndex + 1) + " cleared!");

                // If this is not the final wave, wait before starting the next one.
                if (currentWaveIndex < waves.Length - 1)
                {
                    yield return new WaitForSeconds(currentWave.timeUntilNextWave);
                }

                currentWaveIndex++;
            }

            Debug.Log("Final wave cleared! Minigame ends!");
            // You can add your game-winning logic here, like calling the GameManager.
        }

        // The coroutine that handles spawning all enemies for a single wave.
        private IEnumerator SpawnWave(WaveData_D wave)
        {
            Debug.Log("Starting Wave " + (currentWaveIndex + 1));

            // 1. Create a single list of all enemies to spawn for this wave.
            List<GameObject> enemiesToSpawn = new List<GameObject>();
            foreach (var enemyGroup in wave.enemyGroups)
            {
                for (int i = 0; i < enemyGroup.count; i++)
                {
                    enemiesToSpawn.Add(enemyGroup.enemyPrefab);
                }
            }
            enemiesAlive = enemiesToSpawn.Count;

            // 2. Shuffle the list for a random spawn order.
            for (int i = 0; i < enemiesToSpawn.Count; i++)
            {
                int randomIndex = Random.Range(i, enemiesToSpawn.Count);
                GameObject temp = enemiesToSpawn[i];
                enemiesToSpawn[i] = enemiesToSpawn[randomIndex];
                enemiesToSpawn[randomIndex] = temp;
            }

            // 3. Spawn the enemies one by one from the shuffled list.
            foreach (var enemyPrefab in enemiesToSpawn)
            {
                Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                GameObject enemyInstance = Instantiate(enemyPrefab, randomSpawnPoint.position, Quaternion.identity);

                // Initialize the enemy so it can report its death to this spawner.
                if (enemyInstance.TryGetComponent<BikeAI_D>(out var bike)) bike.Initialize(this);
                else if (enemyInstance.TryGetComponent<SedanAI_D>(out var sedan)) sedan.Initialize(this);
                else if (enemyInstance.TryGetComponent<JeepAI_D>(out var jeep)) jeep.Initialize(this);
                else if (enemyInstance.TryGetComponent<ArmoredVanAI_D>(out var van)) van.Initialize(this);

                // Wait a short time before spawning the next enemy.
                yield return new WaitForSeconds(0.5f); // Using a fixed 0.5s interval for simplicity.
            }
        }

        // This function is called by an enemy right before it dies.
        public void OnEnemyDied()
        {
            enemiesAlive--;
        }
    }
}