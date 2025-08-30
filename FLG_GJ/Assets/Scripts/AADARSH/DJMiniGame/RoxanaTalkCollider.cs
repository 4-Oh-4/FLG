using UnityEngine;

public class RoxanaTalkCollider : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private int i = 1;
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player") && i == 0) {
            i++;
            gameObject.GetComponent<NPC_A>().TriggerDialouge();
            
        }
    }
    public void Iset() {
        i = 0;
    }
}
