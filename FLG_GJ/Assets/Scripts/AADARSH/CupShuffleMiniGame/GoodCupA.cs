using UnityEngine;

public class GoodCupA : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] GameObject canvas;
    [SerializeField] CupShuffleA shuffler;
    public int score;
    private void OnMouseDown() {
        
        if (ShuffleManager_A.shufflefinished&& canvas.active==false) {
            score++;
            Debug.Log("RightChoice " + score);
            if (score >= 3) {
                Debug.Log("GameFinished");
                return;
            }
            if (canvas != null) {
                canvas.SetActive(true);
                shuffler.IncreaseDifficulty();
            }
        }
    }
}
