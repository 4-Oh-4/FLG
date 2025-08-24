using UnityEngine;

// This allows you to create "Wave" assets in the Project window.
[CreateAssetMenu(fileName = "NewWave", menuName = "TruckChase/Wave Data")]
public class WaveData_D : ScriptableObject
{
    // A list of all enemy groups that make up this wave.
    public EnemyGroup[] enemyGroups;
    // The time to wait after this wave is cleared.
    public float timeUntilNextWave = 5f;
}

// Defines a "squad" of a specific enemy type within a wave.
[System.Serializable]
public class EnemyGroup
{
    public GameObject enemyPrefab; // e.g., Bike_D, Sedan_D
    public int count;              // e.g., Spawn 5 of them
    public float spawnInterval = 0.5f; // Time between each spawn in this group
}