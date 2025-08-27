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

    // AMENDED: Added a Start() method to set default volumes.
    private void Start()
    {
        // Set the Music slider to 70% and update the mixer.
        musicVolumeSlider.value = 0.7f;
        SetMusicVolume(0.7f);

        // Set the SFX slider to 75% and update the mixer.
        sfxVolumeSlider.value = 0.75f;
        SetSFXVolume(0.75f);
    }

    // --- Slider Functions ---
    public void SetMusicVolume(float volume)
    {
        masterMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        isMusicMuted = (volume <= 0.001f);
    }

    public void SetSFXVolume(float volume)
    {
        masterMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
        isSfxMuted = (volume <= 0.001f);
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