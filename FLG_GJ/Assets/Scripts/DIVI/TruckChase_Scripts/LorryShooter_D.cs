using UnityEngine;

namespace TruckChase
{
    public class LorryShooter : MonoBehaviour
    {
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private float fireRate = 0.3f;

        private Camera mainCam;
        private float nextFireTime = 0f;

        void Start()
        {
            mainCam = Camera.main;
        }

        void Update()
        {
            // --- Aiming Logic ---
            // Get the world position of the mouse.
            Vector2 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            // Calculate the direction from the shooter to the mouse.
            Vector2 lookDir = mousePos - (Vector2)transform.position;
            // Calculate the angle and apply the rotation to the shooter sprite.
            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            // --- Shooting Logic ---
            if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
            {
                // Shoot from this object's current position and in its current direction.
                Instantiate(bulletPrefab, transform.position, transform.rotation);
                nextFireTime = Time.time + fireRate;
            }
        }
    }
}