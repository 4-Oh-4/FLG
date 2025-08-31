using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetterAct2Home : MonoBehaviour {
    [Tooltip("The name of the scene to load after the reset.")]
    [SerializeField]
    private string sceneToLoad = "Carlos_house";

    [Tooltip("The tag assigned to all objects that persist between scenes (e.g., GameManager, AudioManager) and should be destroyed on reset.")]
    [SerializeField]
    private string persistentObjectTag = "GameController";

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            ResetGame();
        }
    }

    public void ResetGame() {
        // 1. Find and destroy all persistent objects.
        // It's much safer to find objects by a specific tag than to use complex scene-moving hacks.
        GameObject[] persistentObjects = GameObject.FindGameObjectsWithTag(persistentObjectTag);

        foreach (GameObject obj in persistentObjects) {
            Destroy(obj);
        }

        // 2. Load the initial scene. 
        // Using LoadSceneMode.Single automatically unloads all other scenes,
        // which is exactly what you want.
        SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);
    }
}