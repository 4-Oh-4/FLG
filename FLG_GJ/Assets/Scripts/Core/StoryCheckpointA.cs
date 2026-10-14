using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Mid-chapter autosave (one slot). Works alongside ChapterSaveA, which only remembers the chapter.
///
/// Saving: while a supported chapter scene is loaded, every story flag change is recorded in order.
/// Shortly after a change, at a safe moment (not paused, no dialogue, no black screen, no minigame),
/// the checkpoint is written: the flag history, the player position, what changed in the scene
/// since it was loaded (destroyed / toggled / moved objects, switched components), and a few
/// script counters that are not story flags.
///
/// Restoring: load the chapter scene fresh, replay the flag history with presentation muted
/// (IsRestoring: no black screens, dialogue, minigames, NPC walks), then apply the saved scene
/// changes and counters, and put the player back.
/// </summary>
public class StoryCheckpointA : MonoBehaviour {
    // Chapters whose restore has been verified. Others still get chapter-start saves only.
    private static readonly string[] supportedScenes = { "Act_1_Map_K", "Act_2_Map_New", "Act_3_Map_New 1" };

    // Script fields that hold story progress outside the flags: type name -> field names.
    private static readonly Dictionary<string, string[]> trackedFields = new Dictionary<string, string[]> {
        { "LoadMiniGameA", new[] { "once" } },
        { "EvangalicTriggerScript", new[] { "once" } },
        { "PlayerInteractionHoldA", new[] { "once" } },
        { "ShowingStoryUpdates1", new[] { "i" } },
        { "LoadUnloadMiniGamesPlayerA", new[] { "quest", "state" } },
        { "GameManagerAct1A", new[] { "trashTobeCollected", "foodTobeCollected" } },
        { "NpcMotorA", new[] { "isMoving", "targetPosition" } },
        { "FoodSpawnerAct1A", new[] { "isSpawning" } },
        // Auto-narrators fire once; one switched on during the replay must not narrate again.
        { "NPC_A", new[] { "isNarator" } },
        // Act 2 one-shot / staged triggers
        { "ChurchTimeSkip", new[] { "once" } },
        { "ClubEntryColliderA", new[] { "i" } },
        { "DariusIntroMarketAct2", new[] { "once" } },
        { "DoorToNinoDoor", new[] { "i" } },
        { "GateColliderInWareHouse", new[] { "once" } },
        { "GoHomeSCript", new[] { "once" } },
        { "NarratorCallA", new[] { "once" } },
        { "TunnelScriptAct2", new[] { "count" } },
        { "TruckDoorA", new[] { "once" } },
        { "BathroomScriptToDespawn", new[] { "i" } },
        { "Narator_3Colider", new[] { "i" } },
        { "RoxanaTalkCollider", new[] { "i" } },
        { "NinoTriggerOffice", new[] { "i" } },
        // Act 3 staged triggers
        { "BBscriptAct3", new[] { "i" } },
        { "BigShackCollider", new[] { "b" } },
        { "FinalShackColldier", new[] { "h" } },
        { "ShackCollider", new[] { "a" } },
    };

    private const int SaveVersion = 1;
    private const float SaveDelayAfterFlagChange = 1f;
    private const BindingFlags FieldFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    /// <summary>True while a checkpoint is being replayed. Presentation code checks this and does nothing.</summary>
    public static bool IsRestoring { get; private set; }

    /// <summary>True from pressing Continue until the checkpoint has been fully restored (covers the scene load too).</summary>
    public static bool ContinuingFromCheckpoint { get; private set; }

    private static StoryCheckpointA instance;
    private static string SavePath => Path.Combine(Application.persistentDataPath, "checkpoint.json");

    // Current chapter session
    private Scene chapterScene;
    private bool recording;
    private readonly List<FlagChange> flagLog = new List<FlagChange>();
    private readonly List<GameObject> objects = new List<GameObject>();      // index = stable id
    private readonly List<ObjectPristine> pristine = new List<ObjectPristine>();
    private bool dirty;
    private float lastFlagChangeTime;
    private CheckpointData pendingRestore;
    private GameObject loadingCover;

    #region Data

    [Serializable] public class FlagChange { public string key; public bool value; }
    [Serializable] public class ComponentState { public int index; public int enabled = -1; public int isTrigger = -1; public float alpha = -1f; }
    [Serializable] public class ObjectState { public int id; public bool destroyed; public int active = -1; public bool hasPosition; public Vector3 position; public List<ComponentState> components = new List<ComponentState>(); }
    [Serializable] public class FieldState { public int id; public string type; public string field; public string value; }

