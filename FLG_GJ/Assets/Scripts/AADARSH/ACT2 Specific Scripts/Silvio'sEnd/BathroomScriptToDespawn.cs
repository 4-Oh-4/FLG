using UnityEngine;

public class BathroomScriptToDespawn : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("ToBeDestroyed")) {
            Destroy(collision.gameObject);
        }
    }
}
