using UnityEngine;

public class BombA : MonoBehaviour {
    [SerializeField] private float delay = 2f;
    [SerializeField] private float radius = 2f;
    [SerializeField] private GameObject explosionEffect;

    private SpriteRenderer sr;
    private float timer;

    void Start() {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = Color.black;

        timer = 0f;
    }

    void Update() {
        timer += Time.deltaTime;

        if (sr != null) {
            // Flash faster as it gets closer to explosion
            float progress = timer / delay; // 0 to 1
            float flashSpeed = Mathf.Lerp(2f, 20f, progress);
            float t = Mathf.PingPong(Time.time * flashSpeed, 1f);

            sr.color = Color.Lerp(Color.black, Color.red, t);
        }

        if (timer >= delay) {
            Explode();
        }
    }

    void Explode() {
        // Show explosion effect
        if (explosionEffect != null) {
            GameObject fx = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(fx, 1f); // cleanup after 1 sec
        }

        // Damage player if inside radius
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (Collider2D hit in hits) {
            if (hit.CompareTag("Player")) {
                // Example damage call
                //hit.GetComponent<PlayerHealth>()?.TakeDamage(1);
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
