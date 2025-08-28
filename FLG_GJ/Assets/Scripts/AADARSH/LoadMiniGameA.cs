using UnityEngine;

public class LoadMiniGameA : MonoBehaviour
{
    [SerializeField] string MiniGameName;
    private bool once = true;
    [SerializeField] GameObject gameManager;
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")&&once) {
            once = false;
            gameManager.GetComponent<LoadUnloadMiniGamesPlayerA>().LoadMiniGame(MiniGameName);
        }
    }
}
