using UnityEngine;

public class InteractWithDialogueA : MonoBehaviour {
    public float radius = 1.5f;         // size of the circle
    public float distance = 0f;         // how far to cast (0 = just a circle at position)
    public LayerMask npcLayer;          // filter to only hit NPCs

    void Update() {
        if (Input.GetKeyDown(KeyCode.E)) {
            RaycastHit2D hit = Physics2D.CircleCast(
                transform.position,      // start at player position
                radius,                  // circle radius
                Vector2.zero,            // direction (zero = just overlap at position)
                distance,                // distance to cast (0 = overlap circle)
                npcLayer                 // only check NPC layer
            );

            if (hit.collider != null) {
                NPC_A npc = hit.collider.GetComponent<NPC_A>();
                if (npc != null) {
                    npc.TriggerDialouge();
                }
            }
        }
    }

    // Just so you can see it in editor
    void OnDrawGizmosSelected() {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
