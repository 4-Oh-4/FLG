using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System;

/// <summary>
/// A reusable "Press E to Enter" door interaction component.
/// When the player enters the trigger zone, a UI prompt appears.
/// When they press E, it triggers a fade-to-black transition and executes the configured action.
///
/// SETUP:
/// 1. Add this to a GameObject with a 2D trigger collider at the door location.
/// 2. Set the player's tag to "Player".
/// 3. Assign the prompt UI elements in the Inspector.
/// 4. Configure the action (teleport, scene load, etc.) via the onInteract UnityEvent or by overriding DoInteraction().
/// </summary>
public class DoorInteraction : MonoBehaviour {
    [Header("Interaction Settings")]
    [Tooltip("The key to press to interact. Default is E.")]
    [SerializeField] private Key interactKey = Key.E;

    [Tooltip("Optional: A story flag that must be true for this door to be usable. Leave empty for always-available doors.")]
    [SerializeField] private string requiredFlag = "";

    [Tooltip("Text to show on the prompt (e.g., 'Press E to Enter').")]
    [SerializeField] private string promptText = "Press E to Enter";

    [Tooltip("Text to show during the fade transition (e.g., 'Entering the building...').")]
    [SerializeField] private string transitionText = "";

    [Header("UI References")]
    [Tooltip("The prompt UI GameObject to show/hide. Should be a child of this object or a world-space canvas.")]
    [SerializeField] private GameObject promptUI;

    [Tooltip("Optional: The TextMeshPro text on the prompt to set dynamically.")]
    [SerializeField] private TextMeshProUGUI promptLabel;

    [Header("Action")]
    [Tooltip("The UnityEvent to fire when the player interacts. Use this to wire up teleports, scene loads, etc.")]
    [SerializeField] private UnityEngine.Events.UnityEvent onInteract;

    private bool playerInRange = false;
    private bool hasInteracted = false;

    [Header("Reusability")]
    [Tooltip("If true, this door can be used multiple times. If false, it can only be used once.")]
    [SerializeField] private bool isReusable = true;

    private void Start() {
        // Hide prompt at start
        if (promptUI != null) {
            promptUI.SetActive(false);
        }

        // Set the prompt text if a label is assigned
        if (promptLabel != null) {
            promptLabel.text = promptText;
        }
    }

    private void Update() {
        if (!playerInRange || hasInteracted) return;

        // Check for key press
        if (Keyboard.current != null && Keyboard.current[interactKey].wasPressedThisFrame) {
            Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (!collision.CompareTag("Player")) return;
        if (hasInteracted && !isReusable) return;

        // Check if this door requires a story flag
        if (!string.IsNullOrEmpty(requiredFlag)) {
            if (StoryManagertAct1A.Instance == null || !StoryManagertAct1A.Instance.GetFlag(requiredFlag)) {
                return; // Door is locked — don't show prompt
            }
        }

        playerInRange = true;
        if (promptUI != null) {
            promptUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (!collision.CompareTag("Player")) return;

        playerInRange = false;
        if (promptUI != null) {
            promptUI.SetActive(false);
        }
    }

    private void Interact() {
        if (!isReusable) {
            hasInteracted = true;
        }

        // Hide prompt immediately
        if (promptUI != null) {
            promptUI.SetActive(false);
        }

        // If ScreenFader exists, do a proper fade transition
        if (ScreenFader.Instance != null) {
            ScreenFader.Instance.FadeAndExecute(
                onBlackScreen: () => {
                    DoInteraction();
                },
                displayText: transitionText
            );
        } else {
            // Fallback: just do the interaction immediately
            DoInteraction();
        }
    }

    /// <summary>
    /// Override this in subclasses for custom behavior, or just use the onInteract UnityEvent.
    /// </summary>
    protected virtual void DoInteraction() {
        onInteract?.Invoke();
    }

    /// <summary>
    /// Allows other scripts to unlock/lock this door at runtime.
    /// </summary>
    public void SetRequiredFlag(string flagName) {
        requiredFlag = flagName;
    }

    /// <summary>
    /// Reset the door so it can be used again (even if isReusable is false).
    /// </summary>
    public void ResetDoor() {
        hasInteracted = false;
    }

    // Draw the trigger area in the editor for easy visualization
    private void OnDrawGizmosSelected() {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) {
            Gizmos.color = new Color(0f, 1f, 0.5f, 0.3f);
            Gizmos.DrawCube(col.bounds.center, col.bounds.size);
        }
    }
}
