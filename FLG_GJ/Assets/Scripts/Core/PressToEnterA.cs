using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Turns walk-in building entrances into "press E to enter" doors, without changing what the doors do.
///
/// Attached at scene load to the entrance objects listed below. While the player stands in the
/// doorway and the story has that door open, an "E  Enter" prompt is shown; pressing E runs the
/// door's own enter logic. Each door script checks PressToEnterA.Allows(...) first, so simply
/// walking in no longer triggers it. Scripted story transitions (stairs, thug camp, church
/// time-skip, bathroom, final run) are not in the list and stay automatic.
/// </summary>
public class PressToEnterA : MonoBehaviour {
    private const BindingFlags AnyInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    private const string PromptResource = "InteractPrompt";

    /// <summary>True while an entrance prompt is on screen (E then means "enter", not "talk").</summary>
    public static bool PromptVisible { get; private set; }

    private static bool passingThrough; // set while E triggers the door's own logic
    private static GameObject promptInstance;
    private static PressToEnterA promptOwner;

    private MonoBehaviour door;
    private Collider2D doorCollider;
    private Collider2D playerCollider;
    private bool playerInDoorway;

    /// <summary>
    /// Called at the top of each entrance's enter handler. Non-player colliders (e.g. NPCs walking
    /// through) and doors without this component behave exactly as before.
    /// </summary>
    public static bool Allows(Component doorScript, Component other) {
        if (other == null || !other.CompareTag("Player")) return true;
        PressToEnterA p = doorScript.GetComponent<PressToEnterA>();
        if (p == null || !p.enabled) return true;
        return passingThrough;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void HandleStartingScene() => AttachTo(SceneManager.GetActiveScene());

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode) => AttachTo(scene);

    // The building entrances (by script type). LoadMiniGameA is also the thug camp, which stays automatic.
    private static void AttachTo(Scene scene) {
        if (!scene.isLoaded) return;
        foreach (GameObject root in scene.GetRootGameObjects()) {
            foreach (MonoBehaviour mb in root.GetComponentsInChildren<MonoBehaviour>(true)) {
                if (mb == null || !IsEntrance(mb)) continue;
                if (mb.GetComponent<PressToEnterA>() == null) mb.gameObject.AddComponent<PressToEnterA>().door = mb;
            }
        }
    }

    private static bool IsEntrance(MonoBehaviour mb) {
        switch (mb.GetType().Name) {
            case "LoadMiniGameA": return mb.gameObject.name == "Church";
            case "GateToGoUpAct1_A":
            case "ResetterAct1Home":
            case "GoHomeSCript":
            case "ClubEntryColliderA":
            case "DoorToNinoDoor":
            case "TunnelScriptAct2":
            case "GateColliderInWareHouse":
            case "TruckDoorA":
            case "ShackCollider":
            case "BigShackCollider":
            case "FinalShackColldier":
                return true;
            default: return false;
        }
    }

    private void Start() {
        if (door == null) {
            foreach (MonoBehaviour mb in GetComponents<MonoBehaviour>()) if (mb != this && IsEntrance(mb)) door = mb;
        }
        doorCollider = GetComponent<Collider2D>();
    }

    private void OnDisable() {
        playerInDoorway = false;
        if (promptOwner == this) ShowPrompt(false);
    }

    // The doorway can be a trigger (walk into it) or solid (the Act 1 gate: bump into it).
    // Enter/exit are used rather than stay: a player standing still stops producing stay events.
    private void OnTriggerEnter2D(Collider2D other) { if (other.CompareTag("Player")) { playerInDoorway = true; playerCollider = other; } }
    private void OnTriggerStay2D(Collider2D other) { if (other.CompareTag("Player")) { playerInDoorway = true; playerCollider = other; } }
    private void OnTriggerExit2D(Collider2D other) { if (other.CompareTag("Player")) playerInDoorway = false; }
    private void OnCollisionEnter2D(Collision2D c) { if (c.collider.CompareTag("Player")) { playerInDoorway = true; playerCollider = c.collider; } }
    private void OnCollisionStay2D(Collision2D c) { if (c.collider.CompareTag("Player")) { playerInDoorway = true; playerCollider = c.collider; } }
    private void OnCollisionExit2D(Collision2D c) { if (c.collider.CompareTag("Player")) playerInDoorway = false; }

