using UnityEngine;
using System;
using System.Collections.Generic; // Needed for List

// This is our new data container. Because it has [System.Serializable],
// we can edit it in the Inspector. It doesn't inherit from MonoBehaviour.
[System.Serializable]
public class ConditionalDialogue {
    [Tooltip("The flag that MUST be true for this dialogue to trigger.")]
    public string requiredFlag;
    [Tooltip("The sequence of dialogue to play if the flag is true.")]
    public DialougeA[] dialogues;
    [Tooltip("OPTIONAL: The flag to set after this dialogue is complete.")]
    public string flagToSetOnComplete;
}


public class NPC_A : MonoBehaviour {
    [Header("Dialogue Logic")]
    [Tooltip("Dialogues that depend on a story flag. Checked from LAST to FIRST.")]
    [SerializeField] private List<ConditionalDialogue> conditionalDialogues;

    [Tooltip("The dialogue to play if no conditional flags are met.")]
    [SerializeField] private DialougeA[] defaultDialogues;
    [Tooltip("OPTIONAL: The flag to set after the default dialogue is complete.")]
    [SerializeField] private string defaultFlagToSet;


    [Header("Configuration")]
    [SerializeField] private bool isNarator = false;
    [SerializeField] private DialougeManagerA dialogueManager;

    private void Start() {
        if (dialogueManager == null) {
            dialogueManager = FindAnyObjectByType<DialougeManagerA>();
        }
    }

    private void LateUpdate() {
        if (isNarator) {
            TriggerDialouge();
            isNarator = false;
        }
    }

    public void TriggerDialouge() {
        if (StoryCheckpointA.IsRestoring) return; // replaying a save: dialogue already happened (its flags are replayed)
        // An NPC that has never been active (Act 3's police group) hasn't run Start() yet: find the manager now.
        if (dialogueManager == null) dialogueManager = FindAnyObjectByType<DialougeManagerA>();
        if (dialogueManager == null) {
            Debug.LogError("Dialogue Manager not found!");
            return;
        }
        if (StoryManagertAct1A.Instance == null) {
            Debug.Log("StoryManagerAct1A instance not found in the scene!");
            return;
        }
        // --- NEW DYNAMIC LOGIC ---

        // Iterate through our conditional dialogues in REVERSE order.
        // This makes the one at the bottom of the list in the Inspector the highest priority.
        for (int i = conditionalDialogues.Count - 1; i >= 0; i--) {
            var condition = conditionalDialogues[i];

            // If the required flag exists and is true...
            if (!string.IsNullOrEmpty(condition.requiredFlag) && StoryManagertAct1A.Instance.GetFlag(condition.requiredFlag)) {
                // ...then this is the dialogue we will play.
                Debug.Log($"Condition met for flag: {condition.requiredFlag}");
                PlayDialogueSequence(condition.dialogues, condition.flagToSetOnComplete);
                return; // Exit the function after starting the dialogue
            }
        }

        // If the loop finishes and we haven't returned, it means no conditions were met.
        // So, we play the default dialogue.
        Debug.Log("No conditions met, playing default dialogue.");
        PlayDialogueSequence(defaultDialogues, defaultFlagToSet);
    }

    /// <summary>
    /// True if TriggerDialouge() would do something right now (play lines or set a flag).
    /// Mirrors the selection logic in TriggerDialouge() without changing it.
    /// </summary>
    public bool HasInteraction() {
        if (StoryManagertAct1A.Instance == null) return false;
        for (int i = conditionalDialogues.Count - 1; i >= 0; i--) {
            var condition = conditionalDialogues[i];
            if (!string.IsNullOrEmpty(condition.requiredFlag) && StoryManagertAct1A.Instance.GetFlag(condition.requiredFlag)) {
                return (condition.dialogues != null && condition.dialogues.Length > 0) || !string.IsNullOrEmpty(condition.flagToSetOnComplete);
            }
        }
        return (defaultDialogues != null && defaultDialogues.Length > 0) || !string.IsNullOrEmpty(defaultFlagToSet);
    }

    /// <summary>True while this NPC's dialogue box is open.</summary>
    public bool IsDialogueOpen() {
        return dialogueManager != null && dialogueManager.IsOpen;
    }

    // Helper function to reduce code duplication
    private void PlayDialogueSequence(DialougeA[] dialoguesToPlay, string flagToSet) {
        Action onCompleteAction = () => {
            if (!string.IsNullOrEmpty(flagToSet)) {
                StoryManagertAct1A.Instance.SetFlag(flagToSet, true);
                Debug.Log($"Flag '{flagToSet}' was set to true.");
            }
        };
        dialogueManager.StartDialogueSequence(dialoguesToPlay, onCompleteAction);
    }
}