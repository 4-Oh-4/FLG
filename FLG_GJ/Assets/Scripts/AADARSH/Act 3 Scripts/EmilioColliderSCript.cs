using UnityEngine;

public class EmilioColliderSCript : MonoBehaviour
{
    int i = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")&&i==0) {
            gameObject.GetComponent<NPC_A>().TriggerDialouge();
            i++;
        }
    }
}
