using UnityEngine;

public class ShootingManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]public static int score = 0;
    [SerializeField] int targetScore = 15;
    [SerializeField] BottleSpawnerA spawner;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (score >= targetScore) {
            Debug.Log("Finished");
            spawner.StopAllCoroutines();
            gameObject.SetActive(false);
        }
    }
}
