using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Corner minimap for the act maps. Builds itself at startup (no scene setup needed):
/// a second camera follows the act's player and renders into a small texture shown
/// bottom-right, with a dot for the player and a marker for the current quest target
/// (pinned to the edge, pointing the way, when the target is off the map).
/// Hidden in minigames, house interiors, cutscenes and menus. M toggles it.
/// </summary>
public class MinimapA : MonoBehaviour {
    private static readonly string[] actScenes = { "Act_1_Map_K", "Act_2_Map_New", "Act_3_Map_New 1" };

    private const float ViewSize = 22f;          // orthographic size of the minimap camera (main camera is ~5)
    private const int TextureSize = 256;
    private const float PanelSize = 260f;        // in 1920x1080 reference pixels
    private const float Margin = 28f;
    private const string VisiblePrefKey = "MinimapVisible";

    private static readonly Color Cream = new Color(0.93f, 0.89f, 0.80f);
    private static readonly Color Accent = new Color(0.82f, 0.22f, 0.18f);

    private static MinimapA instance;

    private Camera mapCamera;
    private RenderTexture texture;
    private GameObject panel;
    private RectTransform mapArea;
    private RectTransform targetMarker;
    private RectTransform targetArrow;
    private bool userWantsVisible = true;
    private Transform player;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Create() {
        if (instance != null) return;
        var go = new GameObject("Minimap");
        DontDestroyOnLoad(go);
        instance = go.AddComponent<MinimapA>();
    }

