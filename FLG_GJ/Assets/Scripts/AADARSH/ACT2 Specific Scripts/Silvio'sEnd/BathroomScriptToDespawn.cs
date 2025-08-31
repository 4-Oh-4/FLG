using UnityEngine;

public class BathroomScriptToDespawn : MonoBehaviour
{
    int i = 0;
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("ToBeDestroyed")) {
            Destroy(collision.gameObject);
            i++;
            if (i == 2) {
                StoryManagertAct1A.Instance.SetFlag("SilvioInBathroom", true);
            }
        }
    }
}
