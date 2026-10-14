using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Singleton save manager that handles checkpoint-based auto-saving.
/// Saves/loads all story flags, player position, current scene, and act number.
///
/// Checkpoints are triggered by calling SaveCheckpoint() from story events 
/// (e.g., after dialogue, after minigame completion, at building entries).
///
/// Save format: JSON file at Application.persistentDataPath/flg_save.json
/// </summary>
public class SaveManager : MonoBehaviour {
    #region Singleton
    public static SaveManager Instance { get; private set; }
    #endregion

    [Header("Configuration")]
    [Tooltip("The file name for the save data.")]
    [SerializeField] private string saveFileName = "flg_save.json";

    /// <summary>
    /// The full file path where the save data is stored.
    /// </summary>
    private string SaveFilePath => Path.Combine(Application.persistentDataPath, saveFileName);

    /// <summary>
    /// True if a save file exists on disk.
    /// </summary>
    public bool HasSaveFile => File.Exists(SaveFilePath);

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    // ─── SAVE ──────────────────────────────────────────────────────

    /// <summary>
    /// Saves the current game state at a checkpoint.
    /// Call this from story events, flag changes, or building entries.
    /// </summary>
    /// <param name="checkpointId">A human-readable name for this checkpoint (e.g., "After collecting trash").</param>
    /// <param name="actNumber">The current act number (1, 2, or 3).</param>
    public void SaveCheckpoint(string checkpointId, int actNumber = 1) {
        // Prevent saving if the main player is disabled during an additive minigame load
        if (LoadUnloadMiniGamesPlayerA.IsMinigameActive) {
            Debug.LogWarning($"[SaveManager] Blocked save '{checkpointId}' because a minigame is currently active. Main player coordinates are invalid.");
            return;
        }

        SaveData data = new SaveData();

        // 1. Save all story flags
        if (StoryManagertAct1A.Instance != null) {
            Dictionary<string, bool> flags = StoryManagertAct1A.Instance.GetAllFlags();
            foreach (var kvp in flags) {
                data.flags.Add(new FlagEntry(kvp.Key, kvp.Value));
            }
        }

        // 2. Save player position
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) {
            data.playerX = player.transform.position.x;
            data.playerY = player.transform.position.y;
        }

        // 3. Save current scene
        data.sceneName = SceneManager.GetActiveScene().name;

        // 4. Save act number and checkpoint info
        data.actNumber = actNumber;
        data.checkpointId = checkpointId;
        data.timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        // 5. Write to disk
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SaveFilePath, json);

        Debug.Log($"[SaveManager] Checkpoint saved: '{checkpointId}' in {data.sceneName} (Act {actNumber})");
    }

    // ─── LOAD ──────────────────────────────────────────────────────

    /// <summary>
    /// Loads the saved game state and returns the SaveData.
    /// Does NOT automatically load the scene or place the player — 
    /// the caller (e.g., MainMenuManager) should handle that.
    /// </summary>
    /// <returns>The loaded SaveData, or null if no save file exists.</returns>
    public SaveData LoadSaveData() {
        if (!HasSaveFile) {
            Debug.LogWarning("[SaveManager] No save file found.");
            return null;
        }

        string json = File.ReadAllText(SaveFilePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        Debug.Log($"[SaveManager] Save data loaded: '{data.checkpointId}' from {data.timestamp}");
        return data;
    }

    /// <summary>
    /// Loads the save file and restores the full game state:
    /// loads the scene, restores flags, and positions the player.
    /// </summary>
    public void LoadAndRestoreGame() {
        SaveData data = LoadSaveData();
        if (data == null) return;

        // 1. Restore flags BEFORE loading the scene so that 
        // scripts that check flags in Start() will get the right values.
        RestoreFlags(data);

        // 2. Load the saved scene
        // We use a callback to position the player after the scene loads.
        SceneManager.sceneLoaded += OnSceneLoadedForRestore;
        _pendingRestoreData = data;
        SceneManager.LoadScene(data.sceneName);
    }

    private SaveData _pendingRestoreData;

    private void OnSceneLoadedForRestore(Scene scene, LoadSceneMode mode) {
        // Unsubscribe immediately to prevent repeat calls
        SceneManager.sceneLoaded -= OnSceneLoadedForRestore;

        if (_pendingRestoreData == null) return;

        // Position the player at the saved location
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) {
            player.transform.position = new Vector3(
                _pendingRestoreData.playerX,
                _pendingRestoreData.playerY,
                player.transform.position.z
            );
        }

        Debug.Log($"[SaveManager] Game restored to checkpoint: '{_pendingRestoreData.checkpointId}'");
        _pendingRestoreData = null;
    }

    /// <summary>
    /// Restores all story flags from save data.
    /// </summary>
    private void RestoreFlags(SaveData data) {
        if (StoryManagertAct1A.Instance == null) {
            Debug.LogWarning("[SaveManager] StoryManager not found — cannot restore flags.");
            return;
        }

        StoryManagertAct1A.Instance.LoadFlags(data.flags);
    }

    // ─── DELETE ─────────────────────────────────────────────────────

    /// <summary>
    /// Deletes the save file. Used for "New Game" functionality.
    /// </summary>
    public void DeleteSave() {
        if (HasSaveFile) {
            File.Delete(SaveFilePath);
            Debug.Log("[SaveManager] Save file deleted.");
        }
    }
}
