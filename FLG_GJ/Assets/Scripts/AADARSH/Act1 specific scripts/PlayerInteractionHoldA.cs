using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem; // optional, used for keyboard check

public class PlayerInteractionHoldA : MonoBehaviour {
    [Header("Controls")]
    public Key interactKey = Key.E; // uses InputSystem key enum

    [Header("Detection")]
    public float detectionRadius = 0.6f; // radius to search for interactable if not using trigger
    public LayerMask interactableLayer; // set to layer containing destroyable objects

    [Header("Hold")]
    public float holdProgress = 0f;
    private HoldInteractableA currentTarget;
    private float requiredHold = 1.0f;

    [Header("UI")]
    public Image progressFillImage; // UI Image (type = Filled) assigned in inspector
    public GameObject progressRoot;  // parent UI element to enable/disable

    void Start() {
        if (progressRoot != null) progressRoot.SetActive(false);
    }

    void Update() {
        // First, find the nearest interactable within detection radius if currentTarget is null
        if (currentTarget == null) {
            FindNearbyInteractable();
        }

        // If we have a target, track hold
        if (currentTarget != null) {
            // Use InputSystem (Keyboard.current) if available, fallback to Input.GetKey
            bool holding = false;
            if (Keyboard.current != null) {
                holding = Keyboard.current[interactKey].isPressed;
            } else {
                holding = Input.GetKey(KeyCode.E); // fallback
            }

            if (holding) {
                // begin progress UI
                if (progressRoot != null && !progressRoot.activeSelf) progressRoot.SetActive(true);
                if (progressFillImage != null) progressFillImage.fillAmount = holdProgress / currentTarget.destroyHoldTime;

                holdProgress += Time.deltaTime;
                requiredHold = currentTarget.destroyHoldTime;

                // optional callback on start of destroying
                if (holdProgress - Time.deltaTime <= 0.05f) currentTarget.OnDestroyingStarted();

                if (holdProgress >= requiredHold) {
                    // destroy
                    currentTarget.DestroyNow();
                    ResetProgress();
                }
            } else {
                // released early -> cancel
                if (holdProgress > 0f) {
                    CancelProgress();
                }
            }
        } else {
            // no target -> ensure UI hidden and reset
            if (progressRoot != null && progressRoot.activeSelf) progressRoot.SetActive(false);
            holdProgress = 0f;
            if (progressFillImage != null) progressFillImage.fillAmount = 0f;
        }
    }

    void FindNearbyInteractable() {
        // if you prefer trigger-based detection, you can comment this and use OnTriggerEnter2D instead
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius, interactableLayer);
        if (hits.Length > 0) {
            // pick nearest
            float bestDist = float.MaxValue;
            HoldInteractableA best = null;
            foreach (var c in hits) {
                var io = c.GetComponent<HoldInteractableA>();
                if (io == null) continue;
                float d = Vector2.SqrMagnitude((Vector2)c.transform.position - (Vector2)transform.position);
                if (d < bestDist) {
                    bestDist = d;
                    best = io;
                }
            }

            currentTarget = best;
        } else {
            currentTarget = null;
        }
    }

    void CancelProgress() {
        holdProgress = 0f;
        if (progressRoot != null) progressRoot.SetActive(false);
        if (progressFillImage != null) progressFillImage.fillAmount = 0f;
        // optionally tell target that progress cancelled
    }

    void ResetProgress() {
        holdProgress = 0f;
        if (progressRoot != null) progressRoot.SetActive(false);
        if (progressFillImage != null) progressFillImage.fillAmount = 0f;
        currentTarget = null;
    }

    // Optional: draw detection radius in editor
    void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    // Optionally, you can use trigger-based detection:
    // - Add a CircleCollider2D (IsTrigger) on the player sized to detectionRadius
    // - Remove FindNearbyInteractable and implement OnTriggerEnter2D/OnTriggerExit2D to set currentTarget
}
