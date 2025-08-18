using UnityEngine;

public class ShootingTarget : MonoBehaviour
{
    private void OnMouseDown() {
        ShootingManager.score++;
        Debug.Log(ShootingManager.score);
        Destroy(gameObject);
    }
    private void Update() {
        if (transform.position.y < -8) Destroy(gameObject);
    }
}