    [Serializable]
    public class CheckpointData {
        public int version;
        public string scene;
        public string savedAt;
        public Vector3 playerPosition;
        public List<FlagChange> flags = new List<FlagChange>();
        public List<ObjectState> objects = new List<ObjectState>();
        public List<FieldState> fields = new List<FieldState>();
    }

    private class ObjectPristine {
        public bool active;
        public Vector3 position;
        public int componentCount;
        public bool[] enabled;
        public bool[] isTrigger;
        public float[] alpha;
    }

    #endregion

    #region Public API

    public static bool HasCheckpointFor(string sceneName) {
        CheckpointData data = ReadSave();
        return data != null && data.scene == sceneName && IsSupported(sceneName);
    }

    /// <summary>Continue from the mid-chapter checkpoint. Returns false if there is none for the saved chapter.</summary>
    public static bool TryContinue() {
        CheckpointData data = ReadSave();
        if (data == null || data.scene != ChapterSaveA.SavedScene || !IsSupported(data.scene)) return false;
        if (!Application.CanStreamedLevelBeLoaded(data.scene)) return false;
        EnsureInstance();
        instance.pendingRestore = data;
        ContinuingFromCheckpoint = true;
        instance.ShowLoadingCover(true);
        SceneManager.LoadScene(data.scene);
        return true;
    }

    /// <summary>Save right now if the game is in a state that can be restored (used when quitting to the menu).</summary>
    public static void SaveNowIfPossible(bool ignorePause) {
        if (instance != null && instance.CanSave(ignorePause)) instance.WriteSave();
    }

    public static void DeleteCheckpoint() {
        try { if (File.Exists(SavePath)) File.Delete(SavePath); } catch (Exception e) { Debug.LogWarning("[Checkpoint] Could not delete save: " + e.Message); }
    }

    /// <summary>For testing: a readable description of the current chapter state (flags, changed objects, counters, HUD).</summary>
    public static string DescribeCurrentState() {
        if (instance == null || !instance.recording) return "(not recording)";
        CheckpointData data = instance.Capture();
        var lines = new List<string>();
        var flags = StoryManagertAct1A.Instance != null ? StoryManagertAct1A.Instance.GetAllFlagsCopy() : new Dictionary<string, bool>();
        var keys = new List<string>(flags.Keys); keys.Sort(StringComparer.Ordinal);
        foreach (string k in keys) lines.Add("flag " + k + "=" + flags[k]);
        foreach (ObjectState o in data.objects) {
            string name = instance.Describe(o.id);
            if (o.destroyed) { lines.Add("obj " + name + " destroyed"); continue; }
            if (o.active >= 0) lines.Add("obj " + name + " active=" + o.active);
            if (o.hasPosition) lines.Add("obj " + name + " pos=" + o.position.ToString("F2"));
            foreach (ComponentState c in o.components) lines.Add("obj " + name + " comp#" + c.index + " en=" + c.enabled + " trig=" + c.isTrigger + " a=" + c.alpha.ToString("F2", CultureInfo.InvariantCulture));
        }
        foreach (FieldState f in data.fields) lines.Add("field " + instance.Describe(f.id) + " " + f.type + "." + f.field + "=" + f.value);
        foreach (WindowQuestPointer_A pointer in FindObjectsByType<WindowQuestPointer_A>(FindObjectsInactive.Include, FindObjectsSortMode.None)) {
            lines.Add("questPointer " + pointer.name + " active=" + pointer.gameObject.activeInHierarchy + " target=" + (pointer.target != null ? pointer.target.name : "none"));
        }
        foreach (QuestTextA q in FindObjectsByType<QuestTextA>(FindObjectsInactive.Include, FindObjectsSortMode.None)) {
            TMPro.TMP_Text t = q.GetComponentInChildren<TMPro.TMP_Text>(true);
            lines.Add("questText " + (t != null ? t.text : "?"));
        }
        lines.Add("player " + data.playerPosition.ToString("F2"));
        lines.Sort(StringComparer.Ordinal);
        return string.Join("\n", lines.ToArray());
    }

    #endregion

    #region Lifecycle

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init() {
        IsRestoring = false;
        ContinuingFromCheckpoint = false;
        StoryManagertAct1A.OnFlagChanged -= OnFlagChangedStatic;
        StoryManagertAct1A.OnFlagChanged += OnFlagChangedStatic;
        SceneManager.sceneLoaded -= OnSceneLoadedStatic;
        SceneManager.sceneLoaded += OnSceneLoadedStatic;
    }

