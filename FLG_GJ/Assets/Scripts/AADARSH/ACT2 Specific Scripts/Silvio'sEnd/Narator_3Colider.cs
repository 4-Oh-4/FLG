using UnityEngine;

public class Narator_3Colider : MonoBehaviour
{
    [SerializeField] private int i = 1;
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            if (i == 0) {
                i++;
                gameObject.GetComponent<NPC_A>().TriggerDialouge();
                StoryManagertAct1A.Instance.SetFlag("WhoIsThere", true);
            }
        }
    }
    public void setti() {
        i = 0;
    }
}
