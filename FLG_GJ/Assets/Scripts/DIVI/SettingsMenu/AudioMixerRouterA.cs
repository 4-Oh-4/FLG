using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

/// <summary>
/// Sends every AudioSource that has no mixer group into the MasterMixer, so the
/// Music / SFX sliders in the settings panel actually affect the game.
/// Looping sources are treated as music, everything else as sound effects.
/// Sources that already have a mixer group are left alone.
/// </summary>
public class AudioMixerRouterA : MonoBehaviour
{
    [SerializeField] private AudioMixer masterMixer;
    [SerializeField] private string musicGroupName = "Music";
    [SerializeField] private string sfxGroupName = "Sfx";

    private AudioMixerGroup musicGroup;
    private AudioMixerGroup sfxGroup;

    private void Awake()
    {
        if (masterMixer == null) return;
        AudioMixerGroup[] music = masterMixer.FindMatchingGroups(musicGroupName);
        AudioMixerGroup[] sfx = masterMixer.FindMatchingGroups(sfxGroupName);
        if (music.Length > 0) musicGroup = music[0];
        if (sfx.Length > 0) sfxGroup = sfx[0];
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        RouteUnassignedSources();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RouteUnassignedSources();
    }

    public void RouteUnassignedSources()
    {
        if (musicGroup == null || sfxGroup == null) return;
        AudioSource[] sources = FindObjectsByType<AudioSource>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (AudioSource source in sources)
        {
            if (source.outputAudioMixerGroup != null) continue;
            source.outputAudioMixerGroup = source.loop ? musicGroup : sfxGroup;
        }
    }
}
