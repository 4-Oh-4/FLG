using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Title screen: New Game, Continue (last chapter reached), Settings, Quit.
/// </summary>
public class MainMenuManagerA : MonoBehaviour {
    [Tooltip("First scene of a new game (the intro cutscene).")]
    [SerializeField] private string newGameScene = "Visual1";
    [SerializeField] private Button continueButton;
    [Tooltip("Shows which chapter Continue will load.")]
    [SerializeField] private TMP_Text continueLabel;

    private void Awake() {
        // Coming back from the pause menu: make sure nothing from the last session is still frozen or alive.
        Time.timeScale = 1f;
        AudioListener.pause = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        DestroyPersistentObjects();
    }

    private void Start() {
        bool hasSave = ChapterSaveA.HasSave;
        if (continueButton != null) {
            continueButton.interactable = hasSave;
            // The buttons swap sprites instead of tinting, so fade Continue out by hand when there is no save.
            if (continueButton.image != null) continueButton.image.color = hasSave ? Color.white : new Color(1f, 1f, 1f, 0.35f);
        }
        if (continueLabel != null) continueLabel.text = hasSave ? ChapterSaveA.SavedChapterLabel : "";
    }

    public void NewGame() {
        SceneManager.LoadScene(newGameScene);
    }

    public void ContinueGame() {
        if (!ChapterSaveA.HasSave) return;
        SceneManager.LoadScene(ChapterSaveA.SavedScene);
    }

    public void OpenSettings() {
        if (PauseMenuManager.Instance != null) PauseMenuManager.Instance.OpenSettingsFromMenu();
    }

    public void QuitGame() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // Same as the act resetters (destroy GameController-tagged objects), plus the game's other
    // DontDestroyOnLoad objects, which survive if the player quits to the menu mid-act or mid-minigame.
    // The pause menu and Unity's own persistent objects are left alone.
    private static void DestroyPersistentObjects() {
        GameObject probe = new GameObject("PersistentSceneProbe");
        DontDestroyOnLoad(probe);
        foreach (GameObject root in probe.scene.GetRootGameObjects()) {
            if (root == probe) continue;
            bool isGameObjectFromLastSession = root.CompareTag("GameController")
                || root.GetComponentInChildren<StoryManagertAct1A>(true) != null
                || root.GetComponentInChildren<GameManager_Drug>(true) != null;
            if (isGameObjectFromLastSession) Destroy(root);
        }
        Destroy(probe);
    }
}
