using UnityEngine;

public class SmallShack : MonoBehaviour
{
    [SerializeField] private int p = 1;
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player") && p == 0) {
            p++;
            StoryManagertAct1A.Instance.SetFlag("FinalShack",true);
        }
    }
}
