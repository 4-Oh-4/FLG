using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    [Tooltip("The parent panel with the background for the pause menu.")]
    [SerializeField] private GameObject pauseMenuPanel;
    [Tooltip("The child panel that holds the settings UI elements.")]
    [SerializeField] private GameObject settingsPanel;

    private bool isPaused = false;

    private void Start()
    {
        // Ensure both panels are hidden at the start and the game is running.
        pauseMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    private void Update()
    {
        // Listen for the Escape key to toggle the pause menu.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    // Pauses the game and shows the main pause panel.
    void PauseGame()
    {
        isPaused = true;
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f; // Freezes game time.
    }

    // --- Public Button Functions ---

    // Resumes the game and hides all pause UI.
    public void ResumeGame()
    {
        isPaused = false;
        pauseMenuPanel.SetActive(false);
        settingsPanel.SetActive(false); // Also hide settings panel on resume.
        Time.timeScale = 1f; // Un-freezes game time.
    }

    // Called by the "Settings" button.
    public void OpenSettings()
    {
        // Hides the main pause panel and shows the settings panel.
        pauseMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    // Called by the "Back" button in the settings panel.
    public void CloseSettings()
    {
        // Hides the settings panel and shows the main pause panel again.
        settingsPanel.SetActive(false);
        pauseMenuPanel.SetActive(true);
    }

    // Called by the "Quit" button.
    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        // The line below is commented out to prevent errors until the scene exists.
        // SceneManager.LoadScene("MainMenu");
        Debug.Log("Quit to Main Menu button clicked!");
    }
}