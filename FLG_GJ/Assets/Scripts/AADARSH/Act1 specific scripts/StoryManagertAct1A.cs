using UnityEngine;
using UnityEngine.Events; // Required for UnityEvent
using System;             // Required for Action (the C# event)
using System.Collections.Generic;

// First, we define the helper class for our Inspector events.
// This makes it so we can have a list of named events in the StoryManager.


/// <summary>
/// A singleton manager responsible for tracking the game's story state through a system of boolean flags.
/// It provides methods to set and check flags, and broadcasts events whenever a flag's state changes.
/// </summary>
public class StoryManagertAct1A : MonoBehaviour {
    #region Singleton Pattern

    // The static instance of the StoryManager, accessible from anywhere.
    public static StoryManagertAct1A Instance { get; private set; }

    #endregion

    #region Events

    /// <summary>
    /// A static C# event that other scripts can subscribe to for code-based reactions.
    /// It broadcasts the (string flagName, bool newValue) whenever a flag is set.
    /// </summary>
    public static event Action<string, bool> OnFlagChanged;

    [Header("Flag Events (Setup in Inspector)")]
    [Tooltip("Define reactions to flag changes directly in the Unity Inspector.")]
    [SerializeField] private List<FlagUnityEventA> inspectorFlagEvents;

    #endregion

    #region Private State

    // The core dictionary that holds the current state of all story flags.
    private Dictionary<string, bool> flags = new Dictionary<string, bool>();

    #endregion

    #region Unity Lifecycle

    private void Awake() {
        // Implement the singleton pattern.
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Make this manager persist across scene loads.
            InitializeFlags(); // Set up the initial state of the world.
        } else {
            // If another instance already exists, destroy this one.
            Destroy(gameObject);
        }
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Sets the initial state of all story flags at the start of the game.
    /// This is the place to define your game world's default conditions.
    /// </summary>
    private void InitializeFlags() {
        flags.Clear(); // Ensure we start with a clean slate.

        // --- DEFINE ALL YOUR STARTING FLAGS HERE ---
        flags["Default"] = true; // A flag that might be used for generic NPC dialogue.
        flags["InitialNaration"] = false;
        flags["Collect"] = false; // The quest to collect trash and food.
        flags["WeeksLater"] = false;
        flags["AfterFightTalk"] = false;
        flags["EvTalk"] = false;
        flags["BossDefeated"] = false;
        flags["SecretFound"] = false;
        // ...add any other flags your game needs to start with.
    }

    #endregion

    #region Public API

    /// <summary>
    /// Sets the value of a story flag and broadcasts an event about the change.
    /// </summary>
    /// <param name="key">The name of the flag to set (e.g., "Collect").</param>
    /// <param name="value">The new value (true or false).</param>
    public void SetFlag(string key, bool value) {
        // Optimization: If the flag already has the desired value, do nothing.
        // This prevents events from firing unnecessarily.
        if (flags.ContainsKey(key) && flags[key] == value) {
            return;
        }

        // 1. Update the dictionary with the new value.
        if (flags.ContainsKey(key)) {
            flags[key] = value;
        } else {
            flags.Add(key, value);
        }

        Debug.Log($"[StoryManager] Flag '{key}' set to '{value}'.");

        // 2. Broadcast the C# event for any code listeners.
        OnFlagChanged?.Invoke(key, value);

        // 3. Trigger any matching UnityEvents set up in the Inspector.
        TriggerInspectorEvent(key, value);
    }

    /// <summary>
    /// Checks the current value of a story flag.
    /// </summary>
    /// <param name="key">The name of the flag to check.</param>
    /// <returns>True if the flag exists and is true, otherwise false.</returns>
    public bool GetFlag(string key) {
        return flags.ContainsKey(key) && flags[key];
    }

    #endregion

    #region Private Helpers

    /// <summary>
    /// Finds and invokes the corresponding UnityEvent from the inspector list.
    /// </summary>
    private void TriggerInspectorEvent(string key, bool value) {
        foreach (var flagEvent in inspectorFlagEvents) {
            if (flagEvent.flagName == key) {
                if (value == true) {
                    flagEvent.OnSetTrue?.Invoke();
                } else {
                    flagEvent.OnSetFalse?.Invoke();
                }
                // We found our event, no need to keep searching.
                return;
            }
        }
    }

    #endregion
}