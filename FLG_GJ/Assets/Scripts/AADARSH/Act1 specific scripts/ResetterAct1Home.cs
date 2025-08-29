using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetterAct1Home : MonoBehaviour {
    public void UnloadEverything() {
        // Unload all loaded scenes except the currently active one
        for (int i = 0; i < SceneManager.sceneCount; i++) {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.isLoaded) {
                SceneManager.UnloadSceneAsync(scene);
            }
        }

        // Destroy all DontDestroyOnLoad objects
        DestroyDontDestroyOnLoadObjects();
    }

    private void DestroyDontDestroyOnLoadObjects() {
        // Create a new temporary scene
        var temp = new GameObject("TempSceneHolder");
        Scene tempScene = SceneManager.CreateScene("TempScene");

        // Move the temp object into that scene
        SceneManager.MoveGameObjectToScene(temp, tempScene);

        // Now find ALL root objects in DontDestroyOnLoad by moving the temp object
        Scene dontDestroyScene = temp.scene;
        foreach (var root in dontDestroyScene.GetRootGameObjects()) {
            Destroy(root);
        }

        // Clean up temp
        SceneManager.UnloadSceneAsync(tempScene);
        SceneManager.LoadScene("Carlos_house");
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            UnloadEverything();
        }
    }
}
