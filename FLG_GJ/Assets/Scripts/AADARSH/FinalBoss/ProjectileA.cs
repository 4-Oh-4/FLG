using UnityEngine;

public class ProjectileA : MonoBehaviour {
    [SerializeField] private float lifetime = 5f;   // how long before auto-destroy
    [SerializeField] private int damage = 1;        // how much damage to deal

    void Start() {
        Destroy(gameObject, lifetime); // auto destroy after X seconds
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            // Example damage function (you must add this to your player script)
            Debug.Log("Player hit by projectile!");
            // collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);
        }

        // Destroy projectile on any collision
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            Debug.Log("Player hit by projectile!");
            // other.GetComponent<PlayerHealth>().TakeDamage(damage);
            Destroy(gameObject);
        } else if (other.CompareTag("Wall")) {
            Destroy(gameObject);
        }
    }
}
