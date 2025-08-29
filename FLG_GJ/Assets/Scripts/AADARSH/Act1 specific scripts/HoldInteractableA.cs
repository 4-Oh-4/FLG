using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class HoldInteractableA : MonoBehaviour {
    [Tooltip("Time (seconds) the player must hold the key to destroy this object.")]
    public float destroyHoldTime = 1.5f;

    // Optional: call before destroy (play VFX/sound)
    public void OnDestroyingStarted() {
        // play sound / VFX
    }

    // Optional: call when destroyed
    public void OnDestroyed() {
        // spawn debris, increment score, etc.
    }

    // Called by PlayerInteraction when hold completes
    public void DestroyNow() {
        OnDestroyed();
        Destroy(gameObject);
    }
}
