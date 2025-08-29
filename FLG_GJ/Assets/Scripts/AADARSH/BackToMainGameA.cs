using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class BackToMainGameA : MonoBehaviour {
    public string miniGameSceneName;
    [SerializeField] bool complete = false;
    private void Update() {
        if (complete) CompleteMiniGame();
    }
    public void CompleteMiniGame() {
        // Re-enable player controls
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) {
            player.GetComponent<PlayerInput>().enabled = true;
            player.GetComponent<InteractWithDialogueA>().enabled = true;
        }
        FindAnyObjectByType<LoadUnloadMiniGamesPlayerA>().UnloadMiniGame("Church");
        //  Unload minigame scene
       miniGameSceneName=SceneManager.GetActiveScene().name;
        Debug.Log(miniGameSceneName);
        SceneManager.UnloadSceneAsync(miniGameSceneName);
    }
}
