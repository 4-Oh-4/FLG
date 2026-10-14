using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Adds a small distance readout ("24 m") next to the quest arrow while the objective is off-screen.
/// Attached at scene load to every WindowQuestPointer_A; the pointer itself is untouched. The distance
/// is to wherever the arrow points (for Act 1 that can be the stairs or the gate first).
/// </summary>
public class QuestDistanceA : MonoBehaviour {
    private const float Inset = 58f;     // how far inside the arrow the number sits (canvas pixels)
    private const float FontSize = 26f;

    private WindowQuestPointer_A pointer;
    private TextMeshProUGUI label;
    private RectTransform badge;
    private Transform player;
    private int shown = -1;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void HandleStartingScene() => Attach(SceneManager.GetActiveScene());

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode) => Attach(scene);

    private static void Attach(Scene scene) {
        if (!scene.isLoaded) return;
        foreach (GameObject root in scene.GetRootGameObjects()) {
            foreach (WindowQuestPointer_A p in root.GetComponentsInChildren<WindowQuestPointer_A>(true)) {
                if (p.GetComponent<QuestDistanceA>() == null) p.gameObject.AddComponent<QuestDistanceA>().pointer = p;
            }
        }
    }

    private void LateUpdate() {
        if (pointer == null) pointer = GetComponent<WindowQuestPointer_A>();
        if (pointer == null || pointer.pointerImage == null) return;
        if (label == null) Build();
        if (label == null) return;

        Image arrow = pointer.pointerImage;
        bool arrowVisible = pointer.isActiveAndEnabled && pointer.target != null && arrow.gameObject.activeInHierarchy;
        Vector3 goal = arrowVisible ? WaypointRouterA.Resolve(pointer.target).position : Vector3.zero;
        bool offscreen = arrowVisible && IsOffscreen(goal);
        Transform who = Player();
        if (!offscreen || who == null || StoryCheckpointA.IsRestoring) {
            if (badge.gameObject.activeSelf) badge.gameObject.SetActive(false);
            return;
        }

        int metres = Mathf.RoundToInt(Vector2.Distance(who.position, goal));
        if (metres != shown) {
            shown = metres;
            label.text = metres + " m";
        }

        // Sit just inside the arrow, towards the middle of the screen.
        Vector3 arrowPos = arrow.rectTransform.position;
        Vector3 centre = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, arrowPos.z);
        Vector3 inward = (centre - arrowPos).normalized;
        float scale = label.canvas != null ? label.canvas.scaleFactor : 1f;
        badge.position = arrowPos + inward * Inset * scale;
        if (!badge.gameObject.activeSelf) badge.gameObject.SetActive(true);
    }

    // Same test the pointer uses to switch to its edge arrow (its two sprites can be identical).
    private bool IsOffscreen(Vector3 worldPos) {
        Camera cam = pointer.mainCamera != null ? pointer.mainCamera : Camera.main;
        if (cam == null) return false;
        Vector3 s = cam.WorldToScreenPoint(worldPos);
        Vector2 pad = pointer.screenPadding;
        return s.z < 0 || s.x < pad.x || s.x > Screen.width - pad.x || s.y < pad.y || s.y > Screen.height - pad.y;
    }

    private void Build() {
        Canvas canvas = pointer.pointerImage.canvas;
        if (canvas == null) return;
        var go = new GameObject("QuestDistance", typeof(RectTransform));
        go.transform.SetParent(canvas.rootCanvas.transform, false);
        badge = (RectTransform)go.transform;
        badge.sizeDelta = new Vector2(92f, 36f);
        Image back = go.AddComponent<Image>();
        back.color = new Color(0f, 0f, 0f, 0.6f);
        back.raycastTarget = false;
        var text = new GameObject("Text", typeof(RectTransform));
        text.transform.SetParent(go.transform, false);
        label = text.AddComponent<TextMeshProUGUI>();
        label.font = TMP_Settings.defaultFontAsset;
        label.fontSize = FontSize;
        label.fontStyle = FontStyles.Bold;
        label.alignment = TextAlignmentOptions.Center;
        label.color = new Color(1f, 1f, 1f, 0.95f);
        label.raycastTarget = false;
        label.enableWordWrapping = false;
        label.rectTransform.anchorMin = Vector2.zero;
        label.rectTransform.anchorMax = Vector2.one;
        label.rectTransform.offsetMin = label.rectTransform.offsetMax = Vector2.zero;
        go.SetActive(false);
    }

    private Transform Player() {
        if (player != null && player.gameObject.activeInHierarchy) return player;
        player = null;
        Scene active = SceneManager.GetActiveScene();
        foreach (PlayerMovement_A pm in FindObjectsByType<PlayerMovement_A>(FindObjectsSortMode.None)) {
            if (pm.gameObject.scene == active) { player = pm.transform; break; }
        }
        return player;
    }

    private void OnDisable() {
        if (badge != null) badge.gameObject.SetActive(false);
    }
}
