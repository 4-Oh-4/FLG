using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Follows the on-screen objective (QuestTextA) without changing how it is set.
///  - When the objective changes, the HUD text pulses and briefly turns the accent colour, so the
///    player notices there is something new to do.
///  - It keeps this chapter's objectives, shown in the pause menu: the current one and the ones done.
/// During a checkpoint restore the objectives are re-recorded silently (no pulses).
/// </summary>
public class ObjectiveTrackerA : MonoBehaviour {
    private const string Prefix = "Objective:";
    private const int MaxDone = 6;
    private static readonly Color Accent = new Color(1f, 0.82f, 0.35f);

    private static readonly List<string> done = new List<string>();
    private static string current;

    /// <summary>Current objective without the "Objective:" prefix (empty if none).</summary>
    public static string Current => current ?? "";

    /// <summary>Objectives finished in this chapter, oldest first.</summary>
    public static IReadOnlyList<string> Done => done;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init() {
        done.Clear();
        current = null;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // A new chapter starts a fresh list (minigames and interiors load additively and keep it).
    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (mode != LoadSceneMode.Single) return;
        done.Clear();
        current = null;
    }

    /// <summary>Called by QuestTextA.setQuest after it has set the text.</summary>
    public static void OnObjectiveSet(TMP_Text label) {
        if (label == null) return;
        string text = Clean(label.text);
        if (text == current) return;
        if (!string.IsNullOrEmpty(current)) {
            done.Remove(current);
            done.Add(current);
            if (done.Count > MaxDone) done.RemoveAt(0);
        }
        current = text;
        if (string.IsNullOrEmpty(text) || StoryCheckpointA.IsRestoring || StoryCheckpointA.ContinuingFromCheckpoint) return;
        if (!label.isActiveAndEnabled) return;
        ObjectiveTrackerA pulse = label.GetComponent<ObjectiveTrackerA>();
        if (pulse == null) pulse = label.gameObject.AddComponent<ObjectiveTrackerA>();
        pulse.Pulse(label);
    }

    private static string Clean(string text) {
        if (string.IsNullOrEmpty(text)) return "";
        text = text.Trim();
        if (text.StartsWith(Prefix)) text = text.Substring(Prefix.Length).Trim();
        return text;
    }

    /// <summary>Pause-menu text: current objective and the ones done (rich text).</summary>
    public static string Describe() {
        var sb = new StringBuilder();
        sb.Append(string.IsNullOrEmpty(current) ? "<color=#9a9a9a>Nothing right now.</color>" : current);
        if (done.Count > 0) {
            sb.Append("\n\n<size=80%><color=#bdb3a1>DONE</color></size>");
            for (int i = done.Count - 1; i >= 0; i--) sb.Append("\n<size=80%><color=#d2c9b8><s>").Append(done[i]).Append("</s></color></size>");
        }
        return sb.ToString();
    }

    // --- HUD pulse (this component sits on the objective label) ---

    private TMP_Text label;
    private Color baseColor;
    private Vector3 baseScale;
    private Coroutine running;

    private void Pulse(TMP_Text target) {
        if (running != null) {
            StopCoroutine(running);
            label.color = baseColor;
            label.rectTransform.localScale = baseScale;
        } else {
            label = target;
            baseColor = target.color;
            baseScale = target.rectTransform.localScale;
        }
        running = StartCoroutine(PulseRoutine());
    }

    private IEnumerator PulseRoutine() {
        const float duration = 1.4f;
        for (float t = 0f; t < 1f; t += Time.unscaledDeltaTime / duration) {
            float pop = t < 0.15f ? t / 0.15f : 1f - (t - 0.15f) / 0.85f; // quick rise, slow settle
            label.rectTransform.localScale = baseScale * (1f + 0.12f * Mathf.Clamp01(pop));
            label.color = Color.Lerp(baseColor, Accent, Mathf.Clamp01(pop * 1.2f));
            yield return null;
        }
        label.rectTransform.localScale = baseScale;
        label.color = baseColor;
        running = null;
    }

    private void OnDisable() {
        if (running == null) return;
        StopCoroutine(running);
        running = null;
        label.rectTransform.localScale = baseScale;
        label.color = baseColor;
    }
}
