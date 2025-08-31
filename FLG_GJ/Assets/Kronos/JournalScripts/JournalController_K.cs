using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class JournalController : MonoBehaviour
{
    public CanvasGroup fadeGroup;   // assign your Black Image's CanvasGroup here
    public Button continueButton;   // assign your Continue Button here
    public float fadeDuration = 2f; // seconds to fade out
    public GameObject journal;
    public PlayerMovement_A playerMovement;
    void Start()
    {
        // Hook the button click to trigger FadeOut
        //continueButton.onClick.AddListener(() => StartCoroutine(FadeOut()));
    }
    public void fadeout() {
        StopAllCoroutines();
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }
        fadeGroup.alpha = 1f;
        StoryManagertAct1A.Instance.SetFlag("JournalDone", true);
        playerMovement.canMove = true;
        journal.SetActive(false);
        // 👉 at this point screen is fully black
        // you could load the next scene here if needed
        // e.g.: SceneManager.LoadScene("NextScene");
    }
}
