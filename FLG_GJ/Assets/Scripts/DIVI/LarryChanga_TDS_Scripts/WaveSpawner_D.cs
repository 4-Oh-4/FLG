using System.Collections;
using System.Collections.Generic;
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
            // Loop through all the wave assets you've assigned.
            while (currentWaveIndex < waves.Length)
            {
                WaveData_D currentWave = waves[currentWaveIndex];
                yield return StartCoroutine(SpawnWave(currentWave));

                // Pause here until all enemies in the current wave are defeated.
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
            // You can add your game-winning logic here.
        }

        private IEnumerator SpawnWave(WaveData_D wave)
        {
            Debug.Log("Starting Wave " + (currentWaveIndex + 1));
            List<GameObject> enemiesToSpawn = new List<GameObject>();
            foreach (var enemyGroup in wave.enemyGroups)
            {

                for (int i = 0; i < enemyGroup.count; i++)
                {
                    enemiesToSpawn.Add(enemyGroup.enemyPrefab);
                }
            }

            // shuffle the List for a random spwan order
            for(int i = 0; i < enemiesToSpawn.Count; i++)
            {
                int randomIndex = Random.Range(i, enemiesToSpawn.Count);
                GameObject temp = enemiesToSpawn[i];
                enemiesToSpawn[i] = enemiesToSpawn[randomIndex];
                enemiesToSpawn[randomIndex] = temp;
            }

            // Spawn the enemies from the shuffled list
            foreach (var enemyPrefab in enemiesToSpawn)
            {
                Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                GameObject enemyInstance = Instantiate(enemyPrefab, randomSpawnPoint.position, Quaternion.identity);

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

                float interval = wave.enemyGroups.Length > 0 ? wave.enemyGroups[0].spawnInterval : 1f;
                yield return new WaitForSeconds(interval);
            }
        }
        

        // This function is called by an enemy right before it dies.
        public void OnEnemyDied()
        {
            enemiesAlive--;
            Debug.Log("Spawner was notified of an enemy death. Enemies left: " + enemiesAlive);
        }
    }
}