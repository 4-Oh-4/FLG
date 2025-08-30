using UnityEngine;

public class NinoTriggerOffice : MonoBehaviour
{
    [SerializeField] private int i = 0;
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")&&i==0) {
            i++;
            gameObject.GetComponent<NPC_A>().TriggerDialouge();
        }
    }
}
