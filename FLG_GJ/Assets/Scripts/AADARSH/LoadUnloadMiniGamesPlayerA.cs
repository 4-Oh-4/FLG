using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LoadUnloadMiniGamesPlayerA : MonoBehaviour {
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void LoadMiniGame(string miniGameName) {
        SceneManager.LoadSceneAsync(miniGameName);
        GetComponent<PlayerInput>().enabled = false;
        GetComponent<InteractWithDialogueA>().enabled = false;
    }
    public void UnloadMiniGame() {
        GetComponent<PlayerInput>().enabled = true;
        GetComponent<InteractWithDialogueA>().enabled = true;
    }
}