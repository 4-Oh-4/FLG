using System;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Read-only HUD for the two action minigames that show nothing on screen:
///  - TopDownShooter_Drugs_ACT2: Carlo's health, the package's health, ammo, wave.
///  - TruckChase_ACT2: the truck's health, wave.
/// It only reads the minigames' own values; nothing about how they play is changed.
/// </summary>
public class MinigameHudA : MonoBehaviour {
    private const BindingFlags AnyInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    private static readonly Color Cream = new Color(0.95f, 0.92f, 0.85f);
    private static readonly Color Good = new Color(0.45f, 0.80f, 0.40f);
    private static readonly Color Warn = new Color(0.95f, 0.75f, 0.25f);
    private static readonly Color Bad = new Color(0.90f, 0.30f, 0.25f);

    private class Readout {
        public string label;
        public Func<float> value, max;
        public bool asBar;
        public Image fill;
        public TMP_Text text;
    }

    private readonly List<Readout> readouts = new List<Readout>();
    private Scene minigame;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (scene.name != "TopDownShooter_Drugs_ACT2" && scene.name != "TruckChase_ACT2") return;
        var go = new GameObject("MinigameHud");
        SceneManager.MoveGameObjectToScene(go, scene); // goes away with the minigame
        var hud = go.AddComponent<MinigameHudA>();
        hud.minigame = scene;
        hud.Setup();
    }

    private void Setup() {
        if (minigame.name == "TopDownShooter_Drugs_ACT2") {
            Component player = Find("PlayerController_D");
            Component package = Find("Package_D");
            Component waves = Find("WaveSpawner_D");
            Add("CARLO", player, "currentHealth", "maxHealth", true);
            Add("PACKAGE", package, "currentHealth", "maxHealth", true);
            Add("AMMO", player, "currentAmmo", "maxAmmo", false);
            AddWave(waves);
        } else {
            Component truck = Find("LorryHealth_D");
            Component waves = Find("WaveSpawner_D");
            Add("TRUCK", truck, "currentHealth", "maxHealth", true);
            AddWave(waves);
        }
        BuildUI();
    }

    private Component Find(string typeName) {
        foreach (GameObject root in minigame.GetRootGameObjects()) {
            foreach (MonoBehaviour mb in root.GetComponentsInChildren<MonoBehaviour>(true)) {
                if (mb != null && mb.GetType().Name == typeName) return mb;
            }
        }
        return null;
    }

    private void Add(string label, Component owner, string valueField, string maxField, bool asBar) {
        if (owner == null) return;
        FieldInfo v = owner.GetType().GetField(valueField, AnyInstance);
        FieldInfo m = owner.GetType().GetField(maxField, AnyInstance);
        if (v == null || m == null) return;
        readouts.Add(new Readout {
            label = label, asBar = asBar,
            value = () => owner != null ? Convert.ToSingle(v.GetValue(owner)) : 0f,
            max = () => owner != null ? Convert.ToSingle(m.GetValue(owner)) : 1f,
        });
    }

    private void AddWave(Component spawner) {
        if (spawner == null) return;
        FieldInfo index = spawner.GetType().GetField("currentWaveIndex", AnyInstance);
        FieldInfo list = spawner.GetType().GetField("waves", AnyInstance);
        if (index == null || list == null) return;
        readouts.Add(new Readout {
            label = "WAVE", asBar = false,
            value = () => spawner != null ? (int)index.GetValue(spawner) + 1 : 0,
            max = () => spawner != null && list.GetValue(spawner) is Array a ? a.Length : 1,
        });
    }

    private void BuildUI() {
        if (readouts.Count == 0) return;
        var canvasGo = new GameObject("HudCanvas", typeof(RectTransform));
        canvasGo.transform.SetParent(transform, false);
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 80;
        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        const float rowHeight = 40f, width = 380f;
        var panel = NewImage("Panel", canvasGo.transform, new Color(0f, 0f, 0f, 0.55f)).rectTransform;
        panel.anchorMin = panel.anchorMax = panel.pivot = new Vector2(0f, 0f);
        panel.anchoredPosition = new Vector2(28f, 28f);
        panel.sizeDelta = new Vector2(width, rowHeight * readouts.Count + 20f);

        for (int i = 0; i < readouts.Count; i++) {
            Readout r = readouts[i];
            float y = -10f - i * rowHeight;
            var label = NewText(r.label, panel, 24f, TextAlignmentOptions.MidlineLeft);
            Place(label.rectTransform, new Vector2(16f, y), new Vector2(130f, rowHeight));
            if (r.asBar) {
                var back = NewImage("Back", panel, new Color(1f, 1f, 1f, 0.15f));
                Place(back.rectTransform, new Vector2(140f, y - 9f), new Vector2(width - 160f, rowHeight - 18f));
                r.fill = NewImage("Fill", back.transform, Good);
                r.fill.rectTransform.anchorMin = Vector2.zero;
                r.fill.rectTransform.anchorMax = new Vector2(1f, 1f);
                r.fill.rectTransform.pivot = new Vector2(0f, 0.5f);
                r.fill.rectTransform.offsetMin = r.fill.rectTransform.offsetMax = Vector2.zero;
            } else {
                r.text = NewText("", panel, 24f, TextAlignmentOptions.MidlineLeft);
                Place(r.text.rectTransform, new Vector2(140f, y), new Vector2(width - 160f, rowHeight));
            }
        }
    }

    private void Update() {
        foreach (Readout r in readouts) {
            float max = Mathf.Max(1f, r.max());
            float value = Mathf.Clamp(r.value(), 0f, max);
            float t = value / max;
            if (r.fill != null) {
                r.fill.rectTransform.anchorMax = new Vector2(t, 1f);
                r.fill.color = t > 0.5f ? Good : t > 0.25f ? Warn : Bad;
            }
            if (r.text != null) {
                r.text.text = Mathf.RoundToInt(value) + " / " + Mathf.RoundToInt(max);
                r.text.color = r.label == "AMMO" && t <= 0.2f ? Bad : Cream;
            }
        }
    }

    private static void Place(RectTransform rt, Vector2 topLeft, Vector2 size) {
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0f, 1f);
        rt.anchoredPosition = topLeft;
        rt.sizeDelta = size;
    }

    private static Image NewImage(string name, Transform parent, Color color) {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var img = go.AddComponent<Image>();
        img.color = color;
        img.raycastTarget = false;
        return img;
    }

    private static TMP_Text NewText(string text, Transform parent, float size, TextAlignmentOptions align) {
        var go = new GameObject("Text", typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var t = go.AddComponent<TextMeshProUGUI>();
        t.text = text;
        t.fontSize = size;
        t.fontStyle = FontStyles.Bold;
        t.color = Cream;
        t.alignment = align;
        t.raycastTarget = false;
        t.enableWordWrapping = false;
        return t;
    }
}
