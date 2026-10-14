using UnityEngine;
using UnityEngine.EventSystems;
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

    [Header("Global Pause")]
    [Tooltip("Scenes where Escape does nothing (video cutscenes, end screen, menus).")]
    [SerializeField] private string[] noPauseScenes = { "Visual1", "Visual2", "Visual3", "Visual4", "TheEnd", "MainMenu" };
    [Tooltip("Scene loaded by the Quit button. If it isn't in Build Settings, Quit exits the game instead.")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [Tooltip("Used only when the current scenes have no EventSystem, so the buttons still work.")]
    [SerializeField] private GameObject fallbackEventSystem;
    [Tooltip("Full-screen dim image shown while paused. Also blocks clicks on the game UI behind the menu.")]
    [SerializeField] private GameObject backdrop;

    // Name of the prefab in a Resources folder that is spawned once at game start.
    private const string PrefabResourceName = "PauseMenu";

    /// <summary>The one persistent pause menu.</summary>
    public static PauseMenuManager Instance { get; private set; }
    /// <summary>True while the game is paused. Gameplay input scripts check this.</summary>
    public static bool IsPaused { get; private set; }

    private bool isPaused = false;
    private bool settingsOpenedFromMenu = false;
    private AudioSource audioSource;
    private float timeScaleBeforePause = 1f;
    private bool cursorVisibleBeforePause;
    private CursorLockMode cursorLockBeforePause;

    // Spawns the pause menu once, so no scene needs its own copy.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void CreateGlobalInstance()
    {
        if (Instance != null) return;
        GameObject prefab = Resources.Load<GameObject>(PrefabResourceName);
        if (prefab != null) Instantiate(prefab).name = PrefabResourceName;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(transform.root.gameObject);
            return;
        }
        Instance = this;
        IsPaused = false;
        DontDestroyOnLoad(transform.root.gameObject);

        // Get the AudioSource component so we can play sounds.
        audioSource = GetComponent<AudioSource>();
        audioSource.ignoreListenerPause = true; // button clicks must play while the game audio is paused
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
            IsPaused = false;
        }
    }

    private void Start()
    {
        // Ensure both panels are hidden at the start and the game is running.
        pauseMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        if (fallbackEventSystem != null) fallbackEventSystem.SetActive(false);
        if (backdrop != null) backdrop.SetActive(false);
    }

    private void Update()
    {
        // Listen for the Escape key to toggle the pause menu.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingsOpenedFromMenu)
            {
                CloseSettings();
            }
            else if (isPaused)
            {
                ResumeGame();
            }
            else if (CanPauseHere())
            {
                PauseGame();
            }
        }
    }

    private bool CanPauseHere()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            string sceneName = SceneManager.GetSceneAt(i).name;
            foreach (string blocked in noPauseScenes)
            {
                if (sceneName == blocked) return false;
            }
        }
        return true;
    }

    // --- Game State & Panel Functions ---

    void PauseGame()
    {
        isPaused = true;
        IsPaused = true;
        if (backdrop != null) backdrop.SetActive(true);
        pauseMenuPanel.SetActive(true);

        // Remember the current speed so minigames that change it get it back on resume.
        timeScaleBeforePause = Time.timeScale;
        Time.timeScale = 0f; // Freezes game time.
        AudioListener.pause = true;

        cursorVisibleBeforePause = Cursor.visible;
        cursorLockBeforePause = Cursor.lockState;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (fallbackEventSystem != null && EventSystem.current == null)
        {
            fallbackEventSystem.SetActive(true);
        }

        // Catch sounds spawned since the last scene load so the volume sliders affect them too.
        AudioMixerRouterA router = GetComponentInParent<AudioMixerRouterA>();
        if (router != null) router.RouteUnassignedSources();
    }

    public void ResumeGame()
    {
        isPaused = false;
        IsPaused = false;
        if (backdrop != null) backdrop.SetActive(false);
        pauseMenuPanel.SetActive(false);
        settingsPanel.SetActive(false); // Also hide settings panel on resume.

        Time.timeScale = timeScaleBeforePause; // Un-freezes game time.
        AudioListener.pause = false;
        Cursor.visible = cursorVisibleBeforePause;
        Cursor.lockState = cursorLockBeforePause;

        if (fallbackEventSystem != null) fallbackEventSystem.SetActive(false);
    }

    public void OpenSettings()
    {
        pauseMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        if (settingsOpenedFromMenu)
        {
            // Opened from the main menu: there is no pause panel to go back to.
            settingsOpenedFromMenu = false;
            if (backdrop != null) backdrop.SetActive(false);
            return;
        }
        pauseMenuPanel.SetActive(true);
    }

    /// <summary>Shows the settings panel on its own (used by the main menu, where the game is not paused).</summary>
    public void OpenSettingsFromMenu()
    {
        if (isPaused) return;
        settingsOpenedFromMenu = true;
        if (backdrop != null) backdrop.SetActive(true);
        settingsPanel.SetActive(true);
    }

    public void QuitToMainMenu()
    {
        if (isPaused) ResumeGame();
        Time.timeScale = 1f;

        if (Application.CanStreamedLevelBeLoaded(mainMenuSceneName))
        {
            SceneManager.LoadScene(mainMenuSceneName);
            return;
        }

        // No main menu in the build yet: quit the game instead.
        Debug.Log("Main menu scene not in build, quitting the game.");
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
