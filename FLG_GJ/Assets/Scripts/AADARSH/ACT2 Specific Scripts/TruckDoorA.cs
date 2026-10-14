using UnityEngine;

public class TruckDoorA : MonoBehaviour
{
    [SerializeField]private int once=1;
    private void OnTriggerEnter2D(Collider2D collision) {
        if (!PressToEnterA.Allows(this, collision)) return; // building entrance: needs E (see PressToEnterA)
        if (collision.CompareTag("Player")) {
            if (once == 0) {
                FindAnyObjectByType<LoadUnloadMiniGamesPlayerA>().LoadMiniGame("TruckChase_ACT2");
                once++;
            }
        }      
    }
    public void onceSet() {
        once = 0;
    }
}
