using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer masterMixer;

    [Header("UI Sliders")]
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    private bool isMusicMuted = false;
    private bool isSfxMuted = false;

    // Saved between sessions. Defaults to full volume so the game sounds the same as before routing.
    private const string MusicPrefKey = "MusicVolume";
    private const string SfxPrefKey = "SFXVolume";

    private void Start()
    {
        float music = PlayerPrefs.GetFloat(MusicPrefKey, 1f);
        musicVolumeSlider.value = music;
        SetMusicVolume(music);

        float sfx = PlayerPrefs.GetFloat(SfxPrefKey, 1f);
        sfxVolumeSlider.value = sfx;
        SetSFXVolume(sfx);
    }

    // --- Slider Functions ---
    public void SetMusicVolume(float volume)
    {
        masterMixer.SetFloat("MusicVolume", ToDecibels(volume));
        isMusicMuted = (volume <= 0.001f);
        PlayerPrefs.SetFloat(MusicPrefKey, volume);
    }

    public void SetSFXVolume(float volume)
    {
        masterMixer.SetFloat("SFXVolume", ToDecibels(volume));
        isSfxMuted = (volume <= 0.001f);
        PlayerPrefs.SetFloat(SfxPrefKey, volume);
    }

    // Log10(0) is -infinity, so treat 0 as silence (-80 dB, the mixer's minimum).
    private static float ToDecibels(float volume)
    {
        return volume <= 0.0001f ? -80f : Mathf.Log10(volume) * 20f;
    }

    // --- Button Toggle Functions ---
    public void ToggleMusic()
    {
        isMusicMuted = !isMusicMuted;
        float newVolume = isMusicMuted ? 0f : 1f;
        musicVolumeSlider.value = newVolume;
    }

    public void ToggleSFX()
    {
        isSfxMuted = !isSfxMuted;
        float newVolume = isSfxMuted ? 0f : 1f;
        sfxVolumeSlider.value = newVolume;
    }
}