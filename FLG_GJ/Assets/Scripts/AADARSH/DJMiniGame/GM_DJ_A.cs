using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TopDownShooter;
public class GM_DJ_A : MonoBehaviour
{
    [SerializeField] private bool isCompleted = false;
    [SerializeField] string miniGamename= "DJ_MiniGame_D";
    [SerializeField] GameObject canvas;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (isCompleted) TryWin();
    }
    public void TryWin() {
        if (true) {

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            FindAnyObjectByType<LoadUnloadMiniGamesPlayerA>().UnloadMiniGame(miniGamename);
            Debug.Log(miniGamename);
            SceneManager.UnloadSceneAsync(miniGamename);


        }
    }
    private IEnumerator ShowAndWaitRoutine() {
        canvas.SetActive(true);
        yield return new WaitForSeconds(3f);
        //Scene currentScene = gameObject.scene;

        // Reload that scene (no unload needed)
        SceneManager.UnloadSceneAsync(miniGamename);
        SceneManager.LoadSceneAsync(miniGamename, LoadSceneMode.Additive);
    }
    
    public void Fail() {
        DeleteAllEnemies();
        StopAllCoroutines();
        StartCoroutine(ShowAndWaitRoutine());
    }
    // In your GM_DJ_A.cs script
    // No other changes are needed to other scripts or prefabs.

    private void DeleteAllEnemies() {
        // Find all objects with the EnemyIdentifier component, including inactive ones.
        // Added FindObjectsSortMode.None to match the updated Unity API.
        EnemyIdentifier[] allEnemies = FindObjectsByType<EnemyIdentifier>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (EnemyIdentifier enemy in allEnemies) {
            // Destroy the GameObject that the component is attached to.
            Destroy(enemy.gameObject);
        }

        Debug.Log($"Deleted {allEnemies.Length} enemies.");
    }
}
