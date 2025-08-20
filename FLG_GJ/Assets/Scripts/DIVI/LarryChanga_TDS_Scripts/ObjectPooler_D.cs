using System.Collections.Generic;
using UnityEngine;

namespace TopDownShooter
{
    // A class to define what to pool in the Inspector.
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public class ObjectPooler_D : MonoBehaviour
    {
        // A "Singleton" instance that can be accessed from any other script.
        public static ObjectPooler_D Instance;

        public List<Pool> pools;
        public Dictionary<string, Queue<GameObject>> poolDictionary;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            poolDictionary = new Dictionary<string, Queue<GameObject>>();

            // Create all the objects for each pool at the start of the game.
            foreach (Pool pool in pools)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();
                for (int i = 0; i < pool.size; i++)
                {
                    GameObject obj = Instantiate(pool.prefab);
                    obj.SetActive(false); // Start them as inactive.
                    objectPool.Enqueue(obj);
                }
                poolDictionary.Add(pool.tag, objectPool);
            }
        }

        // Called by the spawner to get an enemy from the pool.
        public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
        {
            if (!poolDictionary.ContainsKey(tag)) return null;

            // Take an object from the front of the queue.
            GameObject objectToSpawn = poolDictionary[tag].Dequeue();

            // Activate it and set its position/rotation.
            objectToSpawn.SetActive(true);
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;

            // Add the object back to the end of the queue so it can be reused later.
            poolDictionary[tag].Enqueue(objectToSpawn);

            return objectToSpawn;
        }
    }
}