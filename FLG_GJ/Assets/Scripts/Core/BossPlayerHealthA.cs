using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Gives Carlo health in the final fight against Darius. Darius's projectiles and bombs already
/// detect hitting the player (they only logged it); they now call Hit(). Carlo has a row of hearts
/// mirroring Darius's (same heart art), a moment of invulnerability after each hit, and if he runs out
/// the fight restarts (the briefing then says "Try again."). Winning works exactly as before.
/// </summary>
public class BossPlayerHealthA : MonoBehaviour {
    private const string BossScene = "FinalBossBattleA";
    private const int MaxHealth = 5;
    private const float Invulnerable = 1.2f;
    private const BindingFlags AnyInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    private int health = MaxHealth;
    private float safeUntil;
    private bool down;
    private SpriteRenderer sprite;
    private readonly List<Image> hearts = new List<Image>();
    private Sprite fullHeart, emptyHeart;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void HandleStartingScene() => OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (scene.name != BossScene) return;
        foreach (GameObject root in scene.GetRootGameObjects()) {
            if (!root.CompareTag("Player") || root.GetComponent<BossPlayerHealthA>() != null) continue;
            root.AddComponent<BossPlayerHealthA>();
        }
    }

    /// <summary>Called by Darius's projectiles and bombs when they touch the player.</summary>
    public static void Hit(GameObject target, int damage) {
        if (target == null) return;
        BossPlayerHealthA h = target.GetComponentInParent<BossPlayerHealthA>();
        if (h != null) h.TakeDamage(damage);
    }

    private void Start() {
        sprite = GetComponent<SpriteRenderer>();
        BuildHearts();
    }

    private void TakeDamage(int damage) {
        if (down || Time.time < safeUntil || damage <= 0) return;
        health = Mathf.Max(0, health - damage);
        safeUntil = Time.time + Invulnerable;
        RefreshHearts();
        if (health == 0) {
            down = true;
            StartCoroutine(Defeated());
        } else {
            StartCoroutine(Blink());
        }
    }

    private IEnumerator Blink() {
        if (sprite == null) yield break;
        Color baseColor = sprite.color;
        while (Time.time < safeUntil) {
            sprite.color = new Color(1f, 0.35f, 0.35f, 0.35f);
            yield return new WaitForSeconds(0.08f);
            sprite.color = baseColor;
            yield return new WaitForSeconds(0.08f);
        }
        sprite.color = baseColor;
    }

    // Carlo goes down: stop him, say so, and start the fight again.
    private IEnumerator Defeated() {
        PlayerMovement_A move = GetComponent<PlayerMovement_A>();
        if (move != null) move.canMove = false;
        PlayerAttackA attack = GetComponent<PlayerAttackA>();
        if (attack != null) attack.enabled = false;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;
        if (sprite != null) sprite.color = new Color(1f, 0.35f, 0.35f, 1f);
        ShowBanner("DOWN");
        yield return new WaitForSecondsRealtime(1.6f);
        SceneManager.LoadScene(BossScene);
    }

    private void BuildHearts() {
        StationaryBossA boss = FindAnyObjectByType<StationaryBossA>();
        if (boss == null) return;
        GameObject heartPrefab = Field<GameObject>(boss, "heartPrefab");
        Transform bossBar = Field<Transform>(boss, "healthBarParent");
        fullHeart = Field<Sprite>(boss, "fullHeartSprite");
        emptyHeart = Field<Sprite>(boss, "emptyHeartSprite");
        if (heartPrefab == null || bossBar == null) return;

        // Mirror Darius's bar in the top-right corner of the same canvas.
        var bar = new GameObject("CarloHealthBar", typeof(RectTransform));
        bar.transform.SetParent(bossBar.parent, false);
        var rt = (RectTransform)bar.transform;
        var bossRt = (RectTransform)bossBar;
        FieldInfo bossMaxField = boss.GetType().GetField("maxHealth", AnyInstance);
        int bossMax = bossMaxField != null ? Mathf.Max(1, (int)bossMaxField.GetValue(boss)) : 10;
        rt.anchorMin = rt.anchorMax = new Vector2(1f, 1f);
        rt.pivot = new Vector2(1f, bossRt.pivot.y);
        rt.sizeDelta = new Vector2(bossRt.sizeDelta.x * MaxHealth / bossMax, bossRt.sizeDelta.y);
        rt.anchoredPosition = new Vector2(-30f, bossRt.anchoredPosition.y);
        HorizontalLayoutGroup bossLayout = bossBar.GetComponent<HorizontalLayoutGroup>();
        var layout = bar.AddComponent<HorizontalLayoutGroup>();
        layout.childAlignment = TextAnchor.MiddleRight;
        layout.spacing = bossLayout != null ? bossLayout.spacing : 4f;
        layout.childControlWidth = bossLayout != null && bossLayout.childControlWidth;
        layout.childControlHeight = bossLayout != null && bossLayout.childControlHeight;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        for (int i = 0; i < MaxHealth; i++) {
            GameObject h = Instantiate(heartPrefab, bar.transform);
            Image img = h.GetComponent<Image>();
            if (img != null) hearts.Add(img);
        }

        var label = new GameObject("Label", typeof(RectTransform));
        label.transform.SetParent(bar.transform.parent, false);
        var lrt = (RectTransform)label.transform;
        lrt.anchorMin = lrt.anchorMax = new Vector2(1f, 1f);
        lrt.pivot = new Vector2(1f, 1f);
        lrt.sizeDelta = new Vector2(200f, 30f);
        lrt.anchoredPosition = new Vector2(rt.anchoredPosition.x, rt.anchoredPosition.y - rt.sizeDelta.y * 0.5f - 4f);
        var text = label.AddComponent<TextMeshProUGUI>();
        text.text = "CARLO";
        text.fontSize = 18f;
        text.fontStyle = FontStyles.Bold;
        text.alignment = TextAlignmentOptions.TopRight;
        text.raycastTarget = false;
        RefreshHearts();
    }

    private void RefreshHearts() {
        for (int i = 0; i < hearts.Count; i++) {
            Sprite s = i < health ? fullHeart : emptyHeart;
            if (s != null) hearts[i].sprite = s;
            else hearts[i].color = i < health ? Color.white : new Color(1f, 1f, 1f, 0.25f);
        }
    }

    private void ShowBanner(string message) {
        var go = new GameObject("BossDownBanner", typeof(RectTransform));
        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 310;
        var scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        var dim = new GameObject("Dim", typeof(RectTransform)).AddComponent<Image>();
        dim.transform.SetParent(go.transform, false);
        dim.color = new Color(0f, 0f, 0f, 0.6f);
        var drt = dim.rectTransform; drt.anchorMin = Vector2.zero; drt.anchorMax = Vector2.one; drt.offsetMin = drt.offsetMax = Vector2.zero;
        var t = new GameObject("Text", typeof(RectTransform)).AddComponent<TextMeshProUGUI>();
        t.transform.SetParent(go.transform, false);
        t.text = message;
        t.fontSize = 96f;
        t.fontStyle = FontStyles.Bold;
        t.alignment = TextAlignmentOptions.Center;
        t.color = new Color(0.95f, 0.3f, 0.25f);
        var trt = t.rectTransform; trt.anchorMin = Vector2.zero; trt.anchorMax = Vector2.one; trt.offsetMin = trt.offsetMax = Vector2.zero;
    }

    private static T Field<T>(object owner, string name) where T : class {
        FieldInfo f = owner.GetType().GetField(name, AnyInstance);
        return f != null ? f.GetValue(owner) as T : null;
    }
}
