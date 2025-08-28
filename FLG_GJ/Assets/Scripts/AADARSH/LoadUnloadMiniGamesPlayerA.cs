using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LoadUnloadMiniGamesPlayerA : MonoBehaviour {
    [SerializeField] Camera maincamera;
    [SerializeField] GameObject player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void LoadMiniGame(string miniGameName) {
        Time.timeScale = 0f;
        maincamera.gameObject.SetActive(false);
        player.SetActive(false);
        SceneManager.LoadScene(miniGameName,LoadSceneMode.Additive);
        Scene miniScene = SceneManager.GetSceneByName(miniGameName);
        
    }
    public void UnloadMiniGame() {
        Time.timeScale = 1f;
        maincamera.gameObject.SetActive(true);
        player.SetActive(true);
    }
}