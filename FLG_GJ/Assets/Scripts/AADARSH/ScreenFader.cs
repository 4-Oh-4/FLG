using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// A reusable, general-purpose screen fader singleton.
/// Handles fade-to-black transitions for building entries, teleports, scene changes, etc.
/// Does NOT set any story flags — that's the caller's responsibility.
/// </summary>
public class ScreenFader : MonoBehaviour {
    #region Singleton
    public static ScreenFader Instance { get; private set; }
    #endregion

    [Header("UI References")]
    [Tooltip("The CanvasGroup on the full-screen black overlay panel.")]
    [SerializeField] private CanvasGroup fadeCanvasGroup;

    [Tooltip("Optional: TextMeshPro text element to display a message during the black screen.")]
    [SerializeField] private TextMeshProUGUI fadeText;

    [Header("Settings")]
    [Tooltip("Default duration of the fade-in and fade-out animations in seconds.")]
    [SerializeField] private float defaultFadeDuration = 0.4f;

    [Tooltip("How long to hold the black screen before fading back in.")]
    [SerializeField] private float defaultHoldDuration = 0.6f;

    /// <summary>
    /// True while any fade sequence is in progress. Prevents overlapping fades.
    /// </summary>
    public bool IsFading { get; private set; }

    private Coroutine activeFadeRoutine;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
            return;
        }

        // Start fully transparent and non-blocking.
        if (fadeCanvasGroup != null) {
            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.blocksRaycasts = false;
            fadeCanvasGroup.interactable = false;
        }

        if (fadeText != null) {
            fadeText.text = "";
        }
    }

    // ─── PUBLIC API ────────────────────────────────────────────────

    /// <summary>
    /// Performs a full fade sequence: fade to black → execute action → fade back in.
    /// </summary>
    /// <param name="onBlackScreen">Action to execute while the screen is black (e.g., teleport player). Can be null.</param>
    /// <param name="displayText">Optional text to show during the black screen (e.g., "Entering the warehouse..."). Pass null or empty for no text.</param>
    /// <param name="fadeDuration">Duration of each fade (in + out). Uses default if <= 0.</param>
    /// <param name="holdDuration">How long to hold the black screen. Uses default if <= 0.</param>
    public void FadeAndExecute(Action onBlackScreen, string displayText = null, float fadeDuration = -1f, float holdDuration = -1f) {
        if (IsFading) {
            Debug.LogWarning("[ScreenFader] A fade is already in progress. Ignoring new request.");
            return;
        }

        float fade = fadeDuration > 0 ? fadeDuration : defaultFadeDuration;
        float hold = holdDuration > 0 ? holdDuration : defaultHoldDuration;

        if (activeFadeRoutine != null) {
            StopCoroutine(activeFadeRoutine);
        }

        activeFadeRoutine = StartCoroutine(FadeSequence(onBlackScreen, displayText, fade, hold));
    }

    /// <summary>
    /// Simple fade to black and back, with no action in between.
    /// Useful for simple screen transitions.
    /// </summary>
    public void SimpleFade(string displayText = null) {
        FadeAndExecute(null, displayText);
    }

    /// <summary>
    /// Fade to black only (no automatic fade back). Call FadeIn() manually to bring it back.
    /// Useful for cutscene transitions where you control timing.
    /// </summary>
    public void FadeToBlack(float fadeDuration = -1f) {
        float fade = fadeDuration > 0 ? fadeDuration : defaultFadeDuration;

        if (activeFadeRoutine != null) {
            StopCoroutine(activeFadeRoutine);
        }

        activeFadeRoutine = StartCoroutine(FadeAlpha(0f, 1f, fade));
    }

    /// <summary>
    /// Fade back in from black. Counterpart to FadeToBlack().
    /// </summary>
    public void FadeFromBlack(float fadeDuration = -1f) {
        float fade = fadeDuration > 0 ? fadeDuration : defaultFadeDuration;

        if (activeFadeRoutine != null) {
            StopCoroutine(activeFadeRoutine);
        }

        activeFadeRoutine = StartCoroutine(FadeAlpha(1f, 0f, fade));
    }

    // ─── COROUTINES ────────────────────────────────────────────────

    private IEnumerator FadeSequence(Action onBlackScreen, string displayText, float fadeDuration, float holdDuration) {
        IsFading = true;

        // 1. Fade to black
        yield return FadeAlpha(0f, 1f, fadeDuration);

        // 2. Show optional text
        if (fadeText != null && !string.IsNullOrEmpty(displayText)) {
            fadeText.text = displayText;
        }

        // 3. Execute the action while screen is black
        onBlackScreen?.Invoke();

        // 4. Hold the black screen
        // Use WaitForSecondsRealtime so it works even when timeScale = 0
        yield return new WaitForSecondsRealtime(holdDuration);

        // 5. Clear text
        if (fadeText != null) {
            fadeText.text = "";
        }

        // 6. Fade back in
        yield return FadeAlpha(1f, 0f, fadeDuration);

        IsFading = false;
        activeFadeRoutine = null;
    }

    private IEnumerator FadeAlpha(float from, float to, float duration) {
        if (fadeCanvasGroup == null) yield break;

        fadeCanvasGroup.blocksRaycasts = true;
        fadeCanvasGroup.alpha = from;

        float elapsed = 0f;
        while (elapsed < duration) {
            elapsed += Time.unscaledDeltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        fadeCanvasGroup.alpha = to;

        // Only block raycasts while the screen is actually black
        fadeCanvasGroup.blocksRaycasts = (to > 0.5f);
    }
}
