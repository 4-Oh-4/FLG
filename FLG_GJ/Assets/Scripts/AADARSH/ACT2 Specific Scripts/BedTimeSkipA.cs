using UnityEngine;

public class BedTimeSkipA : MonoBehaviour
{
    [SerializeField] ShowingStoryUpdates1 blackscreen;
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            blackscreen.ShowUpdate("as");
        }
    }
}
