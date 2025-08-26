using UnityEngine;
using UnityEngine.Audio; // Required for using Audio Mixers

public class AudioManager : MonoBehaviour
{
    [Tooltip("Reference to the Master Audio Mixer for the game.")]
    [SerializeField] private AudioMixer masterMixer;

    // This function will be called by the music volume slider.
    public void SetMusicVolume(float volume)
    {
        // The mixer uses a logarithmic scale (decibels), not a linear 0-1 scale.
        // This formula converts the slider's 0-1 value to the mixer's decibel range.
        masterMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
    }

    // This function will be called by the SFX volume slider.
    public void SetSFXVolume(float volume)
    {
        masterMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
    }
}