using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameWithDelay : MonoBehaviour
{
    // The name of the scene to load after the delay
    public string nextSceneName = "MainMenu";
    // The delay in seconds before the scene loads
    public float delayInSeconds = 5.0f;

    // This function starts the coroutine to end the game
    public void EndGameAfterDelay()
    {
        StartCoroutine(EndGameCoroutine());
    }
    private void Start()
    {
        EndGameAfterDelay();
    }
    private IEnumerator EndGameCoroutine()
    {
        // Wait for the specified number of seconds
        yield return new WaitForSeconds(delayInSeconds);
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        // Load the next scene
        SceneManager.LoadScene(nextSceneName);
    }
}