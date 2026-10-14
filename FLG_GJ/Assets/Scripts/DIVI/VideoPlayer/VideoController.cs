using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    [Header("Skipping")]
    [Tooltip("How long Space / Enter / Escape must be held to skip the cutscene.")]
    [SerializeField] private float holdToSkipSeconds = 1f;
    [Tooltip("How long the skip hint stays visible after the last key press.")]
    [SerializeField] private float hintVisibleSeconds = 2.5f;

    // Prefab in a Resources folder with a CanvasGroup on the root and an Image named "Fill" (Filled type).
    private const string SkipHintResource = "CutsceneSkipHint";

    private bool finished;
    private float holdTime;
    private float hintTimer;
    private CanvasGroup hintGroup;
    private Image hintFill;

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

    void Start()
    {
        GameObject prefab = Resources.Load<GameObject>(SkipHintResource);
        if (prefab == null) return;
        GameObject hint = Instantiate(prefab);
        hintGroup = hint.GetComponent<CanvasGroup>();
        Transform fill = hint.transform.Find("Panel/Fill");
        if (fill != null) hintFill = fill.GetComponent<Image>();
        if (hintGroup != null) hintGroup.alpha = 0f;
    }

    void Update()
    {
        if (finished) return;

        bool holding = Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.Escape);
        if (Input.anyKeyDown || holding) hintTimer = hintVisibleSeconds;

        holdTime = holding ? holdTime + Time.unscaledDeltaTime : 0f;
        hintTimer -= Time.unscaledDeltaTime;

        if (hintGroup != null) hintGroup.alpha = Mathf.MoveTowards(hintGroup.alpha, hintTimer > 0f ? 1f : 0f, Time.unscaledDeltaTime * 4f);
        if (hintFill != null) hintFill.fillAmount = Mathf.Clamp01(holdTime / holdToSkipSeconds);

        if (holdTime >= holdToSkipSeconds)
        {
            Debug.Log("Cutscene skipped.");
            OnVideoFinished(videoPlayer);
        }
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        if (finished) return; // skipping and the video ending on the same frame must only load once
        finished = true;
        FindAnyObjectByType<ResetterAct2Home>().ResetGame();
        // This function will be called when the video finishes playing
        Debug.Log("The video has finished playing.");
    }
}
