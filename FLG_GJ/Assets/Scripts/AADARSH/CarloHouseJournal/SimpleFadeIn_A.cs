using TMPro;
using UnityEngine;
using System.Collections; //  ADDED: Needed for IEnumerator

public class SimpleFadeIn_A : MonoBehaviour {
    public CanvasGroup fadeGroup;
    public float firstPhaseDuration = 8f;  // fade black → mostly visible
    public float secondPhaseDuration = 1f; // mostly visible fully visible
    public float targetAlphaPhase1 = 0.1f; // how transparent after first fade
    public TextMeshProUGUI left;
    public TextMeshProUGUI right;
    public GameObject journal;
    public PlayerMovement_A playerMovement;
    string l;
    string r;
    // --- NEW VARIABLE ---
    [Tooltip("The speed at which letters appear, in letters per second.")]
    [Range(1f, 100f)]
    public float lettersPerSecond = 20f;

    private void Start() {
        l = left.text;
        left.text = "";
        r = right.text;
        right.text = "";
        journal.SetActive(false);
    }
    public void StartJournals() {
        journal.SetActive(true);
        StartCoroutine(FadeSequence());
    }
    private IEnumerator FadeSequence() {
        playerMovement.canMove = false;
        //  FADE IN LOGIC (Unchanged) 
        // Start fully black
        fadeGroup.alpha = 1f;

        // Phase 1: 1 → targetAlphaPhase1
        float t = 0f;
        while (t < firstPhaseDuration) {
            t += Time.deltaTime;
            fadeGroup.alpha = Mathf.Lerp(1f, targetAlphaPhase1, t / firstPhaseDuration);
            yield return null;
        }
        fadeGroup.alpha = targetAlphaPhase1;

        // Phase 2: targetAlphaPhase1  0
        t = 0f;
        while (t < secondPhaseDuration) {
            t += Time.deltaTime;
            fadeGroup.alpha = Mathf.Lerp(targetAlphaPhase1, 0f, t / secondPhaseDuration);
            yield return null;
        }
        fadeGroup.alpha = 0f; // fully visible

        //  NEW TYPEWRITER LOGIC
        // After fading is complete, start the typewriter effect.
        // The yield return ensures we wait for the left text to finish before starting the right.

        if (left != null) {
            yield return StartCoroutine(TypewriterEffect(left, l));
        }

        if (right != null) {
            yield return StartCoroutine(TypewriterEffect(right, r));
        }
    }

    // NEW COROUTINE
    private IEnumerator TypewriterEffect(TextMeshProUGUI textElement, string s) {
        // 1. Fetch the original text and clear the UI element
        string originalText = s;
        textElement.text = "";

        // 2. Calculate the delay between each letter
        float delay = 1f / lettersPerSecond;

        // 3. Iterate through each character of the original string
        foreach (char letter in originalText) {
            // Add one letter to the UI element
            textElement.text += letter;
            // Wait for the calculated delay
            yield return new WaitForSeconds(delay);
        }
    }
}