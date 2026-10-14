using UnityEngine;

public class InteractWithDialogueA : MonoBehaviour {
    public float radius = 1.5f;         // size of the circle
    public float distance = 0f;         // how far to cast (0 = just a circle at position)
    public LayerMask npcLayer;          // filter to only hit NPCs

    void Update() {
        if (PauseMenuManager.IsPaused) return; // ignore gameplay input while paused
        if (Input.GetKeyDown(KeyCode.E)) {
            TryInteract();
        }
    }

    // What pressing E does. Public so it can be triggered from tests/tools.
    public void TryInteract() {
        RaycastHit2D hit = Physics2D.CircleCast(
            transform.position,      // start at player position
            radius,                  // circle radius
            Vector2.zero,            // direction (zero = just overlap at position)
            distance,                // distance to cast (0 = overlap circle)
            npcLayer                 // only check NPC layer
        );

        if (hit.collider != null) {
            NPC_A npc = hit.collider.GetComponent<NPC_A>();
            // If the first hit is an NPC with nothing to say (e.g. Silvio2 standing next to Thug_2),
            // talk to the nearest NPC in range that does have something instead.
            if (npc != null && !npc.HasInteraction()) {
                npc = FindNearestInteractableNPC() ?? npc;
            }
            // Don't restart a conversation that is already on screen.
            if (npc != null && !npc.IsDialogueOpen()) {
                npc.TriggerDialouge();
            }
            TrashforHomeA trashforHome = hit.collider.GetComponent<TrashforHomeA>();
            if (trashforHome != null) {
                trashforHome.CollectTrash();
            }
        }
    }

    NPC_A FindNearestInteractableNPC() {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, radius, Vector2.zero, distance, npcLayer);
        NPC_A best = null;
        float bestDist = float.MaxValue;
        foreach (var h in hits) {
            NPC_A candidate = h.collider.GetComponent<NPC_A>();
            if (candidate == null || !candidate.HasInteraction()) continue;
            float d = ((Vector2)h.collider.transform.position - (Vector2)transform.position).sqrMagnitude;
            if (d < bestDist) {
                bestDist = d;
                best = candidate;
            }
        }
        return best;
    }

    // Just so you can see it in editor
    void OnDrawGizmosSelected() {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
