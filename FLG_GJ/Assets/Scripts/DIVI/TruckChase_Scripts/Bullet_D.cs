using UnityEngine;
using TruckChase;

public class Bullet_D : MonoBehaviour
{
    public float speed = 15f;
    public float damage = 10f;
    public float lifeTime = 3f;

    private void OnEnable()
    {
        Invoke(nameof(Deactivate), lifeTime);
    }

    void Update()
    {
        transform.position += transform.up * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<BikeAI_D>(out var bike)) bike.TakeDamage(damage);
        else if (other.TryGetComponent<SedanAI_D>(out var sedan)) sedan.TakeDamage(damage);
        else if (other.TryGetComponent<JeepAI_D>(out var jeep)) jeep.TakeDamage(damage);
        else if (other.TryGetComponent<ArmoredVanAI_D>(out var van)) van.TakeDamage(damage);

        Deactivate();
    }

    void Deactivate()
    {
        CancelInvoke();
        gameObject.SetActive(false);
    }
}