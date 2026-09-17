using UnityEngine;
using System;

/// <summary>
/// Defines a single step in a waypoint chain.
/// </summary>
[Serializable]
public class WaypointStep {
    [Tooltip("The Transform target for this step (e.g., a door, then an NPC).")]
    public Transform target;

    [Tooltip("The mission text to display for this step.")]
    [TextArea(1, 3)]
    public string missionText;

    [Tooltip("How close the player must be to this target (in units) to auto-advance to the next step.")]
    public float arrivalRadius = 3f;
}

/// <summary>
/// A multi-step waypoint chain. Instead of pointing directly at a final target 
/// (e.g., an NPC inside a building), it guides the player through intermediate 
/// steps (e.g., building door first, then the NPC).
///
/// Usage: 
///   1. Call SetChain() with an array of WaypointSteps.
///   2. The chain auto-advances when the player gets close to each step.
///   3. When the last step is reached, it stays on that target until ClearChain() is called.
/// </summary>
public class WaypointChain : MonoBehaviour {
    [Header("References")]
    [Tooltip("The quest pointer that displays the waypoint arrow.")]
    [SerializeField] private WindowQuestPointer_A questPointer;

    [Tooltip("The quest text display on the HUD.")]
    [SerializeField] private QuestTextA questText;

    [Tooltip("The player's Transform for distance checking.")]
    [SerializeField] private Transform player;

    [Header("Chain State")]
    [SerializeField] private WaypointStep[] currentChain;
    private int currentStepIndex = 0;
    private bool isChainActive = false;

    private void Update() {
        if (!isChainActive || currentChain == null || currentChain.Length == 0) return;
        if (player == null) return;

        // Check if player is close enough to the current step's target to advance
        WaypointStep currentStep = currentChain[currentStepIndex];
        if (currentStep.target == null) return;

        float distance = Vector2.Distance(player.position, currentStep.target.position);
        if (distance <= currentStep.arrivalRadius) {
            AdvanceToNextStep();
        }
    }

    /// <summary>
    /// Sets a new waypoint chain. The pointer will guide the player through each step in order.
    /// </summary>
    /// <param name="steps">Array of waypoint steps to follow.</param>
    public void SetChain(WaypointStep[] steps) {
        if (steps == null || steps.Length == 0) {
            Debug.LogWarning("[WaypointChain] Tried to set an empty chain.");
            return;
        }

        currentChain = steps;
        currentStepIndex = 0;
        isChainActive = true;
        ApplyCurrentStep();
    }

    /// <summary>
    /// Sets a simple single-target waypoint (no chain, just a direct pointer).
    /// Convenience method for backwards compatibility with existing code.
    /// </summary>
    public void SetSingleTarget(Transform target, string missionText) {
        WaypointStep step = new WaypointStep {
            target = target,
            missionText = missionText,
            arrivalRadius = 999f // Effectively never auto-advances
        };
        SetChain(new WaypointStep[] { step });
    }

    /// <summary>
    /// Clears the current chain and hides the waypoint.
    /// </summary>
    public void ClearChain() {
        currentChain = null;
        currentStepIndex = 0;
        isChainActive = false;

        if (questPointer != null) {
            questPointer.target = null;
        }
        if (questText != null) {
            questText.clearQuest();
        }
    }

    /// <summary>
    /// Manually force advance to the next step (useful for script-driven events 
    /// like entering a building via trigger).
    /// </summary>
    public void ForceAdvance() {
        AdvanceToNextStep();
    }

    /// <summary>
    /// Returns true if the chain is on its final step.
    /// </summary>
    public bool IsOnFinalStep() {
        return currentChain != null && currentStepIndex >= currentChain.Length - 1;
    }

    private void AdvanceToNextStep() {
        if (currentChain == null) return;

        // If we're not on the last step, move to the next one
        if (currentStepIndex < currentChain.Length - 1) {
            currentStepIndex++;
            ApplyCurrentStep();
            Debug.Log($"[WaypointChain] Advanced to step {currentStepIndex}: {currentChain[currentStepIndex].missionText}");
        }
        // If we ARE on the last step, just stay — don't clear (the quest system will clear when appropriate)
    }

    private void ApplyCurrentStep() {
        if (currentChain == null || currentStepIndex >= currentChain.Length) return;

        WaypointStep step = currentChain[currentStepIndex];

        if (questPointer != null) {
            if (!questPointer.gameObject.activeInHierarchy) {
                questPointer.gameObject.SetActive(true);
            }
            questPointer.target = step.target;
        }

        if (questText != null && !string.IsNullOrEmpty(step.missionText)) {
            questText.setQuest(step.missionText);
        }
    }
}
