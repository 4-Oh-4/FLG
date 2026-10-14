using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Dresses up the story black screens (ShowingStoryUpdates1) without changing them.
///
/// The original panel still turns on and off at the same moments and still sets WeeksLater /
/// BlackScreen{i}. On top of it this draws its own black card: when the black screen was caused by a
/// time skip, a caption fades in ("One month later."), and when the panel turns off the scene fades
/// back in instead of popping. The caption is chosen by the story flag whose event started the black
/// screen (flag events run in the same frame as the black screen they trigger). Black screens with
/// no caption (teleports, minigames) just get the fade.
///
/// A few time skips in the story have no black screen at all; those get a short card of their own.
/// </summary>
public class TimeSkipCardA : MonoBehaviour {
    private const string PrefabResource = "TimeSkipCard";
    private const float CaptionFadeIn = 0.45f;
    private const float FadeOut = 0.6f;
    private const float StandaloneHold = 1.8f;

    // Flag that started the black screen -> caption.
    private static readonly Dictionary<string, string> captions = new Dictionary<string, string> {
        // Act 1
        { "Collect", "One month later." },
        { "SilivioAngry", "That night." },
        { "WeekComplete", "Weeks later." },
        { "GoingToBeEvangalic", "Weeks of service later." },
        // Act 2
        { "TimeSkipToEvening", "That evening." },
        { "TimeSkipToEveningBB", "Midnight." },
        { "TopDownShooterDrugsMiniGame", "Outside the walls." },
        { "NinoTalkDone", "Months later." },
        { "NinoTalkOver", "That night." },
        // Act 3
        { "FewHoursLater", "A few hours later." },
    };

    // Time skips that happen without a black screen -> a short card of their own.
    private static readonly Dictionary<string, string> standaloneCards = new Dictionary<string, string> {
        { "MeetAfterAWeek", "One week later." },
    };

    [SerializeField] private CanvasGroup group;
    [SerializeField] private TMP_Text caption;

    private static TimeSkipCardA instance;
    private static string lastFlag;
    private static int lastFlagFrame = -1;
    private Coroutine running;

    /// <summary>True while a card is covering the screen.</summary>
    public static bool Showing => instance != null && instance.group != null && instance.group.alpha > 0f;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init() {
        lastFlag = null;
        lastFlagFrame = -1;
        StoryManagertAct1A.OnFlagChanged -= OnFlagChanged;
        StoryManagertAct1A.OnFlagChanged += OnFlagChanged;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnFlagChanged(string key, bool value) {
        if (!value) return;
        lastFlag = key;
        lastFlagFrame = Time.frameCount;
        if (standaloneCards.TryGetValue(key, out string text) && !StoryCheckpointA.IsRestoring && Get() != null) {
            instance.Play(null, text);
        }
    }

    // A new chapter scene never starts under an old card.
    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (mode == LoadSceneMode.Single && instance != null) instance.Hide();
    }

    /// <summary>Called by ShowingStoryUpdates1.ShowUpdate with the panel it just turned on.</summary>
    public static void OnBlackScreen(GameObject panel) {
        string text = null;
        if (lastFlagFrame == Time.frameCount && lastFlag != null) captions.TryGetValue(lastFlag, out text);
        if (Get() != null) instance.Play(panel, text);
    }

    private static TimeSkipCardA Get() {
        if (instance != null) return instance;
        GameObject prefab = Resources.Load<GameObject>(PrefabResource);
        if (prefab == null) return null;
        GameObject go = Instantiate(prefab);
        go.name = PrefabResource;
        DontDestroyOnLoad(go);
        instance = go.GetComponent<TimeSkipCardA>();
        instance.Hide();
        return instance;
    }

    private void Play(GameObject panel, string text) {
        if (running != null) StopCoroutine(running);
        running = StartCoroutine(panel != null ? FollowPanel(panel, text) : Standalone(text));
    }

    private void Hide() {
        if (running != null) StopCoroutine(running);
        running = null;
        group.alpha = 0f;
        caption.alpha = 0f;
        group.blocksRaycasts = false;
    }

    // Black immediately (the caller may teleport the player this same frame), caption fades in,
    // and the scene fades back in once the original panel turns off.
    private IEnumerator FollowPanel(GameObject panel, string text) {
        group.alpha = 1f;
        group.blocksRaycasts = true;
        yield return ShowCaption(text);
        // ShowUpdate("") turns the panel on half a second late; wait for it before following it.
        for (float t = 0f; t < 1f && panel != null && !panel.activeInHierarchy; t += Time.unscaledDeltaTime) yield return null;
        while (panel != null && panel.activeInHierarchy) yield return null;
        yield return FadeAway();
    }

    private IEnumerator Standalone(string text) {
        PlayerMovement_A player = FindPlayer();
        bool couldMove = player != null && player.canMove;
        if (player != null) player.canMove = false;
        group.blocksRaycasts = true;
        for (float t = 0f; t < 1f; t += Time.unscaledDeltaTime / 0.4f) {
            group.alpha = t;
            yield return null;
        }
        group.alpha = 1f;
        yield return ShowCaption(text);
        yield return new WaitForSecondsRealtime(StandaloneHold);
        if (player != null && couldMove) player.canMove = true;
        yield return FadeAway();
    }

    private IEnumerator ShowCaption(string text) {
        caption.alpha = 0f;
        if (string.IsNullOrEmpty(text)) yield break;
        caption.text = text;
        for (float t = 0f; t < 1f; t += Time.unscaledDeltaTime / CaptionFadeIn) {
            caption.alpha = t;
            yield return null;
        }
        caption.alpha = 1f;
    }

    private IEnumerator FadeAway() {
        float startCaption = caption.alpha;
        for (float t = 1f; t > 0f; t -= Time.unscaledDeltaTime / FadeOut) {
            group.alpha = t;
            caption.alpha = startCaption * t;
            yield return null;
        }
        Hide();
    }

    private static PlayerMovement_A FindPlayer() {
        Scene active = SceneManager.GetActiveScene();
        foreach (PlayerMovement_A pm in FindObjectsByType<PlayerMovement_A>(FindObjectsSortMode.None)) {
            if (pm.gameObject.scene == active) return pm;
        }
        return null;
    }
}
