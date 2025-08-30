using UnityEngine;

public class GateColliderInWareHouse : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]private int once = 1;
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            if (once == 0) {
                once++;
                StoryManagertAct1A.Instance.SetFlag("TDSStart",true);
            }
        }
    }
    public void setONCE() {
        once = 0;
    }
}
