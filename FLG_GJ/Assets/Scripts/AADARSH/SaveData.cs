using System.Collections.Generic;

/// <summary>
/// Serializable container for all save data.
/// Stored as JSON in Application.persistentDataPath.
/// </summary>
[System.Serializable]
public class SaveData {
    /// <summary>
    /// All story flags and their current values.
    /// </summary>
    public List<FlagEntry> flags = new List<FlagEntry>();

    /// <summary>
    /// The name of the scene the player was in when saving.
    /// </summary>
    public string sceneName;

    /// <summary>
    /// Player position at the time of save.
    /// </summary>
    public float playerX;
    public float playerY;

    /// <summary>
    /// Which act the player is currently in (1, 2, or 3).
    /// </summary>
    public int actNumber = 1;

    /// <summary>
    /// A human-readable checkpoint identifier for debugging/display.
    /// </summary>
    public string checkpointId;

    /// <summary>
    /// Timestamp of when the save was created (for display purposes).
    /// </summary>
    public string timestamp;
}

/// <summary>
/// A single flag entry. We use this instead of Dictionary because 
/// Unity's JsonUtility doesn't serialize Dictionaries directly.
/// </summary>
[System.Serializable]
public class FlagEntry {
    public string key;
    public bool value;

    public FlagEntry() { }
    public FlagEntry(string key, bool value) {
        this.key = key;
        this.value = value;
    }
}
