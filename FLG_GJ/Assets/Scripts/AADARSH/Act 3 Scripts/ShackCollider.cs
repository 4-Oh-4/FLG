using UnityEngine;

public class ShackCollider : MonoBehaviour
{
    [SerializeField] private int a = 1;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player") && a == 1) {
            a++;
            StoryManagertAct1A.Instance.SetFlag("TpToShackArea", true);
        }
    }
    public void setA() {
        a = 1;
    }
}
