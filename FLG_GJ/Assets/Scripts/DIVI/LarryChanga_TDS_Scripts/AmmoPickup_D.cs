using UnityEngine;

namespace TopDownShooter
{
    public class AmmoPickup_D : MonoBehaviour
    {
        // This function is called when another collider enters this object's trigger.
        private void OnTriggerEnter2D(Collider2D other)
        {
            // Check if the object that entered has a PlayerController_D script.
            PlayerController_D player = other.GetComponent<PlayerController_D>();
            if (player != null)
            {
                // If it is the player, call the replenish function and destroy this pickup.
                player.ReplenishAmmo();
                Destroy(gameObject);
            }
        }
    }
}