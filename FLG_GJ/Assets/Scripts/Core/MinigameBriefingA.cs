using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Briefing card at the start of each minigame: what to do and which keys to use, with a short
/// countdown while the game waits (ENTER skips). A retry of the same minigame gets a short
/// "Try again" instead. Nothing in the minigames is changed; the card reacts to their scenes loading.
/// </summary>
public class MinigameBriefingA : MonoBehaviour {
    private const string PrefabResource = "MinigameBriefing";
    private const float FullCountdown = 3f;
    private const float RetryHold = 1.2f;

    private struct Brief {
        public string title, goal, controls;
        public Brief(string title, string goal, string controls) { this.title = title; this.goal = goal; this.controls = controls; }
    }

    // Minigame scene -> briefing.
    private static readonly Dictionary<string, Brief> briefs = new Dictionary<string, Brief> {
        { "DrugTheftMiniGame", new Brief("THE RED STASH",
            "Sneak into the camp, take the red stash and get back out.\nStay out of the guards' sight.",
            "WASD / ARROWS  move") },
        { "DefendYourself_ACT2", new Brief("MARTIN",
            "Defend yourself. Put Martin down before he puts you down.",
            "A / D  move      SPACE  punch      hold LEFT CTRL  block") },
        { "TopDownShooter_Drugs_ACT2", new Brief("THE WAREHOUSE",
            "Survive every wave and keep the package in one piece.\nAmmo is limited. Enemies drop more.",
            "WASD  move      MOUSE  aim      LEFT CLICK  shoot") },
        { "TruckChase_ACT2", new Brief("THE CHASE",
            "Get the guns to the passage. Keep the truck alive until the last wave is gone.",
            "A / D  steer      MOUSE  aim      hold LEFT CLICK  fire") },
        { "DJ_MiniGame_D", new Brief("TOKIO",
            "Click the colour the word SAYS, not the colour it is printed in.\nReach 1500 points in 60 seconds. Wrong answers cost points.",
            "MOUSE  click") },
        { "CupsShuflingA", new Brief("EMILIO'S CUPS",
            "Watch the cup with the ball. Pick it right three times.\nEvery round gets faster.",
            "Click START, then click a cup") },
        { "ShootingPractice_D", new Brief("SALAZAR'S TEST",
            "Shoot the cans before they hit the ground. Three rounds.\nLet three cans fall in a round and it's over.",
            "MOUSE  aim      LEFT CLICK / SPACE  shoot") },
        { "FinalBossBattleA", new Brief("DARIUS",
            "Dodge his shots and bombs. When he is VULNERABLE, get close and hit him.",
            "WASD  move      SPACE  attack") },
    };

    [SerializeField] private CanvasGroup group;
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text goal;
    [SerializeField] private TMP_Text controls;
    [SerializeField] private TMP_Text countdown;
    [SerializeField] private TMP_Text hint;

    private static MinigameBriefingA instance;
    private static readonly HashSet<string> briefed = new HashSet<string>();
    private Coroutine running;
    private float timeScaleBefore = 1f;

    /// <summary>True while the briefing holds the minigame.</summary>
    public static bool Showing { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init() {
        Showing = false;
        briefed.Clear();
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        // A new chapter: full briefings again for anything played from here on.
        if (mode == LoadSceneMode.Single && !briefs.ContainsKey(scene.name)) briefed.Clear();
        if (!briefs.TryGetValue(scene.name, out Brief brief)) return;
        if (StoryCheckpointA.IsRestoring || StoryCheckpointA.ContinuingFromCheckpoint) return;
        if (Get() == null) return;
        bool retry = !briefed.Add(scene.name);
        instance.Play(brief, retry);
    }

    private static MinigameBriefingA Get() {
        if (instance != null) return instance;
        GameObject prefab = Resources.Load<GameObject>(PrefabResource);
        if (prefab == null) return null;
        GameObject go = Instantiate(prefab);
        go.name = PrefabResource;
        DontDestroyOnLoad(go);
        instance = go.GetComponent<MinigameBriefingA>();
        instance.group.alpha = 0f;
        instance.group.blocksRaycasts = false;
        return instance;
    }

    private void Play(Brief brief, bool retry) {
        if (running != null) {
            StopCoroutine(running);
            Release();
        }
        running = StartCoroutine(Run(brief, retry));
    }

    private IEnumerator Run(Brief brief, bool retry) {
        Showing = true;
        timeScaleBefore = Time.timeScale > 0f ? Time.timeScale : 1f;
        Time.timeScale = 0f;

        title.text = brief.title;
        goal.text = retry ? "Try again." : brief.goal;
        controls.text = brief.controls;
        hint.text = retry ? "" : "ENTER  skip";
        countdown.text = "";
        group.blocksRaycasts = true;

        for (float t = 0f; t < 1f; t += Time.unscaledDeltaTime / 0.25f) { group.alpha = t; yield return null; }
        group.alpha = 1f;

        if (retry) {
            yield return new WaitForSecondsRealtime(RetryHold);
        } else {
            float left = FullCountdown;
            yield return new WaitForSecondsRealtime(0.4f); // a beat to read before the count starts
            while (left > 0f) {
                if (!PauseMenuManager.IsPaused) {
                    left -= Time.unscaledDeltaTime;
                    if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) left = 0f;
                }
                countdown.text = left > 0f ? Mathf.CeilToInt(left).ToString() : "";
                yield return null;
            }
        }

        while (PauseMenuManager.IsPaused) yield return null;
        Release();
        for (float t = 1f; t > 0f; t -= Time.unscaledDeltaTime / 0.25f) { group.alpha = t; yield return null; }
        group.alpha = 0f;
        running = null;
    }

    // Give the game back its time (the fade-out runs on real time).
    private void Release() {
        group.blocksRaycasts = false;
        if (Showing && Time.timeScale == 0f) Time.timeScale = timeScaleBefore;
        Showing = false;
        UncoverMinigame();
    }

    // Entering a minigame also starts the act's black screen, which runs on game time and would keep
    // covering the minigame for a moment after the briefing. Hide its panel now; its timer and the
    // story flags it sets at the end are untouched (it turns the panel off again itself).
    private static void UncoverMinigame() {
        var field = typeof(ShowingStoryUpdates1).GetField("storyUpdatePanel",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        if (field == null) return;
        foreach (ShowingStoryUpdates1 s in FindObjectsByType<ShowingStoryUpdates1>(FindObjectsSortMode.None)) {
            if (field.GetValue(s) is GameObject panel && panel.activeSelf) panel.SetActive(false);
        }
    }

    private void OnDestroy() {
        if (Showing && Time.timeScale == 0f) Time.timeScale = 1f;
        Showing = false;
    }
}
