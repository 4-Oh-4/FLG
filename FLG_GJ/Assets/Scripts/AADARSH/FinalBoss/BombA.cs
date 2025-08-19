using UnityEngine;

public class BombA : MonoBehaviour {
    [SerializeField] private float delay = 2f;
    [SerializeField] private float radius = 2f;
    [SerializeField] private GameObject explosionEffect;
    private SpriteRenderer sr;

    void Start() {
        sr = GetComponent<SpriteRenderer>();
        sr.color = Color.black; // start color

        Invoke(nameof(Explode), delay);

        // flash to red just before explosion
        //Invoke(nameof(FlashRed), delay - 0.3f); // flash 0.3s before
    }
    void Update() {
        if (sr != null) {
            float t = Mathf.PingPong(Time.time * 5f, 1f); // oscillates
            sr.color = Color.Lerp(Color.black, Color.red, t);
        }
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
