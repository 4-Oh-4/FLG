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
    public static bool gamefinishedCUP = false;
    void Start()
    {
        // These are static, so they outlive the scene: a finished game (or one quit half-way) would
        // otherwise leave the cups unclickable the next time this minigame loads.
        shufflefinished = false;
        gamefinishedCUP = false;
        

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
        if (PauseMenuManager.IsPaused) return; // cups move per frame, so freeze them while paused
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
