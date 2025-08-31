using UnityEngine;

public class FinalShackColldier : MonoBehaviour
{
    [SerializeField] int h = 1;
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player") && h == 0) {
            StoryManagertAct1A.Instance.SetFlag("TpToFinalShacks", true);
            h++;
        }
    }
    public void seth() {
        h = 0;
    }
}
