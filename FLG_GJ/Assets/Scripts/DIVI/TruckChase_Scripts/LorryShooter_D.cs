using UnityEngine;

namespace TruckChase
{
    public class LorryShooter_D : MonoBehaviour
    {
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private float fireRate = 0.3f;

        private Camera mainCam;
        private float nextFireTime = 0f;

        void Start()
        {
            mainCam = Camera.main;

            // --- Setup Checks ---
            if (mainCam == null)
            {
                Debug.LogError("LorryShooter Error: Main Camera not found! Make sure your camera is tagged 'MainCamera'.");
            }
            if (bulletPrefab == null)
            {
                Debug.LogError("LorryShooter Error: Bullet Prefab is not assigned in the Inspector!");
            }
        }

        void Update()
        {
            // Stop if setup is incorrect.
            if (mainCam == null || bulletPrefab == null) return;

            // --- Aiming Logic ---
            Vector2 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            Vector2 lookDir = mousePos - (Vector2)transform.position;
            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            // --- Shooting Logic ---
            if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
            {
                Instantiate(bulletPrefab, transform.position, transform.rotation);
                nextFireTime = Time.time + 1f / fireRate;
            }
        }
    }
}