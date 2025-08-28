using UnityEngine;

public class LoadMiniGameA : MonoBehaviour
{
    [SerializeField] string MiniGameName;
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            collision.GetComponent<LoadUnloadMiniGamesPlayerA>().LoadMiniGame(MiniGameName);
        }
    }
}
