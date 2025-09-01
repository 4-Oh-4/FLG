using UnityEngine;

public class PlayerAttackA : MonoBehaviour {
    [SerializeField] private int damage = 1;

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) // attack key
        {
            // check if boss in range
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 2f);
            foreach (Collider2D hit in hits) {
                if (hit.CompareTag("Boss")) {
                    hit.GetComponent<StationaryBossA>().TakeDamage(damage);
                }
            }
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 2f);
    }
}
