using UnityEngine;

public class BedTimeSkipA : MonoBehaviour
{
    [SerializeField] BlackScreenHouse blackscreen;
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            blackscreen.ShowUpdate("Few Weeks Later");
        }
    }
}
