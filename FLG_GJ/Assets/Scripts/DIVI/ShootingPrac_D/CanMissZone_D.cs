using UnityEngine;

public class CanMissZone_D : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        var can = other.GetComponent<Can_D>();
        if (!can) return;

        Destroy(can.gameObject);
        GameManager_SP.Instance.OnCanMissed();
    }
}
