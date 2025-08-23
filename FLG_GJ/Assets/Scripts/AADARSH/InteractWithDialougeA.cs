using UnityEngine;

public class InteractWithDialougeA : MonoBehaviour {
    private NPC_A npc;
    private bool canTalk = false;

    private void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("got him");
        npc = other.GetComponent<NPC_A>();
        if (npc != null) {
            canTalk = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.GetComponent<NPC_A>() == npc) {
            canTalk = false;
            npc = null;
        }
    }

    private void Update() {
        if (canTalk && Input.GetKeyDown(KeyCode.E)) {
            npc.TriggerDialouge();
        }
    }
}
