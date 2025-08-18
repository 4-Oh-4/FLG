using UnityEngine;

public class GoodCupA : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] ShuffleManager_A shuffleManager;
    [SerializeField] GameObject canvas;
    public int score;
    private void OnMouseDown() {
        score++;
        Debug.Log("RightChoice"+score);
        if (canvas != null) {
            canvas.SetActive(true);
        }
    }
}
