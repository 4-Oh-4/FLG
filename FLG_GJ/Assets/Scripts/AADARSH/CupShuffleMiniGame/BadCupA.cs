using UnityEngine;

public class BadCupA : MonoBehaviour
{
    //[SerializeField] ShuffleManager_A shuffleManager;
    [SerializeField] GameObject canvas;

    private void OnMouseDown() {
        if (ShuffleManager_A.shufflefinished && canvas.active == false) {
            Debug.Log("wrong Choice");
            if (canvas != null) {
                canvas.SetActive(true);
            }
        }
    }
}
