using UnityEngine;
using TruckChase;

public class LorryShooter_D : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float fireRate = 10f;

    private Camera mainCam;
    private float nextFireTime = 0f;

    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        Vector2 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 lookDir = mousePos - (Vector2)transform.position;

        // Rotate so that local X axis (red arrow) points at mouse
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + 1f / fireRate;
            ObjectPooler_D.Instance.SpawnFromPool("Bullet_D", transform.position, transform.rotation);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        // Draws a red line along local X axis
        Gizmos.DrawLine(transform.position, transform.position + transform.right * 5f);
    }
}
