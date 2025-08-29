using UnityEngine;
namespace TruckChase
{

    public class EnemyAvoidance : MonoBehaviour
    {
        public float avoidanceRadius = 2f;   // how close before avoiding
        public float avoidanceStrength = 5f; // how strongly to steer away

        void Update()
        {
            Collider2D[] neighbors = Physics2D.OverlapCircleAll(transform.position, avoidanceRadius);
            Vector2 separationForce = Vector2.zero;

            foreach (Collider2D neighbor in neighbors)
            {
                if (neighbor.gameObject != gameObject && neighbor.CompareTag("Enemy"))
                {
                    Vector2 away = (Vector2)(transform.position - neighbor.transform.position);
                    float distance = away.magnitude;

                    if (distance > 0)
                        separationForce += away.normalized / distance; // stronger when closer
                }
            }

            if (separationForce != Vector2.zero)
            {
                transform.position += (Vector3)(separationForce * avoidanceStrength * Time.deltaTime);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, avoidanceRadius);
        }
    }
}
