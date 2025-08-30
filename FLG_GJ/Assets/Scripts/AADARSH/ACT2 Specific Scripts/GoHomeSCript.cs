using UnityEngine;

public class GoHomeSCript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]private int once = 2;
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            if (once == 90) {
                FindAnyObjectByType<LoadUnloadMiniGamesPlayerA>().LoadMiniGame("Carlos_house3");
                once = 5;
            }
            if (once == 0) {
                FindAnyObjectByType<LoadUnloadMiniGamesPlayerA>().LoadMiniGame("Carlos_house2");
                once = 5;
            }
            
        }
    }
    public void resetOnce() {
        once = 0;
    }
    public void resetTwice() {
        once = 90;
    }
}
