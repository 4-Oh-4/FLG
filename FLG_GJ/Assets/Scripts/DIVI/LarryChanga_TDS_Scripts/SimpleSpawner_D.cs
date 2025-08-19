using System.Collections;
using UnityEngine;

namespace TopDownShooter
{
    // A simple class to define a group of enemies in a wave.
    [System.Serializable]
    public class EnemyGroup
    {
        public GameObject enemyPrefab;
        public int count;
    }

    // A simple class to define a whole wave.
    [System.Serializable]
    public class Wave
    {
        public EnemyGroup[] enemiesInWave;
        public float timeBetweenSpawns = 0.5f; // Time between each enemy in this wave.
        public float timeUntilNextWave = 5f;   // Time after this wave is cleared.
    }

    public class SimpleSpawner_D : MonoBehaviour
    {
        [Header("Spawning Setup")]
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private Wave[] waves;

        private int enemiesAlive = 0;
        private int currentWaveIndex = 0;

        // Starts the spawning process when the game begins.
        private void Start()
        {
            StartCoroutine(SpawnLoop());
        }

        // The main loop that handles wave progression.
        private IEnumerator SpawnLoop()
        {
            // Continue as long as there are waves left to spawn.
            while (currentWaveIndex < waves.Length)
            {
                Wave currentWave = waves[currentWaveIndex];

                // Spawn all enemies for the current wave.
                yield return StartCoroutine(SpawnWave(currentWave));

                // Wait until all spawned enemies are defeated.
                while (enemiesAlive > 0)
                {
                    yield return null; // Wait for the next frame.
                }

                Debug.Log("Wave " + (currentWaveIndex + 1) + " cleared!");
                yield return new WaitForSeconds(currentWave.timeUntilNextWave);

                currentWaveIndex++;
            }

            Debug.Log("All waves cleared! You win!");
            // Here you would add your game-winning logic.
        }

        // Spawns all enemies for a single wave.
        private IEnumerator SpawnWave(Wave wave)
        {
            foreach (var enemyGroup in wave.enemiesInWave)
            {
                for (int i = 0; i < enemyGroup.count; i++)
                {
                    // Pick a random spawn point.
                    Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

                    // Create the enemy.
                    GameObject enemyInstance = Instantiate(enemyGroup.enemyPrefab, randomSpawnPoint.position, Quaternion.identity);

                    // Get the enemy's health script and tell it to notify us when it dies.
                    Health_D enemyHealth = enemyInstance.GetComponent<Health_D>();
                    if (enemyHealth != null)
                    {
                        enemiesAlive++;
                        enemyHealth.onDeath.AddListener(() => OnEnemyDied());
                    }

                    // Wait a short time before spawning the next enemy in the wave.
                    yield return new WaitForSeconds(wave.timeBetweenSpawns);
                }
            }
        }

        // This function is called by the onDeath event from an enemy's Health_D script.
        void OnEnemyDied()
        {
            enemiesAlive--;
        }
    }
}