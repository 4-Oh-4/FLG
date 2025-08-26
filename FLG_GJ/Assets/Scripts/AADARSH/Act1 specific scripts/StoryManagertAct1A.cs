using System.Collections.Generic;
using UnityEngine;

public class StoryManagertAct1A : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static StoryManagertAct1A Instance;
    public GameManagerAct1A gameM;
    // Track overall chapter
    public int currentChapter = 0;

    // Track story flags (events, choices, mini-game results)
    public Dictionary<string, bool> flags = new Dictionary<string, bool>();

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
        flags["WeeksLater"] = false;
        // Initialize story flags here
        flags["Collect"] = false;
        flags["AfterFightTalk"] = false;
        flags["EvTalk"] = false;
    }

    // Helper functions to set/check flags
    public void SetFlag(string key, bool value) {
        if (flags.ContainsKey(key)) {
            flags[key] = value;
        } else {
            flags.Add(key, value);
        }
        if (key == "Collect") {
            if (gameM!= null) {
                gameM.ShowUI();
            }
        }
    }

    public bool GetFlag(string key) {
        return flags.ContainsKey(key) && flags[key];
    }
}
