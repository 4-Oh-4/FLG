using UnityEngine;
using TruckChase;

public class LorryShooter_D : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float fireRate = 10f;
    [SerializeField] private Camera cam;

    // NEW: A public Transform for the gun's muzzle position
    [SerializeField] private Transform firePoint;

    private Camera mainCam;
    private float nextFireTime = 0f;

    void Start()
    {
        if (cam == null)
            mainCam = Camera.main;
        else mainCam = cam;

        // NEW: A safety check. If you forget to assign a fire point,
        // it will default to the object's main transform.
        if (firePoint == null)
        {
            firePoint = transform;
        }
    }

    void Update()
    {
        // This aiming logic is already perfect and needs no changes.
        Vector2 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 lookDir = mousePos - (Vector2)transform.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + 1f / fireRate;

            // CHANGED: The bullet now spawns from the firePoint's position and rotation.
            ObjectPooler_D.Instance.SpawnFromPool("Bullet_D", firePoint.position, firePoint.rotation);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.right * 5f);
    }
}