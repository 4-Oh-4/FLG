using UnityEngine;

public class RoxanaTalkCollider : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private int i = 1;
    [SerializeField] Collider2D collider2;
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player") && i == 0) {
            i++;
            gameObject.GetComponent<NPC_A>().TriggerDialouge();
            collider2.enabled = false;
        }
    }
    public void Iset() {
        i = 0;
    }
}
