using UnityEngine;

public class BigShackMIddleMan : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            gameObject.GetComponent<NPC_A>().TriggerDialouge();
        }
    }
}
