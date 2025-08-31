using UnityEngine;

public class PoliceFlagScript : MonoBehaviour
{
    [SerializeField] private int i = 1;
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("PoliceRaid")) {
            StoryManagertAct1A.Instance.SetFlag("RunToCentralBB", true);
        }
    }
}
