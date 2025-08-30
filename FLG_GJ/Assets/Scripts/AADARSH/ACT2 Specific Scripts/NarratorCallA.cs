using UnityEngine;

public class NarratorCallA : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private int once = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player") && once == 0) {
            once++;
            gameObject.GetComponent<NPC_A>().TriggerDialouge();
        }
    }
    public void ResetOncE() {
        once = 0;
    }
}
