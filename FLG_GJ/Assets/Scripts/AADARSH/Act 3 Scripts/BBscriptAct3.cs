using UnityEngine;

public class BBscriptAct3 : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private int i = 1;
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            if (i == 0) {
                StoryManagertAct1A.Instance.SetFlag("BossFight", true) ;
            }
        }
    }
    public void setii() {
        i = 0;
    }
}
