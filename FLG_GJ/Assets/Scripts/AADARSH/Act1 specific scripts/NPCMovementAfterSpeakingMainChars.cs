// NPCMovementAfterSpeakingMainChars.cs
using UnityEngine;
using System.Collections.Generic;

// Helper class to define a movement rule in the Inspector.
[System.Serializable]
public class MovementRule {
    [Tooltip("The story flag that must be true for the NPC to move.")]
    public string requiredFlag;
    [Tooltip("The Transform (empty GameObject) the NPC will move to when the flag is true.")]
    public Transform targetWaypoint;
}


// This script requires an NpcMotorA to function.
[RequireComponent(typeof(NpcMotorA))]
public class NPCMovementAfterSpeakingMainChars : MonoBehaviour {
    [Tooltip("The list of movement rules for this NPC. The LAST one in the list has the HIGHEST priority.")]
    [SerializeField] private List<MovementRule> movementRules;

    private NpcMotorA motor;

    private void Awake() {
        motor = GetComponent<NpcMotorA>();
    }

    // --- CHANGED ---
    // OnEnable is now ONLY for subscribing to the event.
    private void OnEnable() {
        StoryManagertAct1A.OnFlagChanged += HandleFlagChange;
    }

    // --- ADDED ---
    // Start is called after all Awakes are done, so StoryManager.Instance will be ready.
    private void Start() {
        // Check flags at the start of the game.
        CheckAllFlagsAndMove();
    }
    // -------------

    private void OnDisable() {
        StoryManagertAct1A.OnFlagChanged -= HandleFlagChange;
    }

    private void HandleFlagChange(string flagName, bool value) {
        if (value == true) {
            CheckAllFlagsAndMove();
        }
    }

    private void CheckAllFlagsAndMove() {
        // ... (This method remains unchanged) ...
        for (int i = movementRules.Count - 1; i >= 0; i--) {
            var rule = movementRules[i];

            // This line will now work because StoryManager.Instance is guaranteed to exist.
            if (StoryManagertAct1A.Instance.GetFlag(rule.requiredFlag)) {
                if (rule.targetWaypoint != null) {
                    Debug.Log($"NPC '{gameObject.name}' moving to '{rule.targetWaypoint.name}' because flag '{rule.requiredFlag}' is true.");
                    motor.MoveTo(rule.targetWaypoint.position);
                    return;
                }
            }
        }
    }
}