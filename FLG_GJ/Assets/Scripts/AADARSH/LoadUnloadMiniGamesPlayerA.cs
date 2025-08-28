using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LoadUnloadMiniGamesPlayerA : MonoBehaviour {
    [SerializeField] Camera maincamera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void LoadMiniGame(string miniGameName) {
        Time.timeScale = 0f;
        maincamera.gameObject.SetActive(false);
        GetComponent<PlayerInput>().enabled = false;
        GetComponent<InteractWithDialogueA>().enabled = false;
        SceneManager.LoadScene(miniGameName,LoadSceneMode.Additive);
        Scene miniScene = SceneManager.GetSceneByName(miniGameName);
        if (miniScene.IsValid())
            SceneManager.SetActiveScene(miniScene);
    }
    public void UnloadMiniGame() {
        Time.timeScale = 1f;
        maincamera.gameObject.SetActive(true);
        GetComponent<PlayerInput>().enabled = true;
        GetComponent<InteractWithDialogueA>().enabled = true;
    }
}