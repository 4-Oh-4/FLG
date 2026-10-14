using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Makes the quest arrow (and minimap marker) lead the player to the right exit first when the
/// objective is on another level they cannot walk to.
///
/// Only Act 1 needs this: its rooftop and the ground below are separate walkable areas, joined
/// only by the stairs (GoingDownACT1Trigger, roof -> ground) and the gate (GateToGoUpAct1_A,
/// ground -> roof). In Acts 2 and 3 objectives already point at entrances until the story
/// teleports the player inside, so targets there are returned unchanged.
///
/// The rooftop's exact shape is found once per load by flood-filling walkable space from the
/// roof spawn point inside a small bounding box (a few thousand physics checks, a few ms).
/// </summary>
public static class WaypointRouterA {
    private const string Act1Scene = "Act_1_Map_K";
    private static readonly Rect roofBounds = Rect.MinMaxRect(-45f, -12.5f, -22.5f, 8.5f); // measured roof extent + margin
    private static readonly Vector2 roofSeed = new Vector2(-33f, -4.6f);                    // GameManagerAct1A.posRoof
    private const float Cell = 0.5f;

    private static int builtForScene = -1;
    private static HashSet<Vector2Int> roofCells;
    private static Vector2 probeSize;
    private static Vector2 probeOffset;
    private static Transform player;
    private static GoingDownACT1Trigger stairsDown;
    private static GateToGoUpAct1_A gateUp;

    /// <summary>The transform the quest arrow should point at for this target right now.</summary>
    public static Transform Resolve(Transform target) {
        if (target == null) return null;
        Scene active = SceneManager.GetActiveScene();
        if (active.name != Act1Scene) return target;
        if (builtForScene != active.handle && !Build(active)) return target;
        if (player == null || !player.gameObject.activeInHierarchy) return target;

        bool playerOnRoof = OnRoof(player.position);
        bool targetOnRoof = OnRoof(target.position);
        if (playerOnRoof == targetOnRoof) return target;

        if (playerOnRoof) {
            // The stairs only work once the story opens them (their collider becomes a trigger).
            if (stairsDown == null || !stairsDown.gameObject.activeInHierarchy) return target;
            Collider2D c = stairsDown.GetComponent<Collider2D>();
            return c != null && c.enabled && c.isTrigger ? stairsDown.transform : target;
        }
        return gateUp != null && gateUp.gameObject.activeInHierarchy ? gateUp.transform : target;
    }

    private static bool Build(Scene scene) {
        player = null;
        foreach (PlayerMovement_A pm in Object.FindObjectsByType<PlayerMovement_A>(FindObjectsInactive.Include, FindObjectsSortMode.None)) {
            if (pm.gameObject.scene == scene) { player = pm.transform; break; }
        }
        stairsDown = Object.FindAnyObjectByType<GoingDownACT1Trigger>(FindObjectsInactive.Include);
        gateUp = Object.FindAnyObjectByType<GateToGoUpAct1_A>(FindObjectsInactive.Include);
        if (player == null) return false;

        BoxCollider2D box = player.GetComponent<BoxCollider2D>();
        probeSize = box != null ? box.size * 0.9f : new Vector2(0.4f, 0.75f);
        probeOffset = box != null ? box.offset : Vector2.zero;

        Physics2D.SyncTransforms();
        roofCells = new HashSet<Vector2Int>();
        var queue = new Queue<Vector2Int>();
        Vector2Int seed = ToCell(roofSeed);
        roofCells.Add(seed);
        queue.Enqueue(seed);
        Vector2Int[] steps = { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };
        while (queue.Count > 0) {
            Vector2Int c = queue.Dequeue();
            foreach (Vector2Int s in steps) {
                Vector2Int n = c + s;
                if (roofCells.Contains(n) || !roofBounds.Contains(ToWorld(n)) || !IsWalkable(n)) continue;
                roofCells.Add(n);
                queue.Enqueue(n);
            }
        }
        builtForScene = scene.handle;
        return true;
    }

    // A point counts as roof if its cell (or, for targets standing on something solid, the nearest
    // walkable cell around it) belongs to the rooftop area.
    private static bool OnRoof(Vector2 position) {
        if (!roofBounds.Contains(position)) return false;
        Vector2Int c = ToCell(position);
        if (roofCells.Contains(c)) return true;
        if (IsWalkable(c)) return false;
        for (int r = 1; r <= 4; r++) {
            for (int dx = -r; dx <= r; dx++) {
                for (int dy = -r; dy <= r; dy++) {
                    if (Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy)) != r) continue;
                    Vector2Int n = new Vector2Int(c.x + dx, c.y + dy);
                    if (roofCells.Contains(n)) return true;
                    if (IsWalkable(n)) return false;
                }
            }
        }
        return false;
    }

    private static bool IsWalkable(Vector2Int cell) {
        foreach (Collider2D hit in Physics2D.OverlapBoxAll(ToWorld(cell) + probeOffset, probeSize, 0f)) {
            if (hit.isTrigger || hit.attachedRigidbody != null) continue; // triggers, NPCs and the player don't block
            return false;
        }
        return true;
    }

    private static Vector2Int ToCell(Vector2 p) => new Vector2Int(Mathf.RoundToInt(p.x / Cell), Mathf.RoundToInt(p.y / Cell));
    private static Vector2 ToWorld(Vector2Int c) => new Vector2(c.x * Cell, c.y * Cell);
}
