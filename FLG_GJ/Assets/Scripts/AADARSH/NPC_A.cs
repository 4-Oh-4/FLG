using UnityEngine;

public class NPC_A : MonoBehaviour
{
    public DialougeA dialouge;
    [SerializeField]private bool canTalk = false;

    private void Update() {
        if (canTalk&&Input.GetKeyDown(KeyCode.E)) TriggerDialouge();
    }
    public void TriggerDialouge() {
        FindAnyObjectByType<DialougeManagerA>().StartDialouge(dialouge);
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        canTalk = true;
    }
    private void OnTriggerExit2D(Collider2D collision) {
        canTalk = false;
    }
}