    // sceneLoaded is not raised for the scene the game starts in (e.g. pressing Play in Act 1 in the editor).
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void HandleStartingScene() {
        Scene active = SceneManager.GetActiveScene();
        if (instance != null && instance.handledScene == active.handle) return;
        OnSceneLoadedStatic(active, LoadSceneMode.Single);
    }

    private int handledScene = -1;

    private static void EnsureInstance() {
        if (instance != null) return;
        var go = new GameObject("StoryCheckpoint");
        DontDestroyOnLoad(go);
        instance = go.AddComponent<StoryCheckpointA>();
    }

    private static void OnSceneLoadedStatic(Scene scene, LoadSceneMode mode) {
        if (mode != LoadSceneMode.Single) return;
        EnsureInstance();
        instance.OnChapterSceneLoaded(scene);
    }

    private static void OnFlagChangedStatic(string key, bool value) {
        if (instance == null || !instance.recording || IsRestoring) return;
        instance.flagLog.Add(new FlagChange { key = key, value = value });
        instance.dirty = true;
        instance.lastFlagChangeTime = Time.unscaledTime;
    }

    private void OnChapterSceneLoaded(Scene scene) {
        if (handledScene == scene.handle) return;
        handledScene = scene.handle;
        recording = false;
        flagLog.Clear();
        objects.Clear();
        pristine.Clear();
        dirty = false;

        if (!IsSupported(scene.name)) {
            pendingRestore = null;
            ContinuingFromCheckpoint = false;
            ShowLoadingCover(false);
            // A different chapter started: the old mid-chapter checkpoint no longer applies.
            if (IsChapterScene(scene.name)) DeleteCheckpoint();
            return;
        }

        chapterScene = scene;
        BuildObjectTable(scene);
        recording = true;

        if (pendingRestore != null && pendingRestore.scene == scene.name) {
            CheckpointData data = pendingRestore;
            pendingRestore = null;
            StartCoroutine(RestoreRoutine(data));
        } else {
            // Fresh start of this chapter (new game or arriving from the previous chapter).
            ContinuingFromCheckpoint = false;
            DeleteCheckpoint();
            ShowLoadingCover(false);
        }
    }

    private void Update() {
        if (!dirty || IsRestoring) return;
        if (Time.unscaledTime - lastFlagChangeTime < SaveDelayAfterFlagChange) return;
        if (CanSave(false)) WriteSave();
    }

    private void OnApplicationQuit() {
        if (dirty) SaveNowIfPossible(true);
    }

    #endregion

    #region Saving

    private static bool IsSupported(string sceneName) => Array.IndexOf(supportedScenes, sceneName) >= 0;
    private static bool IsChapterScene(string sceneName) => sceneName == "Carlos_house" || sceneName == "Act_2_Map_New" || sceneName == "Act_3_Map_New 1" || IsSupported(sceneName);

    private bool CanSave(bool ignorePause) {
        if (!recording || IsRestoring || !chapterScene.IsValid() || !chapterScene.isLoaded) return false;
        if (SceneManager.sceneCount != 1) return false;                         // minigame or interior loaded on top
        if (!ignorePause && PauseMenuManager.IsPaused) return false;
        if (!ignorePause && Time.timeScale <= 0f) return false;
        foreach (DialougeManagerA dm in FindObjectsByType<DialougeManagerA>(FindObjectsSortMode.None)) {
            if (dm.IsOpen) return false;                                        // mid-conversation
        }
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return false;
        PlayerMovement_A movement = player.GetComponent<PlayerMovement_A>();
        if (movement != null && !movement.canMove) return false;                // black screen, journal, cutscene
        return true;
    }

    private void WriteSave() {
        try {
            CheckpointData data = Capture();
            string json = JsonUtility.ToJson(data);
            string tmp = SavePath + ".tmp";
            File.WriteAllText(tmp, json);
            if (File.Exists(SavePath)) File.Delete(SavePath);
            File.Move(tmp, SavePath);
            dirty = false;
            Debug.Log("[Checkpoint] Saved (" + data.flags.Count + " story steps).");
        } catch (Exception e) {
            Debug.LogWarning("[Checkpoint] Save failed: " + e.Message);
        }
    }

