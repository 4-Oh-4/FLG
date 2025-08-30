using UnityEngine;

public class Can_D : MonoBehaviour
{
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Launch(float gravityScale, float upForce, float lateralForce)
    {
        rb.gravityScale = gravityScale;
        rb.AddForce(new Vector2(lateralForce, upForce), ForceMode2D.Impulse);
    }

    public void Hit()
    {
        // TODO: Add effects (particles, sound, animation)
        Destroy(gameObject); // destroy on click
    }
}
