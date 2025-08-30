using UnityEngine;
using UnityEngine.SceneManagement;

public class CarlosHouse3 : MonoBehaviour {
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter2D(Collider2D collision) {
        FindAnyObjectByType<LoadUnloadMiniGamesPlayerA>().UnloadMiniGame("Carlos_house3");
        SceneManager.UnloadSceneAsync("Carlos_house3");

    }
}
