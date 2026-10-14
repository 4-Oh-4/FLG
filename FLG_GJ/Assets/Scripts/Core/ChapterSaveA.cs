using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Remembers the last chapter the player reached, so the main menu can offer "Continue".
/// A chapter is saved when its scene is loaded on its own (LoadSceneMode.Single). Each of these
/// scenes already starts from a clean state in the normal story flow (the resetters destroy the
/// previous act's StoryManager first), so loading one directly gives the same starting point.
/// </summary>
public static class ChapterSaveA {
    private const string SceneKey = "ChapterSave.Scene";

    // Scene name and the label shown in the menu, in story order.
    private static readonly string[] chapterScenes = { "Act_1_Map_K", "Carlos_house", "Act_2_Map_New", "Act_3_Map_New 1" };
    private static readonly string[] chapterLabels = { "Act 1", "Act 2 - Home", "Act 2", "Act 3" };

    public static string SavedScene => PlayerPrefs.GetString(SceneKey, "");

    public static bool HasSave => !string.IsNullOrEmpty(SavedScene) && Application.CanStreamedLevelBeLoaded(SavedScene);

    public static string SavedChapterLabel {
        get {
            int i = Array.IndexOf(chapterScenes, SavedScene);
            return i >= 0 ? chapterLabels[i] : "";
        }
    }

    public static void DeleteSave() {
        PlayerPrefs.DeleteKey(SceneKey);
        PlayerPrefs.Save();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (mode != LoadSceneMode.Single) return; // minigames and house interiors load additively
        if (Array.IndexOf(chapterScenes, scene.name) < 0) return;
        PlayerPrefs.SetString(SceneKey, scene.name);
        PlayerPrefs.Save();
    }
}
