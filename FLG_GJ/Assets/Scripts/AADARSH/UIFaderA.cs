using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the fading in and out of a UI element using a CanvasGroup.
/// </summary>
public class UIFader : MonoBehaviour {
    [Header("UI References")]
    [Tooltip("The CanvasGroup of the UI element you want to fade. The Image should be a child of this.")]
    public CanvasGroup uiCanvasGroup;

    [Tooltip("The GameObject containing the entire canvas to be enabled/disabled.")]
    public GameObject canvasGameObject;

    [Tooltip("The button that will trigger the fade-out effect.")]
    public Button fadeOutButton;

    // Assuming PlayerMovement_A is a custom script you have in your project.
    [Tooltip("Reference to the player movement script to disable/enable movement.")]
    public PlayerMovement_A playerMovement;

    [Header("Fade Settings")]
    [Tooltip("How long, in seconds, the fade-in and fade-out animations should take.")]
    public float fadeDuration = 1.0f;
    public bool onStart = false;
    private Coroutine _activeFadeCoroutine;

    /// <summary>
    /// Unity's built-in method, called once when the script instance is being loaded.
    /// </summary>
    void Start() {
        // Ensure the UI is initially invisible and not interactable.
        if (uiCanvasGroup != null) {
            uiCanvasGroup.alpha = 0f;
            uiCanvasGroup.interactable = false;
            uiCanvasGroup.blocksRaycasts = false;
        } else {
            Debug.LogError("UI Canvas Group is not assigned in the inspector!");
            return;
        }

        // Add a listener to the button's onClick event.
        // When the button is clicked, it will call the FadeOut method.
        if (fadeOutButton != null) {
            fadeOutButton.onClick.AddListener(FadeOut);
        } else {
            Debug.LogError("Fade Out Button is not assigned in the inspector!");
        }

        // Start with the entire canvas disabled.
        if (canvasGameObject != null) {
            canvasGameObject.SetActive(false);
        }
        if (onStart) FadeIn();
    }

    /// <summary>
    /// Added for convenience to allow closing the UI with the spacebar.
    /// </summary>
    void Update() {
        // If the canvas is visible and the user presses Space, fade out.
        if (uiCanvasGroup != null && uiCanvasGroup.alpha > 0 && Input.GetKeyDown(KeyCode.N)) {
            FadeOut();
        }
    }

    /// <summary>
    /// Public method to start the fade-in process.
    /// Can be called from other scripts.
    /// </summary>
    public void FadeIn() {
        // Enable the canvas GameObject before fading in.
        if (canvasGameObject != null) {
            canvasGameObject.SetActive(true);
        }

        // Make the UI element visible and interactable before fading.
        if (uiCanvasGroup != null) {
            // Disable player movement when UI is active.
            if (playerMovement != null) {
                playerMovement.canMove = false;
            }
            uiCanvasGroup.interactable = true;
            uiCanvasGroup.blocksRaycasts = true;
            StartFade(1f);
        }
    }

    /// <summary>
    /// Public method to start the fade-out process.
    /// Called by the fadeOutButton.
    /// </summary>
    public void FadeOut() {
        // Make the UI element non-interactable while fading out.
        if (uiCanvasGroup != null) {
            // Re-enable player movement.
            if (playerMovement != null) {
                playerMovement.canMove = true;
            }
            uiCanvasGroup.interactable = false;
            uiCanvasGroup.blocksRaycasts = false;
            StartFade(0f);
        }
    }

    /// <summary>
    /// Manages starting a new fade coroutine, ensuring only one is active at a time.
    /// </summary>
    /// <param name="targetAlpha">The target alpha value (0 for transparent, 1 for opaque).</param>
    private void StartFade(float targetAlpha) {
        // If a fade is already running, stop it.
        if (_activeFadeCoroutine != null) {
            StopCoroutine(_activeFadeCoroutine);
        }
        // Start a new fade coroutine.
        _activeFadeCoroutine = StartCoroutine(FadeCanvasGroup(targetAlpha));
    }


    /// <summary>
    /// A coroutine that gradually changes the alpha of the CanvasGroup over time.
    /// </summary>
    /// <param name="targetAlpha">The final alpha value (0 for transparent, 1 for opaque).</param>
    private IEnumerator FadeCanvasGroup(float targetAlpha) {
        float currentTime = 0;
        float startAlpha = uiCanvasGroup.alpha;

        // Loop until the fade duration is reached.
        while (currentTime < fadeDuration) {
            // Increment time.
            currentTime += Time.deltaTime;

            // Calculate the new alpha value using Lerp for a smooth transition.
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, currentTime / fadeDuration);

            // Apply the new alpha to the CanvasGroup.
            uiCanvasGroup.alpha = newAlpha;

            // Wait for the next frame before continuing the loop.
            yield return null;
        }

        // Ensure the final alpha is set correctly.
        uiCanvasGroup.alpha = targetAlpha;
        _activeFadeCoroutine = null;

        // If we just faded out completely, disable the entire canvas GameObject.
        if (targetAlpha == 0f && canvasGameObject != null) {
            canvasGameObject.SetActive(false);
        }
    }
}
