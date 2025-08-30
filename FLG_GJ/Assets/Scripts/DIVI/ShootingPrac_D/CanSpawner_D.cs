using System.Collections;
using UnityEngine;

public class CanSpawner_D : MonoBehaviour
{
    [Header("Setup")]
    
    [SerializeField] private GameObject[] canPrefabs;
    [SerializeField] private Vector2 spawnXRange = new Vector2(-6f, 6f);

    private bool spawning;
    private Coroutine loop;

    public void Begin(RoundSettings settings)
    {
        if (loop != null) StopCoroutine(loop);
        spawning = true;
        loop = StartCoroutine(SpawnLoop(settings));
    }

    public void End()
    {
        spawning = false;
        if (loop != null) StopCoroutine(loop);
        loop = null;
    }

    private IEnumerator SpawnLoop(RoundSettings s)
    {
        int spawnedCount = 0;
        while (spawning && spawnedCount < s.totalCans)
        {
            SpawnOne(s);
            spawnedCount++;
            float waitTime = Random.Range(s.minSpawnInterval, s.maxSpawnInterval);
            yield return new WaitForSeconds(waitTime);
        }
        End();
    }

    private void SpawnOne(RoundSettings s)
    {
        
        if (canPrefabs == null || canPrefabs.Length == 0)
        {
            Debug.LogError("Can Prefabs array is not set up in the Spawner!");
            return;
        }

        
        int randomIndex = Random.Range(0, canPrefabs.Length);
        GameObject prefabToSpawn = canPrefabs[randomIndex];

       
        float x = Random.Range(spawnXRange.x, spawnXRange.y);
        Vector3 pos = new Vector3(x, transform.position.y, 0f);

      
        var go = Instantiate(prefabToSpawn, pos, Quaternion.identity);
        

        var can = go.GetComponent<Can_D>();

        
        float up = Random.Range(s.minUpForce, s.maxUpForce);
        float lateral = Random.Range(-s.maxLateralForce, s.maxLateralForce);

        can.Launch(s.gravityScale, up, lateral);
        go.layer = LayerMask.NameToLayer("Can");
    }
}