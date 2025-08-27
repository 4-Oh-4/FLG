using System.Collections.Generic;
using UnityEngine;

namespace TopDownShooter
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public class ObjectPooler_D : MonoBehaviour
    {
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
            foreach (Pool pool in pools)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();
                for (int i = 0; i < pool.size; i++)
                {
                    GameObject obj = Instantiate(pool.prefab);
                    obj.SetActive(false);
                    objectPool.Enqueue(obj);
                }
                poolDictionary.Add(pool.tag, objectPool);
            }
        }

        public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
                return null;
            }

            // AMENDED: Added a loop to find a valid, non-destroyed object.
            for (int i = 0; i < poolDictionary[tag].Count; i++)
            {
                GameObject objectToSpawn = poolDictionary[tag].Dequeue();

                // If the object is a "ghost" (was destroyed), just skip it.
                if (objectToSpawn == null)
                {
                    // Optionally, you could create a new object here to maintain pool size.
                    // For now, we'll just let the pool shrink.
                    continue;
                }

                // If we found a valid object, activate it and put it back in the queue.
                objectToSpawn.SetActive(true);
                objectToSpawn.transform.position = position;
                objectToSpawn.transform.rotation = rotation;
                poolDictionary[tag].Enqueue(objectToSpawn);

                return objectToSpawn;
            }

            // This will only be reached if every object in the pool was a "ghost".
            Debug.LogError("Could not find a valid object to spawn for tag: " + tag + ". Is the pool too small?");
            return null;
        }
    }
}