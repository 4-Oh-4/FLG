using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

/// <summary>
/// Shows the newspapers (and the ending letter) at their points in the story, as a full-screen
/// page the player reads and dismisses with Space / Enter.
///
/// Nothing in the scenes calls this. Pages are tied to story moments from the outside:
///  - a story flag (Act 1's "one month later" time skip), shown on top of that black screen;
///  - a chapter or cutscene scene starting (the video waits until the pages are closed).
/// While a page is open the game is paused (timeScale 0), so timers such as the black screen and
/// THE END simply wait. Pages never open over a dialogue or during a checkpoint restore.
/// </summary>
public class StoryDocumentA : MonoBehaviour {
    private const string PrefabResource = "StoryDocument";
    private const string PaperFolder = "Newspapers/";
    private const string LetterId = "letter";
    private const float MinReadTime = 0.8f; // ignore dismiss presses before this (avoids skipping by accident)
    private const float DropInTime = 0.35f;

    // Story flag -> pages, and how long after the flag to open them (lets the time-skip caption show first).
    private static readonly Dictionary<string, string[]> flagPages = new Dictionary<string, string[]> {
        { "Collect", new[] { "2012" } },
    };
    private const float FlagPageDelay = 1.4f;

    // Scene start -> pages.
    private static readonly Dictionary<string, string[]> scenePages = new Dictionary<string, string[]> {
        { "Visual2", new[] { "2014" } },                 // after the promotion, before the Niño montage
        { "Carlos_house", new[] { "2016" } },            // Act II opens
        { "Act_3_Map_New 1", new[] { "2022", "2023" } }, // five years pass, Act III opens
        { "Visual4", new[] { LetterId } },               // the letter left in Niño's empty hideout
        { "TheEnd", new[] { "2023 after raid" } },       // epilogue
    };

    private const string LetterText =
        "H.\n\n" +
        "The army moves on Tocoron at dawn on the 20th. Eleven thousand men, tanks at every gate. " +
        "Nobody inside will know until it is too late.\n\n" +
        "Take the tunnel tonight. Leave the boys behind. They will keep the soldiers busy long enough.\n\n" +
        "A car will wait for you on the Aragua road.\n\n" +
        "Burn this.\n\n" +
        "T. El Asimi";

    [SerializeField] private CanvasGroup group;
    [SerializeField] private RectTransform sheet;
    [SerializeField] private Image paper;
    [SerializeField] private GameObject letter;
    [SerializeField] private TMP_Text letterText;
    [SerializeField] private CanvasGroup hint;

    private static StoryDocumentA instance;
    private readonly Queue<string> queue = new Queue<string>();
    private Coroutine running;
    private VideoPlayer heldVideo;
    private float heartbeat; // last time the open page updated (see Update)

    /// <summary>True while a page is on screen.</summary>
    public static bool IsOpen { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init() {
        IsOpen = false;
        StoryManagertAct1A.OnFlagChanged -= OnFlagChanged;
        StoryManagertAct1A.OnFlagChanged += OnFlagChanged;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void HandleStartingScene() => OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);

    private static int handledScene = -1;

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (mode != LoadSceneMode.Single || scene.handle == handledScene) return;
        handledScene = scene.handle;
        if (instance != null) instance.CloseNow(); // pages never carry over into another scene
        if (StoryCheckpointA.ContinuingFromCheckpoint || StoryCheckpointA.IsRestoring) return;
        if (scenePages.TryGetValue(scene.name, out string[] pages) && Get() != null) instance.Enqueue(pages, 0.25f, true);
    }

    private static void OnFlagChanged(string key, bool value) {
        if (!value || StoryCheckpointA.IsRestoring || StoryCheckpointA.ContinuingFromCheckpoint) return;
        if (flagPages.TryGetValue(key, out string[] pages) && Get() != null) instance.Enqueue(pages, FlagPageDelay, false); // over the time-skip narration, before it continues
    }

    private static bool dismissRequested;

    /// <summary>Close the current page as if Space was pressed (for UI buttons and tests).</summary>
    public static void Dismiss() => dismissRequested = true;

    /// <summary>Open pages from code (e.g. tests). Ids are file names in Resources/Newspapers, or "letter".</summary>
    public static void Show(params string[] pages) {
        if (Get() != null) instance.Enqueue(pages, 0f, false);
    }

    private static StoryDocumentA Get() {
        if (instance != null) return instance;
        GameObject prefab = Resources.Load<GameObject>(PrefabResource);
        if (prefab == null) return null;
        GameObject go = Instantiate(prefab);
        go.name = PrefabResource;
        DontDestroyOnLoad(go);
        instance = go.GetComponent<StoryDocumentA>();
        instance.SetVisible(false);
        return instance;
    }

    private void Enqueue(string[] pages, float delay, bool waitForDialogue) {
        foreach (string p in pages) queue.Enqueue(p);
        if (running == null) running = StartCoroutine(Run(delay, waitForDialogue));
    }

