using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Displays the current quest/mission objective text on the HUD.
/// Supports a subtle slide-in animation when the quest text changes.
/// </summary>
public class QuestTextA : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI questtext;

    [Header("Animation")]
    [Tooltip("How long the slide-in animation takes.")]
    [SerializeField] float animationDuration = 0.3f;
    [Tooltip("How far the text slides in from (in pixels).")]
    [SerializeField] float slideDistance = 30f;

    private Coroutine animCoroutine;
    private RectTransform rectTransform;
    private Vector2 originalPosition;

    private void Start() {
        if (questtext != null) {
            rectTransform = questtext.GetComponent<RectTransform>();
            originalPosition = rectTransform.anchoredPosition;
        }
    }

    /// <summary>
    /// Sets the quest text with a slide-in animation.
    /// </summary>
    public void setQuest(string text) {
        if (questtext == null) return;

        questtext.text = text;

        // Play slide-in animation
        if (rectTransform != null && gameObject.activeInHierarchy) {
            if (animCoroutine != null) StopCoroutine(animCoroutine);
            animCoroutine = StartCoroutine(SlideInAnimation());
        }
    }

    /// <summary>
    /// Clears the quest text from the HUD.
    /// </summary>
    public void clearQuest() {
        if (questtext != null) {
            questtext.text = "";
        }
    }

    private IEnumerator SlideInAnimation() {
        // Start slightly offset and transparent
        Color startColor = questtext.color;
        startColor.a = 0f;
        questtext.color = startColor;
        rectTransform.anchoredPosition = originalPosition + new Vector2(slideDistance, 0);

        float elapsed = 0f;
        while (elapsed < animationDuration) {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / animationDuration);

            // Slide from right to original position
            rectTransform.anchoredPosition = Vector2.Lerp(
                originalPosition + new Vector2(slideDistance, 0),
                originalPosition,
                t
            );

            // Fade in
            Color c = questtext.color;
            c.a = t;
            questtext.color = c;

            yield return null;
        }

        // Snap to final values
        rectTransform.anchoredPosition = originalPosition;
        Color finalColor = questtext.color;
        finalColor.a = 1f;
        questtext.color = finalColor;

        animCoroutine = null;
    }
}
