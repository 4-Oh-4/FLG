using UnityEngine;

public class ClubEntryColliderA : MonoBehaviour
{
    [SerializeField] private int i = 20;
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            if (i == 1) {
                StoryManagertAct1A.Instance.SetFlag("ClubEntryAgain", true);
                i=20;
            }
            if (i == 0){ StoryManagertAct1A.Instance.SetFlag("ClubEntry", true);
            i=20;
            }

        }
    }
    public void setI() {
        if (!StoryManagertAct1A.Instance.GetFlag("ClubEntry"))
            i = 0;
        else i = 1;
    }
}
