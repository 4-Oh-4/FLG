using UnityEngine;
using UnityEngine.SceneManagement;

// This line ensures the GameObject will always have an AudioSource component.
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

    private bool isPaused = false;
    private AudioSource audioSource;

    private void Awake()
    {
        // Get the AudioSource component so we can play sounds.
        audioSource = GetComponent<AudioSource>();
    }

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

    // --- Game State & Panel Functions ---

    void PauseGame()
    {
        isPaused = true;
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f; // Freezes game time.
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseMenuPanel.SetActive(false);
        settingsPanel.SetActive(false); // Also hide settings panel on resume.
        Time.timeScale = 1f; // Un-freezes game time.
    }

    public void OpenSettings()
    {
        pauseMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        pauseMenuPanel.SetActive(true);
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        // The line below is commented out to prevent errors until the scene exists.
        // SceneManager.LoadScene("MainMenu");
        Debug.Log("Quit to Main Menu button clicked!");
    }

    // --- Sound Effect Functions ---

    // This function will be called by a button's "PointerDown" event.
    public void PlayButtonPressedSound()
    {
        if (buttonPressedSound != null)
        {
            audioSource.PlayOneShot(buttonPressedSound);
        }
    }

    // This function will be called by a button's "PointerUp" event.
    public void PlayButtonReleasedSound()
    {
        if (buttonReleasedSound != null)
        {
            audioSource.PlayOneShot(buttonReleasedSound);
        }
    }
}