using UnityEngine;

public class BombA : MonoBehaviour {
    [SerializeField] private float delay = 2f;
    [SerializeField] private float radius = 2f;
    [SerializeField] private GameObject explosionEffect;

    void Start() {
        Invoke(nameof(Explode), delay);
    }

    void Explode() {
        // Show explosion effect
        if (explosionEffect != null)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        // Damage player if inside radius
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (Collider2D hit in hits) {
            if (hit.CompareTag("Player")) {
                // Call player damage function
                Debug.Log("Player hit by bomb!");
            }
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