    private void Update() {
        // Safety net in case an exit event was missed (collider toggled while overlapping, etc.).
        if (playerInDoorway && (playerCollider == null || !playerCollider.gameObject.activeInHierarchy || doorCollider == null
            || !Expanded(doorCollider.bounds, 0.75f).Intersects(playerCollider.bounds))) {
            playerInDoorway = false;
        }
        bool canUse = playerInDoorway && door != null && IsOpen() && PlayerCanAct();
        if (canUse) {
            if (promptOwner != this) ShowPrompt(true);
            if (Input.GetKeyDown(KeyCode.E)) Enter();
        } else if (promptOwner == this) {
            ShowPrompt(false);
        }
    }

    private void Enter() {
        ShowPrompt(false);
        passingThrough = true;
        try {
            if (door is GateToGoUpAct1_A gate) {
                gate.GoUp();
            } else {
                MethodInfo enter = door.GetType().GetMethod("OnTriggerEnter2D", AnyInstance);
                if (enter != null) enter.Invoke(door, new object[] { playerCollider });
            }
        } finally {
            passingThrough = false;
        }
    }

    private static bool PlayerCanAct() {
        if (PauseMenuManager.IsPaused || StoryCheckpointA.IsRestoring) return false;
        foreach (DialougeManagerA dm in FindObjectsByType<DialougeManagerA>(FindObjectsSortMode.None)) if (dm.IsOpen) return false;
        GameObject player = GameObject.FindWithTag("Player");
        PlayerMovement_A movement = player != null ? player.GetComponent<PlayerMovement_A>() : null;
        return movement == null || movement.canMove;
    }

    // Whether the story currently has this entrance open (same conditions the door script itself checks).
    private bool IsOpen() {
        if (doorCollider != null && !doorCollider.enabled) return false;
        string type = door.GetType().Name;
        if (type == "GateToGoUpAct1_A") return doorCollider != null && !doorCollider.isTrigger; // becomes solid when usable
        if (doorCollider != null && !doorCollider.isTrigger) return false;                      // trigger doors only work as triggers
        switch (type) {
            case "LoadMiniGameA": { int v = Int("once"); return v == 0 || v == 5; }
            case "GoHomeSCript": { int v = Int("once"); return v == 0 || v == 90; }
            case "ClubEntryColliderA": { int v = Int("i"); return v == 0 || v == 1; }
            case "DoorToNinoDoor": return Int("i") == 0;
            case "TunnelScriptAct2": return Int("count") == 0;
            case "GateColliderInWareHouse": return Int("once") == 0;
            case "TruckDoorA": return Int("once") == 0;
            case "ShackCollider": return Int("a") == 1;
            case "BigShackCollider": return Int("b") == 0;
            case "FinalShackColldier": return Int("h") == 0;
            default: return true; // ResetterAct1Home: the Home object is only switched on when it is time to go home
        }
    }

    private static Bounds Expanded(Bounds b, float amount) {
        b.Expand(amount);
        return b;
    }

    private int Int(string field) {
        FieldInfo f = door.GetType().GetField(field, AnyInstance);
        return f != null ? (int)f.GetValue(door) : -1;
    }

    private void ShowPrompt(bool show) {
        if (show) {
            if (promptInstance == null) {
                GameObject prefab = Resources.Load<GameObject>(PromptResource);
                if (prefab != null) {
                    promptInstance = Instantiate(prefab);
                    DontDestroyOnLoad(promptInstance);
                }
            }
            promptOwner = this;
        } else if (promptOwner == this) {
            promptOwner = null;
        }
        PromptVisible = promptOwner != null;
        if (promptInstance != null) promptInstance.SetActive(PromptVisible);
    }
}
