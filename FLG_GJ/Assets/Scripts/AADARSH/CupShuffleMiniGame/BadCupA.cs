using UnityEngine;

public class BadCupA : MonoBehaviour
{
    //[SerializeField] ShuffleManager_A shuffleManager;
    [SerializeField] GameObject canvas;
    [SerializeField] Vector3 initial;
    [SerializeField] Vector3 offset;
    [SerializeField] float movespeed = 0.01f;
    private bool moveUP=false;
    private bool moveDown=false;

    private void OnMouseDown() {
        if (ShuffleManager_A.shufflefinished && canvas.active == false) {
            Debug.Log("wrong Choice");
            initial = transform.position;
            moveUP = true;
            if (canvas != null) {
                canvas.SetActive(true);
            }
        }
    }
    private void Update() {
        if (moveUP) {
            transform.position = Vector3.Lerp(transform.position, initial + offset, movespeed);
            if (Mathf.Abs(transform.position.y - initial.y - offset.y) < 0.1) {
                transform.position = initial + offset;
                moveUP = false;
                moveDown = true;
            }
        }
        if (moveDown) {
            transform.position = Vector3.Lerp(transform.position, initial, movespeed);
            if (Mathf.Abs(transform.position.y - initial.y) < 0.1) {
                transform.position = initial;
                moveDown = false;
            }
        }
    }
}
