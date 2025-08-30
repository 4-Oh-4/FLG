using UnityEngine;

public class Can_D : MonoBehaviour
{
    [HideInInspector] public Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Launch(float gravityScale, float upForce, float lateralForce)
    {
        // Reset physics state
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.gravityScale = gravityScale;

        // Random lateral left/right
        float side = Random.value < 0.5f ? -1f : 1f;
        Vector2 impulse = Vector2.up * upForce + Vector2.right * side * lateralForce;

        rb.AddForce(impulse, ForceMode2D.Impulse);
        rb.AddTorque(Random.Range(-50f, 50f), ForceMode2D.Impulse);
    }

    public void Hit()
    {
        // Pop effect hook here if you want
        Destroy(gameObject);
    }
}
