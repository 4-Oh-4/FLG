using UnityEngine;

namespace TruckChase
{
    public class Rocket_D : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] private float speed = 5f;
        [SerializeField] private float rotationSpeed = 200f;
        [SerializeField] private float damage = 15f;

        [Header("Explosion")]
        [SerializeField] private float explosionRadius = 1.5f;
        [SerializeField] private float explosionDamage = 10f;

        private Transform lorryTarget;
        private Rigidbody2D rb;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            GameObject lorryObject = GameObject.Find("Lorry");
            if (lorryObject != null)
            {
                lorryTarget = lorryObject.transform;
            }
        }

        void FixedUpdate()
        {
            if (lorryTarget == null) return;

            Vector2 direction = (Vector2)lorryTarget.position - rb.position;
            direction.Normalize();
            float rotateAmount = Vector3.Cross(direction, transform.up).z;
            rb.angularVelocity = -rotateAmount * rotationSpeed;
            rb.linearVelocity = transform.up * speed;

            if (transform.position.y > 12f) Destroy(gameObject);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            Explode();
            Destroy(gameObject);
        }

        void Explode()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

            foreach (Collider2D hit in colliders)
            {
                // If we hit the lorry, deal direct damage.
                if (hit.TryGetComponent<LorryHealth_D>(out LorryHealth_D lorry))
                {
                    lorry.TakeDamage(damage);
                }

                // AMENDED: Check for each enemy AI script instead of EnemyHealth_D
                if (hit.TryGetComponent<BikeAI_D>(out var bike)) bike.TakeDamage(explosionDamage);
                else if (hit.TryGetComponent<SedanAI_D>(out var sedan)) sedan.TakeDamage(explosionDamage);
                else if (hit.TryGetComponent<JeepAI_D>(out var jeep)) jeep.TakeDamage(explosionDamage);
                else if (hit.TryGetComponent<ArmoredVanAI_D>(out var van)) van.TakeDamage(explosionDamage);
            }
        }
    }
}