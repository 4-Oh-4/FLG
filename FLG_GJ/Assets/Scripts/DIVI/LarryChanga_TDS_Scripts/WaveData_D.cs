using UnityEngine;

namespace TopDownShooter
{
    // This allows you to create "Wave" assets directly in the Project window.
    [CreateAssetMenu(fileName = "NewWave", menuName = "TopDownShooter/Wave Data")]
    public class WaveData_D : ScriptableObject
    {
        public EnemyGroup[] enemyGroups;
        public float timeUntilNextWave = 5f;
    }

    // This defines a "squad" of a specific enemy type within a wave.
    [System.Serializable]
    public class EnemyGroup
    {
        public GameObject enemyPrefab; // E.g., Rusher_D, Brute_D
        public int count;              // E.g., Spawn 10 of them
        public float spawnInterval = 0.5f; // Time between each spawn
    }
}