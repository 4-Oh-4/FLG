using UnityEngine;
using UnityEngine.Audio; // Required for using Audio Mixers
using UnityEngine.UI;    // Required for accessing the Slider component

public class AudioManager : MonoBehaviour
{
    [Header("Audio Mixer")]
    [Tooltip("Reference to the Master Audio Mixer for the game.")]
    [SerializeField] private AudioMixer masterMixer;

    [Header("UI Sliders")]
    [Tooltip("The slider that controls music volume.")]
    [SerializeField] private Slider musicVolumeSlider;
    [Tooltip("The slider that controls SFX volume.")]
    [SerializeField] private Slider sfxVolumeSlider;

    // Private variables to track the on/off state for the toggles.
    private bool isMusicMuted = false;
    private bool isSfxMuted = false;

    // --- Slider Functions ---

    // This is called by the music slider's "On Value Changed" event.
    public void SetMusicVolume(float volume)
    {
        // This formula converts the slider's 0-1 value to the mixer's decibel range.
        masterMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        isMusicMuted = (volume <= 0.001f);
    }

    // This is called by the SFX slider's "On Value Changed" event.
    public void SetSFXVolume(float volume)
    {
        masterMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
        isSfxMuted = (volume <= 0.001f);
    }

    // --- Button Toggle Functions ---

    // This is called by the "MUSIC" button's OnClick event.
    public void ToggleMusic()
    {
        isMusicMuted = !isMusicMuted;
        float newVolume = isMusicMuted ? 0f : 1f;

        // Set the slider's value, which will automatically call SetMusicVolume.
        musicVolumeSlider.value = newVolume;
    }

    // This is called by the "SFX" button's OnClick event.
    public void ToggleSFX()
    {
        isSfxMuted = !isSfxMuted;
        float newVolume = isSfxMuted ? 0f : 1f;
        sfxVolumeSlider.value = newVolume;
    }
}