using UnityEngine;

public class BigShackCollider : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private int b = 1;
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player") && b == 0) {
            b++;
            StoryManagertAct1A.Instance.SetFlag("TpToBigShack",true);
        }
    }
    public void setb() {
        b = 0;
    }
}
