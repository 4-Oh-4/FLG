using UnityEngine;

public class SimpleFadeIn_K : MonoBehaviour
{
    public CanvasGroup fadeGroup;
    public float firstPhaseDuration = 8f;  // fade black → mostly visible
    public float secondPhaseDuration = 1f; // mostly visible → fully visible
    public float targetAlphaPhase1 = 0.1f; // how transparent after first fade

    private void Start()
    {
        StartCoroutine(FadeSequence());
    }

    private System.Collections.IEnumerator FadeSequence()
    {
        // Start fully black
        fadeGroup.alpha = 1f;

        // Phase 1: 1 → targetAlphaPhase1
        float t = 0f;
        while (t < firstPhaseDuration)
        {
            t += Time.deltaTime;
            fadeGroup.alpha = Mathf.Lerp(1f, targetAlphaPhase1, t / firstPhaseDuration);
            yield return null;
        }
        fadeGroup.alpha = targetAlphaPhase1;

        // Phase 2: targetAlphaPhase1 → 0
        t = 0f;
        while (t < secondPhaseDuration)
        {
            t += Time.deltaTime;
            fadeGroup.alpha = Mathf.Lerp(targetAlphaPhase1, 0f, t / secondPhaseDuration);
            yield return null;
        }
        fadeGroup.alpha = 0f; // fully visible
    }
}