    private static CheckpointData ReadSave() {
        try {
            if (!File.Exists(SavePath)) return null;
            CheckpointData data = JsonUtility.FromJson<CheckpointData>(File.ReadAllText(SavePath));
            return data != null && data.version == SaveVersion ? data : null;
        } catch (Exception e) {
            Debug.LogWarning("[Checkpoint] Could not read save, ignoring it: " + e.Message);
            return null;
        }
    }

    private CheckpointData Capture() {
        var data = new CheckpointData {
            version = SaveVersion,
            scene = chapterScene.name,
            savedAt = DateTime.Now.ToString("o", CultureInfo.InvariantCulture),
            flags = new List<FlagChange>(flagLog),
        };
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) data.playerPosition = player.transform.position;

        for (int id = 0; id < objects.Count; id++) {
            GameObject go = objects[id];
            ObjectPristine p = pristine[id];
            if (go == null) {
                data.objects.Add(new ObjectState { id = id, destroyed = true });
                continue;
            }
            var state = new ObjectState { id = id };
            bool changed = false;
            if (go.activeSelf != p.active) { state.active = go.activeSelf ? 1 : 0; changed = true; }
            if (TracksPosition(go) && (go.transform.position - p.position).sqrMagnitude > 0.0001f) {
                state.hasPosition = true; state.position = go.transform.position; changed = true;
            }
            Component[] comps = SavedComponents(go);
            if (comps.Length == p.componentCount) {
                for (int i = 0; i < comps.Length; i++) {
                    var cs = new ComponentState { index = i };
                    bool compChanged = false;
                    bool en, trig; float a;
                    if (ReadComponent(comps[i], out en, out trig, out a)) {
                        if (en != p.enabled[i]) { cs.enabled = en ? 1 : 0; compChanged = true; }
                        if (trig != p.isTrigger[i]) { cs.isTrigger = trig ? 1 : 0; compChanged = true; }
                        if (a >= 0f && Mathf.Abs(a - p.alpha[i]) > 0.001f) { cs.alpha = a; compChanged = true; }
                    }
                    if (compChanged) { state.components.Add(cs); changed = true; }
                }
            }
            if (changed) data.objects.Add(state);

            foreach (MonoBehaviour mb in go.GetComponents<MonoBehaviour>()) {
                if (mb == null) continue;
                string[] fields;
                if (!trackedFields.TryGetValue(mb.GetType().Name, out fields)) continue;
                foreach (string fieldName in fields) {
                    FieldInfo fi = mb.GetType().GetField(fieldName, FieldFlags);
                    if (fi == null) continue;
                    data.fields.Add(new FieldState { id = id, type = mb.GetType().Name, field = fieldName, value = Encode(fi.GetValue(mb)) });
                }
            }
        }
        return data;
    }

    #endregion

    #region Restoring

    private IEnumerator RestoreRoutine(CheckpointData data) {
        // Let every Start() and the first LateUpdate (narrator auto-dialogue) run on the fresh scene first.
        yield return null;
        yield return null;

        IsRestoring = true;
        try {
            StoryManagertAct1A story = StoryManagertAct1A.Instance;
            if (story != null) {
                foreach (FlagChange f in data.flags) story.SetFlag(f.key, f.value);
            }
            flagLog.Clear();
            flagLog.AddRange(data.flags);

            GameObject player = GameObject.FindWithTag("Player");
            if (player != null) {
                player.transform.position = data.playerPosition;
                Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
                if (rb != null) { rb.position = data.playerPosition; rb.linearVelocity = Vector2.zero; }
            }

            ApplyObjects(data);
            ApplyFields(data);
        } catch (Exception e) {
            Debug.LogError("[Checkpoint] Restore failed part-way: " + e);
        } finally {
            IsRestoring = false;
        }

        AfterRestore(data);
        dirty = false;
        Debug.Log("[Checkpoint] Restored " + data.flags.Count + " story steps.");

        yield return null;
        ShowLoadingCover(false);
        ContinuingFromCheckpoint = false;
    }

    private void ApplyObjects(CheckpointData data) {
        foreach (ObjectState o in data.objects) {
            if (o.id < 0 || o.id >= objects.Count) continue;
            GameObject go = objects[o.id];
            if (go == null) continue;
            if (o.destroyed) { Destroy(go); continue; }
            if (o.active >= 0) go.SetActive(o.active == 1);
            if (o.hasPosition) {
                go.transform.position = o.position;
                Rigidbody2D rb = go.GetComponent<Rigidbody2D>();
                if (rb != null) rb.position = o.position;
            }
            Component[] comps = SavedComponents(go);
            foreach (ComponentState c in o.components) {
                if (c.index < 0 || c.index >= comps.Length) continue;
                WriteComponent(comps[c.index], c);
            }
        }
    }

    private void ApplyFields(CheckpointData data) {
        foreach (FieldState f in data.fields) {
            if (f.id < 0 || f.id >= objects.Count || objects[f.id] == null) continue;
            if (f.type == "FoodSpawnerAct1A") continue; // coroutine state, handled in AfterRestore
            foreach (MonoBehaviour mb in objects[f.id].GetComponents<MonoBehaviour>()) {
                if (mb == null || mb.GetType().Name != f.type) continue;
                FieldInfo fi = mb.GetType().GetField(f.field, FieldFlags);
                if (fi != null) fi.SetValue(mb, Decode(f.value, fi.FieldType));
            }
        }
    }

    // State that needs a method call rather than a field write.
    private void AfterRestore(CheckpointData data) {
        // Player control: the intro journal and black screens freeze the player; a checkpoint is always taken with control.
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) {
            PlayerMovement_A movement = player.GetComponent<PlayerMovement_A>();
            if (movement != null) movement.canMove = true;
            UnityEngine.InputSystem.PlayerInput input = player.GetComponent<UnityEngine.InputSystem.PlayerInput>();
            if (input != null) input.enabled = true;
        }

        foreach (FieldState f in data.fields) {
            if (f.id < 0 || f.id >= objects.Count || objects[f.id] == null) continue;
            GameObject go = objects[f.id];

            if (f.type == "FoodSpawnerAct1A") {
                FoodSpawnerAct1A spawner = go.GetComponent<FoodSpawnerAct1A>();
                if (spawner == null) continue;
                if ((bool)Decode(f.value, typeof(bool))) spawner.StartSpawning(); else spawner.StopSpawning();
            } else if (f.type == "NpcMotorA" && f.field == "isMoving" && (bool)Decode(f.value, typeof(bool))) {
                // Was walking when saved: carry on to the same destination.
                NpcMotorA motor = go.GetComponent<NpcMotorA>();
                FieldInfo target = typeof(NpcMotorA).GetField("targetPosition", FieldFlags);
                if (motor != null && target != null) motor.MoveTo((Vector3)target.GetValue(motor));
            }
        }

        // Collection HUD text follows the restored counters.
        foreach (GameManagerAct1A gm in FindObjectsByType<GameManagerAct1A>(FindObjectsSortMode.None)) {
            MethodInfo updateUI = typeof(GameManagerAct1A).GetMethod("UpdateUI", FieldFlags);
            if (updateUI != null) updateUI.Invoke(gm, null);
        }

        // Silvio's death (Act 2) is an animation; once it has happened the script disables itself.
        foreach (silvioDeath death in FindObjectsByType<silvioDeath>(FindObjectsInactive.Include, FindObjectsSortMode.None)) {
            if (!death.enabled && StoryManagertAct1A.Instance != null && StoryManagertAct1A.Instance.GetFlag("DeathTalk")) {
                Animator anim = death.GetComponent<Animator>();
                if (anim != null && anim.isActiveAndEnabled) anim.SetTrigger("Die");
            }
        }

        // Finished cooking station turns yellow instead of disappearing.
        foreach (HoldInteractableCookingA cook in FindObjectsByType<HoldInteractableCookingA>(FindObjectsInactive.Include, FindObjectsSortMode.None)) {
            if (!cook.enabled) {
                Renderer r = cook.GetComponent<Renderer>();
                if (r != null) r.material.color = Color.yellow;
            }
        }
    }

    #endregion

    #region Object table

    private void BuildObjectTable(Scene scene) {
        foreach (GameObject root in scene.GetRootGameObjects()) AddRecursive(root.transform);
    }

    private void AddRecursive(Transform t) {
        GameObject go = t.gameObject;
        Component[] comps = SavedComponents(go);
        var p = new ObjectPristine {
            active = go.activeSelf,
            position = t.position,
            componentCount = comps.Length,
            enabled = new bool[comps.Length],
            isTrigger = new bool[comps.Length],
            alpha = new float[comps.Length],
        };
        for (int i = 0; i < comps.Length; i++) {
            bool en, trig; float a;
            ReadComponent(comps[i], out en, out trig, out a);
            p.enabled[i] = en; p.isTrigger[i] = trig; p.alpha[i] = a;
        }
        objects.Add(go);
        pristine.Add(p);
        for (int i = 0; i < t.childCount; i++) AddRecursive(t.GetChild(i));
    }

    // Components in a stable order for saving. PressToEnterA is added to doors at runtime, so it is
    // left out to keep component indexes identical whether or not it has been attached yet.
    private static Component[] SavedComponents(GameObject go) {
        Component[] all = go.GetComponents<Component>();
        int keep = 0;
        foreach (Component c in all) if (!(c is PressToEnterA)) keep++;
        if (keep == all.Length) return all;
        var result = new Component[keep];
        int i = 0;
        foreach (Component c in all) if (!(c is PressToEnterA)) result[i++] = c;
        return result;
    }

    // Only things that walk around get their position saved (the player is saved separately).
    private static bool TracksPosition(GameObject go) {
        if (go.CompareTag("Player") || go.GetComponent<Camera>() != null || go.transform is RectTransform) return false;
        return go.GetComponent<NpcMotorA>() != null || go.GetComponent<Rigidbody2D>() != null;
    }

    private static bool ReadComponent(Component c, out bool enabled, out bool isTrigger, out float alpha) {
        enabled = true; isTrigger = false; alpha = -1f;
        if (c == null) return false;
        if (c is Behaviour) enabled = ((Behaviour)c).enabled;
        else if (c is Collider2D) enabled = ((Collider2D)c).enabled;
        else if (c is Renderer) enabled = ((Renderer)c).enabled;
        else return false;
        if (c is Collider2D) isTrigger = ((Collider2D)c).isTrigger;
        if (c is CanvasGroup) alpha = ((CanvasGroup)c).alpha;
        return true;
    }

    private static void WriteComponent(Component c, ComponentState s) {
        if (c == null) return;
        if (s.enabled >= 0) {
            bool en = s.enabled == 1;
            if (c is Behaviour) ((Behaviour)c).enabled = en;
            else if (c is Collider2D) ((Collider2D)c).enabled = en;
            else if (c is Renderer) ((Renderer)c).enabled = en;
        }
        if (s.isTrigger >= 0 && c is Collider2D) ((Collider2D)c).isTrigger = s.isTrigger == 1;
        if (s.alpha >= 0f && c is CanvasGroup) ((CanvasGroup)c).alpha = s.alpha;
    }

    private string Describe(int id) {
        if (id < 0 || id >= objects.Count) return "#" + id;
        GameObject go = objects[id];
        return "#" + id + "(" + (go != null ? go.name : "destroyed") + ")";
    }

    #endregion

    #region Loading cover

    // Plain black screen while the scene is rebuilt, so the fresh scene never flashes before the restore.
    private void ShowLoadingCover(bool show) {
        if (show && loadingCover == null) {
            loadingCover = new GameObject("CheckpointLoadingCover");
            loadingCover.transform.SetParent(transform, false);
            Canvas canvas = loadingCover.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1000;
            Image img = new GameObject("Black", typeof(RectTransform)).AddComponent<Image>();
            img.transform.SetParent(loadingCover.transform, false);
            img.color = Color.black;
            RectTransform rt = img.rectTransform;
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one; rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
        }
        if (loadingCover != null) loadingCover.SetActive(show);
    }

    #endregion

    #region Value encoding

    private static string Encode(object v) {
        if (v == null) return "";
        if (v is Vector3) { Vector3 p = (Vector3)v; return string.Format(CultureInfo.InvariantCulture, "{0},{1},{2}", p.x, p.y, p.z); }
        if (v is float) return ((float)v).ToString("R", CultureInfo.InvariantCulture);
        return Convert.ToString(v, CultureInfo.InvariantCulture);
    }

    private static object Decode(string s, Type t) {
        if (t == typeof(int)) return int.Parse(s, CultureInfo.InvariantCulture);
        if (t == typeof(bool)) return bool.Parse(s);
        if (t == typeof(float)) return float.Parse(s, CultureInfo.InvariantCulture);
        if (t == typeof(string)) return s;
        if (t == typeof(Vector3)) {
            string[] p = s.Split(',');
            return new Vector3(float.Parse(p[0], CultureInfo.InvariantCulture), float.Parse(p[1], CultureInfo.InvariantCulture), float.Parse(p[2], CultureInfo.InvariantCulture));
        }
        throw new NotSupportedException("Cannot restore field of type " + t);
    }

    #endregion
}
