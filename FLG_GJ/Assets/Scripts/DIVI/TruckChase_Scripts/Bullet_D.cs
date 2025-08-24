using UnityEngine;

// Ensure this is in the same namespace as your other scripts.
namespace TruckChase
{
    public class Bullet_D : MonoBehaviour
    {
        [SerializeField] private float speed = 15f;
        [SerializeField] private float damage = 10f;

        void Update()
        {
            // Move the bullet "forward" (in the direction it was fired).
            transform.position += transform.up * speed * Time.deltaTime;

            // Destroy the bullet if it flies too far off-screen.
            if (transform.position.y < -12f)
            {
                Destroy(gameObject);
            }
        }

        // This function is called when the bullet's trigger collides with another collider.
        void OnTriggerEnter2D(Collider2D other)
        {
            // Check for all possible enemy types and deal damage.
            if (other.TryGetComponent<BikeAI_D>(out var bike)) bike.TakeDamage(damage);
            else if (other.TryGetComponent<SedanAI_D>(out var sedan)) sedan.TakeDamage(damage);
            else if (other.TryGetComponent<JeepAI_D>(out var jeep)) jeep.TakeDamage(damage);
            else if (other.TryGetComponent<ArmoredVanAI_D>(out var van)) van.TakeDamage(damage);

            // Destroy the bullet after it hits something.
            Destroy(gameObject);
        }
    }
}