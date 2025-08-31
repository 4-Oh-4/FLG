using UnityEngine;

public class ShackMiddelNPC : MonoBehaviour
{
    
    [SerializeField] private int b = 1;
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player") && b == 0) {
            StoryManagertAct1A.Instance.SetFlag("TalkWithMiddleMan", true);
            b++;
            gameObject.GetComponent<NPC_A>().TriggerDialouge();
        }
    }
    public void setbb() {
        b = 0;
    }
}
