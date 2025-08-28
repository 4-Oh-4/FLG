using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PackagePickup_Drug : MonoBehaviour
{
    void Reset() { GetComponent<Collider2D>().isTrigger = true; }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager_Drug.Instance?.OnPackageCollected();
            Destroy(gameObject);
        }
    }
}
