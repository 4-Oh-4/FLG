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
            while (currentWaveIndex < waves.Length)
            {
                WaveData_D currentWave = waves[currentWaveIndex];
                yield return StartCoroutine(SpawnWave(currentWave));

                while (enemiesAlive > 0)
                {
                    yield return null;
                }

                if (currentWaveIndex < waves.Length - 1)
                {
                    yield return new WaitForSeconds(currentWave.timeUntilNextWave);
                }
                currentWaveIndex++;
            }
        }

        private IEnumerator SpawnWave(WaveData_D wave)
        {
            List<GameObject> enemiesToSpawn = new List<GameObject>();
            foreach (var enemyGroup in wave.enemyGroups)
            {
                for (int i = 0; i < enemyGroup.count; i++)
                {
                    enemiesToSpawn.Add(enemyGroup.enemyPrefab);
                }
            }
            enemiesAlive = enemiesToSpawn.Count;

            for (int i = 0; i < enemiesToSpawn.Count; i++)
            {
                int randomIndex = Random.Range(i, enemiesToSpawn.Count);
                GameObject temp = enemiesToSpawn[i];
                enemiesToSpawn[i] = enemiesToSpawn[randomIndex];
                enemiesToSpawn[randomIndex] = temp;
            }

            foreach (var enemyPrefab in enemiesToSpawn)
            {
                Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                GameObject enemyInstance = ObjectPooler_D.Instance.SpawnFromPool(enemyPrefab.name, randomSpawnPoint.position, Quaternion.identity);

                if (enemyInstance != null)
                {
                    // AMENDED: Now correctly initializes the two enemy types.
                    if (enemyInstance.TryGetComponent<EnemyAI_D>(out var standardEnemy)) standardEnemy.Initialize(this);
                    else if (enemyInstance.TryGetComponent<SaboteurAI_D>(out var saboteurEnemy)) saboteurEnemy.Initialize(this);
                }

                yield return new WaitForSeconds(0.75f);
            }
        }

        public void OnEnemyDied()
        {
            enemiesAlive--;
        }
    }
}