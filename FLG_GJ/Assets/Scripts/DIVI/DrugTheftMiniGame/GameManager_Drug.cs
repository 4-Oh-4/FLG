using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager_Drug : MonoBehaviour
{
    public static GameManager_Drug Instance { get; private set; }
    public bool HasPackage { get; private set; }
    public GameObject canvas;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Time.timeScale = 1f;
        canvas.SetActive(false);
    }

    public void OnPackageCollected()
    {
        HasPackage = true;
        Debug.Log("Package collected – get to extraction!");
    }

    public void OnPlayerCaught()
    {
        if (Time.timeScale == 0f) return;
        Debug.Log("Caught! Game Over.");
        StartCoroutine(ShowAndWaitRoutine());

    }
    private IEnumerator ShowAndWaitRoutine() {
        canvas.SetActive(true);
        yield return new WaitForSeconds(3f);
        Scene currentScene = gameObject.scene;

        // Reload that scene (no unload needed)
        SceneManager.UnloadSceneAsync("DrugTheftMiniGame");
        SceneManager.LoadSceneAsync("DrugTheftMiniGame", LoadSceneMode.Additive);
    }
    public void TryWin()
    {
        if (HasPackage)
        {
            FindAnyObjectByType<LoadUnloadMiniGamesPlayerA>().UnloadMiniGame("DrugTheft");
            Debug.Log("You escaped with the package! You win.");
            SceneManager.UnloadSceneAsync("DrugTheftMiniGame");
            

        }
    }
}
