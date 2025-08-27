using UnityEngine;

namespace TopDownShooter
{
    public class AmmoPickup_D : MonoBehaviour
    {
        [SerializeField] private float lifeTime = 5f;

        private void OnEnable()
        {
            Invoke(nameof(Deactivate), lifeTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            PlayerController_D player = other.GetComponent<PlayerController_D>();
            if (player != null)
            {
                player.ReplenishAmmo();
                Deactivate();
            }
        }

        private void Deactivate()
        {
            // Cancel the scheduled deactivation to prevent errors if the player picks it up first.
            CancelInvoke();
            // AMENDED: This now deactivates the object instead of destroying it.
            gameObject.SetActive(false);
        }
    }
}