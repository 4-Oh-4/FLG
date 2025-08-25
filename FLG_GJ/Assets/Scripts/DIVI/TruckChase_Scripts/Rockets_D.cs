using UnityEngine;
using TruckChase;

public class Rocket_D : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSpeed = 200f;
    [SerializeField] private float damage = 15f;
    [SerializeField] private float lifeTime = 5f;

    [Header("Explosion")]
    [SerializeField] private float explosionRadius = 1.5f;
    [SerializeField] private float explosionDamage = 10f;

    private Transform lorryTarget;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        lorryTarget = GameObject.Find("Lorry")?.transform;
        Invoke(nameof(Deactivate), lifeTime);
    }

    void FixedUpdate()
    {
        if (lorryTarget == null) return;

        Vector2 direction = ((Vector2)lorryTarget.position - rb.position).normalized;
        float rotateAmount = Vector3.Cross(direction, transform.up).z;
        rb.angularVelocity = -rotateAmount * rotationSpeed;
        rb.linearVelocity = transform.up * speed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Explode();
        Deactivate();
    }

    void Deactivate()
    {
        CancelInvoke();
        gameObject.SetActive(false);
    }

    void Explode()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D hit in colliders)
        {
            if (hit.TryGetComponent<LorryHealth_D>(out LorryHealth_D lorry)) lorry.TakeDamage(damage);
            else if (hit.TryGetComponent<BikeAI_D>(out var bike)) bike.TakeDamage(explosionDamage);
            else if (hit.TryGetComponent<SedanAI_D>(out var sedan)) sedan.TakeDamage(explosionDamage);
            else if (hit.TryGetComponent<JeepAI_D>(out var jeep)) jeep.TakeDamage(explosionDamage);
            else if (hit.TryGetComponent<ArmoredVanAI_D>(out var van)) van.TakeDamage(explosionDamage);
        }
    }
}