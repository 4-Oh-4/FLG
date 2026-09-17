using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

/// <summary>
/// Manages the pause menu. Works globally across all scenes via DontDestroyOnLoad.
/// Supports pausing during dialogue, exit to desktop, and quit to main menu.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class PauseMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    [Tooltip("The parent panel with the background for the pause menu.")]
    [SerializeField] private GameObject pauseMenuPanel;
    [Tooltip("The child panel that holds the settings UI elements.")]
    [SerializeField] private GameObject settingsPanel;

    [Header("Button Sound Effects")]
    [Tooltip("The 'compressed' sound that plays on mouse down.")]
    [SerializeField] private AudioClip buttonPressedSound;
    [Tooltip("The 'uncompressed' sound that plays on mouse up.")]
    [SerializeField] private AudioClip buttonReleasedSound;

    [Header("Settings")]
    [Tooltip("The name of the main menu scene to load when quitting.")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool isPaused = false;
    private AudioSource audioSource;

    /// <summary>
    /// True when the game is paused. Other scripts can check this to know the pause state.
    /// </summary>
    public static bool IsGamePaused { get; private set; }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        // Ensure both panels are hidden at the start and the game is running.
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        Time.timeScale = 1f;
        IsGamePaused = false;
    }

    private void Update()
    {
        // Listen for the Escape key using the New Input System.
        // This works even during dialogue since we don't block it.
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
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

    // --- Game State & Panel Functions ---

    void PauseGame()
    {
        isPaused = true;
        IsGamePaused = true;
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f; // Freezes game time.
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        isPaused = false;
        IsGamePaused = false;
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        Time.timeScale = 1f; // Un-freezes game time.
    }

    public void OpenSettings()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
    }

    public void QuitToMainMenu()
    {
        // Restore time before loading a new scene.
        Time.timeScale = 1f;
        isPaused = false;
        IsGamePaused = false;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    /// <summary>
    /// Quits the application entirely. Works in builds; stops play mode in Editor.
    /// </summary>
    public void ExitToDesktop()
    {
        Debug.Log("Exit to Desktop requested.");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // --- Sound Effect Functions ---

    // This function will be called by a button's "PointerDown" event.
    public void PlayButtonPressedSound()
    {
        if (buttonPressedSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(buttonPressedSound);
        }
    }

    // This function will be called by a button's "PointerUp" event.
    public void PlayButtonReleasedSound()
    {
        if (buttonReleasedSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(buttonReleasedSound);
        }
    }
}