using UnityEngine;

public class BedTimeSkipA : MonoBehaviour
{
    [SerializeField] BlackScreenHouse blackscreen;
    [SerializeField] string message;
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            blackscreen.ShowUpdate(message);
        }
    }
}
