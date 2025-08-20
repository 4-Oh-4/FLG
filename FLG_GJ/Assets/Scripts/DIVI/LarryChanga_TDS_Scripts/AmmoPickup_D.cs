using UnityEngine;

namespace TopDownShooter
{
    public class AmmoPickup_D : MonoBehaviour
    {
        [Tooltip("How many seconds the pickup will stay on the ground before disappearing.")]
        [SerializeField] private float lifeTime = 5f;

        private void Start()
        {
            // Schedule this GameObject to be destroyed after 'lifeTime' seconds.
            Destroy(gameObject, lifeTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            PlayerController_D player = other.GetComponent<PlayerController_D>();
            if (player != null)
            {
                player.ReplenishAmmo();
                // If the player picks it up, destroy it immediately.
                Destroy(gameObject);
            }
        }
    }
}