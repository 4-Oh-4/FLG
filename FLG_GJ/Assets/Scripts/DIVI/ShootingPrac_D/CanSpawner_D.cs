using System.Collections;
using UnityEngine;

public class CanSpawner_D : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private GameObject canPrefab;
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
        var wait = new WaitForSeconds(s.spawnInterval);
        while (spawning)
        {
            SpawnOne(s);
            yield return wait;
        }
    }

    private void SpawnOne(RoundSettings s)
    {
        float x = Random.Range(spawnXRange.x, spawnXRange.y);
        Vector3 pos = new Vector3(x, transform.position.y, 0f);

        var go = Instantiate(canPrefab, pos, Quaternion.identity);
        var can = go.GetComponent<Can_D>();
        float up = Random.Range(s.minUpForce, s.maxUpForce);
        can.Launch(s.gravityScale, up, s.lateralForce);
        go.layer = LayerMask.NameToLayer("Can");
    }
}
