using TMPro;
using UnityEngine;

public class ShootingManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]public static int score = 0;
    [SerializeField] int targetScore = 15;
    [SerializeField] BottleSpawnerA spawner;
    [SerializeField] TextMeshProUGUI TargetScore;
    [SerializeField] TextMeshProUGUI ScoreBoard;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        TargetScore.text = "Target Score: " + targetScore.ToString();
        ScoreBoard.text = "Score: " + score.ToString();
        if (score >= targetScore) {
            Debug.Log("Finished");
            spawner.StopAllCoroutines();
            spawner.GameFinished();
            gameObject.SetActive(false);
        }
    }
    public static void ResetAll() {

    }
}
