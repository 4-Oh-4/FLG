using UnityEngine;

public class FinalShackConvo : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            StoryManagertAct1A.Instance.SetFlag("FinalConvo", true);
            gameObject.GetComponent<NPC_A>().TriggerDialouge();
        }
    }
}
