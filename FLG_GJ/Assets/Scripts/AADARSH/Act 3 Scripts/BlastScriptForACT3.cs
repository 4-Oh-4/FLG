using UnityEngine;
using System.Collections; // Required for using Coroutines

public class BlastScriptForACT3 : MonoBehaviour {
    // --- Private Variables ---
    // A private reference to the Animator component on this GameObject.
    private Animator animator;
    [SerializeField] GameObject fences;
    [SerializeField] Sprite sprite;
    // --- Initialization ---
    // Awake is called when the script instance is being loaded.
    void Awake() {
        sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
        // Get the Animator component attached to this same GameObject.
        animator = GetComponent<Animator>();

        // Check if an Animator component was actually found. This is important for debugging.
        if (animator == null) {
            Debug.LogError("BlastScriptForACT3 requires an Animator component on the same GameObject, but none was found!", this.gameObject);
            return; // Stop if no animator is present to prevent further errors.
        }

        // IMPORTANT: Ensure the animator is disabled by default.
        // It will be enabled only when the blast is triggered.
        animator.enabled = false;
    }

    // --- Public Function to be called by Unity Events ---
    /// <summary>
    /// This function can be called by a Unity Event. It starts the timed blast sequence.
    /// </summary>
    public void TriggerBlast() {
        // Make sure we have a valid animator before starting the coroutine.
        if (animator != null) {
            // Start the coroutine that handles the delay.
            StartCoroutine(BlastCoroutine());
        } else {
            Debug.LogWarning("TriggerBlast was called, but no Animator is available.", this.gameObject);
        }
    }

    // --- Coroutine for the Timed Blast ---
    /// <summary>
    /// This coroutine waits for a specified time and then enables the animator.
    /// </summary>
    private IEnumerator BlastCoroutine() {
        yield return new WaitForSeconds(2f);

        // 1. Wait for 1 second. The code pauses here.
        fences.SetActive(false);
        // 2. After 1 second, enable the animator.
        // The animation clip set as the default state in the Animator will start playing.
        Debug.Log("Enabling blast animation on " + gameObject.name);
        animator.enabled = true;
        yield return new WaitForSeconds(0.3f);
        StoryManagertAct1A.Instance.SetFlag("PoliceComing",true);
        gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
        yield return new WaitForSeconds(1f);
        StoryManagertAct1A.Instance.SetFlag("RunToCentralBB", true);
    }
}

