using UnityEngine;

public class LoadMiniGameA : MonoBehaviour
{
    [SerializeField] string MiniGameName;
    private int once = 0;
    [SerializeField] GameObject gameManager;
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")&&once==0) {
            once = 1;
            gameManager.GetComponent<LoadUnloadMiniGamesPlayerA>().LoadMiniGame(MiniGameName);
        }
        if (collision.CompareTag("Player") && once == 5) {
            once = 1;
            gameManager.GetComponent<LoadUnloadMiniGamesPlayerA>().LoadMiniGame(MiniGameName+"2");
        }
    }
    public void SetOnce() {
        once = 5;
    }
}
