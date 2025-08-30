using UnityEngine;

public class DariusIntroMarketAct2 : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private bool once = false;
    [SerializeField] Collider2D colliderl;
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            if (once) {
                gameObject.GetComponent<NPC_A>().TriggerDialouge();
                StoryManagertAct1A.Instance.SetFlag("TalkWithDarius", true);
                colliderl.enabled = false;
            }
        }
    }
    public void setONce() {
        once = true;
    }
}
