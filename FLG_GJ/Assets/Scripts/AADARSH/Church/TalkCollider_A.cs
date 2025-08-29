using UnityEngine;

public class TalkCollider_A : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] NPC_A father;
    [SerializeField] GameObject triggerGate;
    private bool once = true;
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")&&once) {
            Debug.Log("collider");
            once = false;
            father.TriggerDialouge();
            triggerGate.SetActive(true);
        }
    }
}
