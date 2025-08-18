using UnityEngine;

public class ShuffleManager_A : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private CupShuffleA cupshuffle;
    [SerializeField] private Transform ball;
    [SerializeField] private Transform cup;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float movementSpeed = 3f;
    [SerializeField] GameObject canvas;
    private Vector3 initial;
    private bool moveUPwards = false;
    private bool moveDOWNwards = false;
    public static bool shufflefinished = false;
    void Start()
    {
        

    }
    public void StartAgain() {
        canvas.SetActive(false);
        initial = cup.position;
        ball.SetParent(null);
        moveUPwards = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) {
            StartAgain();
        }
        if (moveUPwards) {
            cup.position = Vector3.Lerp(cup.position, initial + offset, movementSpeed);
            if (Mathf.Abs(cup.position.y - initial.y - offset.y) < 0.001) {
                cup.position = initial + offset;
                moveUPwards = false;
                moveDOWNwards = true;
            }
        }
        if (moveDOWNwards) {
            cup.position = Vector3.Lerp(cup.position, initial, movementSpeed);
            if (Mathf.Abs(cup.position.y - initial.y) < 0.001) {
                cup.position = initial;
                moveDOWNwards = false;
                ball.SetParent(cup);
                cupshuffle.StartShuffle();
            }
        }
    }
    
}
