using UnityEngine;

public class ChurchTimeSkip : MonoBehaviour
{
    private int once = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")&&once==0) {
            once++;
            if(!StoryManagertAct1A.Instance.GetFlag("TimeSkipToEvening"))
            StoryManagertAct1A.Instance.SetFlag("TimeSkipToEvening",true);
            else
                StoryManagertAct1A.Instance.SetFlag("TimeSkipToEveningBB", true);

        }
    }
    public void ResetOncE() {
        once = 0;
    }
}
