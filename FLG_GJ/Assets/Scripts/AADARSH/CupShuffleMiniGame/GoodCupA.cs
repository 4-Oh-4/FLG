using UnityEngine;

public class GoodCupA : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] GameObject canvas;
    [SerializeField] CupShuffleA shuffler;
    public int score;
    [SerializeField] private Transform cup;
    [SerializeField] Vector3 initial;
    [SerializeField] Vector3 offset;
    [SerializeField] float movespeed = 0.01f;
    private bool moveUP = false;
    private bool moveDown = false;
    private void OnMouseDown() {
        
        if (ShuffleManager_A.shufflefinished&& canvas.active==false) {
            score++;
            initial = transform.position;
            cup.SetParent(null);
            moveUP = true;
            Debug.Log("RightChoice " + score);
            if (score >= 3) {
                Debug.Log("GameFinished");
                return;
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
                if (canvas != null) {
                    canvas.SetActive(true);
                    shuffler.IncreaseDifficulty();
                }
            }
        }
    }
}
