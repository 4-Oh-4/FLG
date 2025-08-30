using UnityEngine;
using UnityEngine.SceneManagement;

public class GetOutOfHouseA : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter2D(Collider2D collision) {
        FindAnyObjectByType<LoadUnloadMiniGamesPlayerA>().UnloadMiniGame("Carlos_house2");
        SceneManager.UnloadSceneAsync("Carlos_house2");

    }
}
