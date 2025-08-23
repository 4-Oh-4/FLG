using UnityEngine;

public class InteractWithDialougeA : MonoBehaviour
{
    private NPC_A npc;
    private bool canTalk;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter(Collider other) {
        npc=other.GetComponent<NPC_A>();
        if (npc) {
            canTalk = true;
        }
    }
    private void OnTriggerExit(Collider other) {
        canTalk = false;
    }
    private void Update() {
        if (canTalk && Input.GetKeyDown(KeyCode.E)) {
            npc.TriggerDialouge();
        }
    }
}