    private IEnumerator Run(float delay, bool waitForDialogue) {
        yield return new WaitForSecondsRealtime(delay);
        // Wait for a calm moment: no checkpoint restore, not behind the pause menu (and, for pages at
        // the start of a scene, no dialogue box). The dialogue can't advance while a page is open.
        while (!CanOpen(waitForDialogue)) yield return null;

        float previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        heartbeat = Time.realtimeSinceStartup;
        IsOpen = true;
        VideoPlayer video = HoldVideo(true, null);
        heldVideo = video;

        while (queue.Count > 0) {
            string id = queue.Dequeue();
            if (!Load(id)) continue;
            yield return Present();
        }

        SetVisible(false);
        IsOpen = false;
        if (Time.timeScale == 0f) Time.timeScale = previousTimeScale > 0f ? previousTimeScale : 1f;
        HoldVideo(false, video);
        heldVideo = null;
        running = null;
    }

    // Safety net: if the page coroutine ever stops while the game is paused for it, close the page and
    // give the game back instead of leaving it frozen.
    private void Update() {
        if (!IsOpen || Time.realtimeSinceStartup - heartbeat < 3f) return;
        Debug.LogWarning("StoryDocumentA: page stopped updating; closing it.");
        VideoPlayer video = heldVideo;
        CloseNow();
        HoldVideo(false, video);
    }

    private static bool CanOpen(bool waitForDialogue) {
        if (StoryCheckpointA.IsRestoring || StoryCheckpointA.ContinuingFromCheckpoint || PauseMenuManager.IsPaused) return false;
        if (!waitForDialogue) return true;
        foreach (DialougeManagerA dm in FindObjectsByType<DialougeManagerA>(FindObjectsSortMode.None)) if (dm.IsOpen) return false;
        return true;
    }

    private bool Load(string id) {
        if (id == LetterId) {
            paper.gameObject.SetActive(false);
            letter.SetActive(true);
            letterText.text = LetterText;
            return true;
        }
        Sprite sprite = Resources.Load<Sprite>(PaperFolder + id);
        if (sprite == null) {
            Debug.LogWarning("StoryDocumentA: no newspaper '" + id + "' in Resources/" + PaperFolder);
            return false;
        }
        letter.SetActive(false);
        paper.gameObject.SetActive(true);
        paper.sprite = sprite;
        paper.preserveAspect = true;
        return true;
    }

    // Drop the page in, wait for the player to dismiss it, then fade it out.
    private IEnumerator Present() {
        SetVisible(true);
        hint.alpha = 0f;
        float tilt = Random.Range(-4f, -1.5f);
        for (float t = 0f; t < 1f; t += Time.unscaledDeltaTime / DropInTime) {
            heartbeat = Time.realtimeSinceStartup;
            float e = 1f - (1f - t) * (1f - t); // ease out
            group.alpha = Mathf.Clamp01(t * 2f);
            sheet.localScale = Vector3.one * Mathf.Lerp(1.12f, 1f, e);
            sheet.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(tilt - 6f, tilt, e));
            yield return null;
        }
        group.alpha = 1f;
        sheet.localScale = Vector3.one;
        sheet.localRotation = Quaternion.Euler(0f, 0f, tilt);

        float shown = 0f;
        dismissRequested = false;
        while (true) {
            heartbeat = Time.realtimeSinceStartup;
            if (dismissRequested && shown > MinReadTime) break;
            shown += Time.unscaledDeltaTime;
            if (shown > MinReadTime) hint.alpha = Mathf.MoveTowards(hint.alpha, 1f, Time.unscaledDeltaTime * 3f);
            if (shown > MinReadTime && !PauseMenuManager.IsPaused && DismissPressed()) break;
            yield return null;
        }

        for (float t = 1f; t > 0f; t -= Time.unscaledDeltaTime / 0.25f) {
            heartbeat = Time.realtimeSinceStartup;
            group.alpha = t;
            yield return null;
        }
        group.alpha = 0f;
    }

    // E and the mouse are left out on purpose: E talks to NPCs and opens doors, clicks attack.
    private static bool DismissPressed() {
        return Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter);
    }

    // In a cutscene scene the video waits behind the page (and can't be skipped by the same key).
    private static VideoPlayer HoldVideo(bool hold, VideoPlayer known) {
        VideoPlayer video = known != null ? known : FindAnyObjectByType<VideoPlayer>();
        if (video == null) return null;
        VideoController controller = video.GetComponent<VideoController>();
        if (controller == null) controller = FindAnyObjectByType<VideoController>();
        if (hold) {
            if (controller != null) controller.enabled = false;
            if (instance != null) instance.StartCoroutine(KeepPaused(video));
        } else {
            if (controller != null) controller.enabled = true;
            video.Play();
        }
        return video;
    }

    // The video may only start preparing after the page opens; keep it paused until it closes.
    private static IEnumerator KeepPaused(VideoPlayer video) {
        while (IsOpen && video != null) {
            if (video.isPlaying) video.Pause();
            yield return null;
        }
    }

    private void CloseNow() {
        StopAllCoroutines();
        running = null;
        heldVideo = null;
        queue.Clear();
        if (IsOpen && Time.timeScale == 0f) Time.timeScale = 1f;
        IsOpen = false;
        SetVisible(false);
    }

    private void SetVisible(bool visible) {
        group.alpha = visible ? group.alpha : 0f;
        group.blocksRaycasts = visible;
        gameObject.SetActive(true);
    }
}
