using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// After Continue on the title screen, shows a short banner once the game is back:
/// the chapter and where the player left off ("Where you left off: Save Silvio.").
/// It waits for the checkpoint restore, newspaper pages and time-skip cards to finish first,
/// and never blocks input.
/// </summary>
public class ContinueRecapA : MonoBehaviour {
    private const string PrefabResource = "ContinueRecap";
    private const float Hold = 3.5f;

    [SerializeField] private CanvasGroup group;
    [SerializeField] private TMP_Text chapter;
    [SerializeField] private TMP_Text objective;

    private static bool requested;
    private static string chapterLabel;
    private static ContinueRecapA instance;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init() {
        requested = false;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <summary>Called by the title screen's Continue, just before it loads the saved chapter.</summary>
    public static void Request(string label) {
        requested = true;
        chapterLabel = label;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (mode != LoadSceneMode.Single || !requested) return;
        requested = false;
        if (instance == null) {
            GameObject prefab = Resources.Load<GameObject>(PrefabResource);
            if (prefab == null) return;
            GameObject go = Instantiate(prefab);
            go.name = PrefabResource;
            DontDestroyOnLoad(go);
            instance = go.GetComponent<ContinueRecapA>();
        }
        instance.StopAllCoroutines();
        instance.group.alpha = 0f;
        instance.StartCoroutine(instance.Show(scene.handle));
    }

    private IEnumerator Show(int sceneHandle) {
        // Let the restore and any start-of-chapter pages play out first.
        while (StoryCheckpointA.ContinuingFromCheckpoint || StoryCheckpointA.IsRestoring) yield return null;
        yield return new WaitForSecondsRealtime(0.8f);
        while (StoryDocumentA.IsOpen || TimeSkipCardA.Showing || PauseMenuManager.IsPaused) yield return null;
        if (SceneManager.GetActiveScene().handle != sceneHandle) yield break;

        chapter.text = string.IsNullOrEmpty(chapterLabel) ? "Continue" : chapterLabel.ToUpperInvariant();
        string where = ObjectiveTrackerA.Current;
        objective.text = string.IsNullOrEmpty(where) ? "" : "Where you left off: " + where;

        for (float t = 0f; t < 1f; t += Time.unscaledDeltaTime / 0.4f) { group.alpha = t; yield return null; }
        group.alpha = 1f;
        yield return new WaitForSecondsRealtime(Hold);
        for (float t = 1f; t > 0f; t -= Time.unscaledDeltaTime / 0.6f) { group.alpha = t; yield return null; }
        group.alpha = 0f;
    }
}