    private void Awake() {
        userWantsVisible = PlayerPrefs.GetInt(VisiblePrefKey, 1) == 1;
        BuildCamera();
        BuildUI();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (texture != null) texture.Release();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (mode == LoadSceneMode.Single) player = null; // find the new act's player
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.M) && !PauseMenuManager.IsPaused && ShouldExist()) {
            userWantsVisible = !userWantsVisible;
            PlayerPrefs.SetInt(VisiblePrefKey, userWantsVisible ? 1 : 0);
        }
    }

    private void LateUpdate() {
        bool show = userWantsVisible && ShouldExist();
        if (panel.activeSelf != show) panel.SetActive(show);
        if (!show) {
            mapCamera.enabled = false;
            return;
        }

        Vector3 p = player.position;
        mapCamera.transform.position = new Vector3(p.x, p.y, -50f);
        // Render at half rate; the texture keeps the last image in between.
        mapCamera.enabled = (Time.frameCount & 1) == 0;

        UpdateTargetMarker(p);
    }

    // Visible only in an act map while its own player is the one being controlled.
    private bool ShouldExist() {
        Scene active = SceneManager.GetActiveScene();
        if (System.Array.IndexOf(actScenes, active.name) < 0) return false;
        if (SceneManager.sceneCount != 1) return false; // minigame or house interior on top
        if (StoryCheckpointA.IsRestoring) return false;
        if (player == null) {
            foreach (PlayerMovement_A pm in FindObjectsByType<PlayerMovement_A>(FindObjectsSortMode.None)) {
                if (pm.gameObject.scene == active) { player = pm.transform; break; }
            }
        }
        return player != null && player.gameObject.activeInHierarchy;
    }

    private void UpdateTargetMarker(Vector3 playerPos) {
        Transform target = null;
        foreach (WindowQuestPointer_A pointer in FindObjectsByType<WindowQuestPointer_A>(FindObjectsSortMode.None)) {
            if (pointer.isActiveAndEnabled && pointer.target != null) { target = pointer.target; break; }
        }
        if (target == null) {
            targetMarker.gameObject.SetActive(false);
            targetArrow.gameObject.SetActive(false);
            return;
        }

        // World offset -> position inside the square map (half size = ViewSize world units).
        Vector2 offset = (Vector2)(target.position - playerPos);
        float half = mapArea.rect.width * 0.5f;
        Vector2 local = offset / ViewSize * half;
        float edge = half - 10f;
        bool inside = Mathf.Abs(local.x) <= edge && Mathf.Abs(local.y) <= edge;

        targetMarker.gameObject.SetActive(inside);
        targetArrow.gameObject.SetActive(!inside);
        if (inside) {
            targetMarker.anchoredPosition = local;
        } else {
            // Pin to the square's edge in the target's direction and point the arrow outwards.
            float scale = edge / Mathf.Max(Mathf.Abs(local.x), Mathf.Abs(local.y));
            targetArrow.anchoredPosition = local * scale;
            targetArrow.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(local.y, local.x) * Mathf.Rad2Deg - 90f);
        }
    }

    private void BuildCamera() {
        texture = new RenderTexture(TextureSize, TextureSize, 16) { name = "MinimapTexture", filterMode = FilterMode.Bilinear };
        texture.Create();

        var camGo = new GameObject("MinimapCamera");
        camGo.transform.SetParent(transform, false);
        mapCamera = camGo.AddComponent<Camera>();
        mapCamera.orthographic = true;
        mapCamera.orthographicSize = ViewSize;
        mapCamera.clearFlags = CameraClearFlags.SolidColor;
        mapCamera.backgroundColor = new Color(0.08f, 0.08f, 0.09f);
        mapCamera.cullingMask = ~(1 << LayerMask.NameToLayer("UI"));
        mapCamera.targetTexture = texture;
        mapCamera.depth = -100;
        mapCamera.enabled = false;
    }

    private void BuildUI() {
        var canvasGo = new GameObject("MinimapCanvas", typeof(RectTransform));
        canvasGo.transform.SetParent(transform, false);
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 90; // above the game HUD, below the pause menu (500)
        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        // Frame
        RectTransform frame = NewImage("Frame", canvasGo.transform, Cream).rectTransform;
        frame.anchorMin = frame.anchorMax = frame.pivot = new Vector2(1f, 0f);
        frame.anchoredPosition = new Vector2(-Margin, Margin);
        frame.sizeDelta = new Vector2(PanelSize, PanelSize);
        panel = frame.gameObject;

        // Dark inner border, then the map itself
        RectTransform inner = NewImage("Inner", frame, new Color(0f, 0f, 0f, 0.85f)).rectTransform;
        Stretch(inner, 3f);
        var raw = new GameObject("Map", typeof(RectTransform)).AddComponent<RawImage>();
        raw.transform.SetParent(inner, false);
        raw.texture = texture;
        raw.raycastTarget = false;
        mapArea = raw.rectTransform;
        Stretch(mapArea, 3f);

        Sprite knob = MakeShapeSprite(false);

        // Player dot (dark ring + cream centre)
        Image ring = NewImage("PlayerRing", mapArea, new Color(0f, 0f, 0f, 0.8f));
        ring.sprite = knob;
        ring.rectTransform.sizeDelta = new Vector2(16f, 16f);
        Image dot = NewImage("PlayerDot", ring.transform, Cream);
        dot.sprite = knob;
        dot.rectTransform.sizeDelta = new Vector2(10f, 10f);

        // Quest target: dot when visible on the map, arrow pinned to the edge when not
        Image marker = NewImage("TargetMarker", mapArea, Accent);
        marker.sprite = knob;
        marker.rectTransform.sizeDelta = new Vector2(14f, 14f);
        targetMarker = marker.rectTransform;

        Image arrow = NewImage("TargetArrow", mapArea, Accent);
        arrow.sprite = MakeShapeSprite(true);
        arrow.rectTransform.sizeDelta = new Vector2(18f, 18f);
        targetArrow = arrow.rectTransform;

        panel.SetActive(false);
    }

    private static Image NewImage(string name, Transform parent, Color color) {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var img = go.AddComponent<Image>();
        img.color = color;
        img.raycastTarget = false;
        return img;
    }

    private static void Stretch(RectTransform rt, float inset) {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = new Vector2(inset, inset);
        rt.offsetMax = new Vector2(-inset, -inset);
    }

    // White circle, or a triangle pointing up (+Y), generated so no art asset is needed.
    private static Sprite MakeShapeSprite(bool triangle) {
        const int size = 32;
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false) { filterMode = FilterMode.Bilinear };
        var pixels = new Color32[size * size];
        float c = (size - 1) * 0.5f;
        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                bool inside = triangle
                    ? Mathf.Abs(x - c) <= (size - 1 - y) * 0.5f            // wide at the bottom row, tip at the top
                    : (x - c) * (x - c) + (y - c) * (y - c) <= c * c;
                pixels[y * size + x] = inside ? new Color32(255, 255, 255, 255) : new Color32(255, 255, 255, 0);
            }
        }
        tex.SetPixels32(pixels);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }
}
