using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Controls the Main Menu scene UI.
/// Handles New Game, Continue, Settings, and Quit buttons.
///
/// SETUP (Unity Editor):
/// 1. Create a new scene called "MainMenu" and add it to Build Settings.
/// 2. Create a Canvas with buttons: New Game, Continue, Settings, Quit.
/// 3. Add this script to a Manager GameObject in the scene.
/// 4. Wire the buttons to this script's public methods via onClick events.
/// 5. The Continue button should be assigned to the continueButton field so it can be disabled when no save exists.
/// </summary>
public class MainMenuManager : MonoBehaviour {

    [Header("Scene Names")]
    [Tooltip("The name of the first scene to load for a new game (Act 1).")]
    [SerializeField] private string newGameSceneName = "Act_1_Map_K";

    [Header("UI References")]
    [Tooltip("The Continue button — will be disabled if no save file exists.")]
    [SerializeField] private Button continueButton;

    [Tooltip("Optional: The settings panel to show/hide.")]
    [SerializeField] private GameObject settingsPanel;

    [Header("Display")]
    [Tooltip("Optional: A text element to show the last checkpoint info on the Continue button.")]
    [SerializeField] private TMPro.TextMeshProUGUI checkpointInfoText;

    private void Start() {
        // Ensure time is running (in case we came from a paused game)
        Time.timeScale = 1f;

        // Show cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Hide settings panel if shown
        if (settingsPanel != null) {
            settingsPanel.SetActive(false);
        }

        // Check if a save file exists and update the Continue button accordingly
        UpdateContinueButton();
    }

    /// <summary>
    /// Starts a new game. Deletes any existing save and loads the first scene.
    /// </summary>
    public void NewGame() {
        // Delete old save so we start fresh
        if (SaveManager.Instance != null) {
            SaveManager.Instance.DeleteSave();
        }

        // Destroy any persistent game objects from a previous session
        // (StoryManager, SaveManager, etc. will be re-created by the new scene)
        DestroyPersistentObjects();

        SceneManager.LoadScene(newGameSceneName);
    }

    /// <summary>
    /// Continues from the last checkpoint.
    /// </summary>
    public void ContinueGame() {
        if (SaveManager.Instance != null && SaveManager.Instance.HasSaveFile) {
            SaveManager.Instance.LoadAndRestoreGame();
        } else {
            Debug.LogWarning("[MainMenu] No save file found. Cannot continue.");
        }
    }

    /// <summary>
    /// Opens the settings panel.
    /// </summary>
    public void OpenSettings() {
        if (settingsPanel != null) {
            settingsPanel.SetActive(true);
        }
    }

    /// <summary>
    /// Closes the settings panel.
    /// </summary>
    public void CloseSettings() {
        if (settingsPanel != null) {
            settingsPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Quits the application.
    /// </summary>
    public void QuitGame() {
        Debug.Log("[MainMenu] Quit requested.");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // ─── HELPERS ───────────────────────────────────────────────────

    private void UpdateContinueButton() {
        bool hasSave = SaveManager.Instance != null && SaveManager.Instance.HasSaveFile;

        if (continueButton != null) {
            continueButton.interactable = hasSave;

            // Optionally dim the button text when disabled
            var colors = continueButton.colors;
            colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            continueButton.colors = colors;
        }

        // Show checkpoint info if available
        if (checkpointInfoText != null && hasSave) {
            SaveData data = SaveManager.Instance.LoadSaveData();
            if (data != null) {
                checkpointInfoText.text = $"Act {data.actNumber} — {data.checkpointId}";
            }
        } else if (checkpointInfoText != null) {
            checkpointInfoText.text = "No save data";
        }
    }

    private void DestroyPersistentObjects() {
        // Destroy the existing StoryManager singleton so a fresh one is created
        if (StoryManagertAct1A.Instance != null) {
            Destroy(StoryManagertAct1A.Instance.gameObject);
        }

        // Note: SaveManager and ScreenFader will persist — they're scene-independent utilities.
    }
}
