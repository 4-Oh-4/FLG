using System.Collections;
using UnityEngine;
using TMPro; // Add this namespace to use TextMeshPro elements

public class ShowingStoryUpdates1 : MonoBehaviour {
    [Header("UI References")]
    [Tooltip("The parent GameObject of the story update UI (e.g., a Panel with an Image and Text).")]
    [SerializeField] private GameObject storyUpdatePanel;

    [Tooltip("The TextMeshPro UI element that will display the update message.")]
    [SerializeField] private TextMeshProUGUI updateText;

    [Header("Configuration")]
    [Tooltip("How long the update will be displayed on screen, in seconds.")]
    [SerializeField] private float displayDuration = 4f;

    // A variable to hold a reference to the running coroutine.
    // This allows us to stop it if a new update comes in.
    private Coroutine displayCoroutine;

    // We can hide the panel at the start to ensure it's not visible.
    void Start() {
        // Safety check to prevent errors if the panel isn't assigned.
        if (storyUpdatePanel == null) {
            Debug.LogError("Story Update Panel is not assigned in the Inspector!", this.gameObject);
            return;
        }

        storyUpdatePanel.SetActive(false);
    }

    /// <summary>
    /// This is the public function you will call from other scripts to show a new story update.
    /// </summary>
    /// <param name="message">The text you want to display in the update.</param>
    public void ShowUpdate(string message) {
        // If an update is already being shown, stop that one first.
        // This ensures the new update gets the full display time.
        if (displayCoroutine != null) {
            StopCoroutine(displayCoroutine);
        }

        // Start the new coroutine and store a reference to it.
        displayCoroutine = StartCoroutine(ShowAndWaitRoutine(message));
    }

    /// <summary>
    /// A coroutine that handles the sequence: show UI, wait, and then hide UI.
    /// </summary>
    private IEnumerator ShowAndWaitRoutine(string message) {
        if(message=="")yield return new WaitForSeconds(0.5f);
        // 1. Set the text message.
        if (updateText != null) {
            //updateText.text = message;
        }

        // 2. Enable the panel GameObject to make it visible.
        storyUpdatePanel.SetActive(true);

        // 3. Wait for the specified duration. The 'yield' keyword pauses the function here.
        yield return new WaitForSeconds(displayDuration);
        StoryManagertAct1A.Instance.SetFlag("WeeksLater",true);
        // 4. After the wait is over, disable the panel GameObject to hide it.
        storyUpdatePanel.SetActive(false);
    }
}