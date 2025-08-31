using UnityEngine;

public class BedTimeSkipA : MonoBehaviour
{
    [SerializeField] BlackScreenHouse blackscreen;
    [SerializeField] SimpleFadeIn_A fadeIn_A;
    [SerializeField] Collider2D collidera;
    [SerializeField] string message;
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            if(collidera!=null)collidera.enabled = true;
            if (fadeIn_A == null) blackscreen.ShowUpdate(message);
            else fadeIn_A.StartJournals();
        }
    }
}
