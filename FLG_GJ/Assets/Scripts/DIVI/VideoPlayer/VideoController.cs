using UnityEngine;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    void OnEnable()
    {
        // Subscribe to the loopPointReached event
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    void OnDisable()
    {
        // Unsubscribe to the loopPointReached event
        videoPlayer.loopPointReached -= OnVideoFinished;
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        FindAnyObjectByType<ResetterAct2Home>().ResetGame();
        // This function will be called when the video finishes playing
        Debug.Log("The video has finished playing.");
    }
}