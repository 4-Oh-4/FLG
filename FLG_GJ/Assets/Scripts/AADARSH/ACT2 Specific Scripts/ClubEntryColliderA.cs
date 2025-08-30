using UnityEngine;

public class ClubEntryColliderA : MonoBehaviour
{
    [SerializeField] private int i = 3;
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            if (i == 0) StoryManagertAct1A.Instance.SetFlag("ClubEntry", true);
            i++;
        }
    }
    public void setI() {
        i = 0;
    }
}
