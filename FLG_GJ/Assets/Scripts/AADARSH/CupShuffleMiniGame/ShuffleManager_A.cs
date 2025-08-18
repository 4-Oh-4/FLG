using UnityEngine;

public class ShuffleManager_A : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private CupShuffleA cupshuffle;
    [SerializeField] private Transform ball;
    [SerializeField] private Transform cup;
    [SerializeField] private Vector2 offset;
    [SerializeField] private float movementSpeed = 3f;
    [SerializeField] private Vector2 initial;
    private bool moveUPwards = false;
    private bool moveDOWNwards = false;
    void Start()
    {
        initial = cup.position;

    }
    public void StartAgain() {
        moveUPwards = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) {
            StartAgain();
        }
        if (moveUPwards) {
            cup.position = Vector2.Lerp(cup.position, initial + offset, movementSpeed);
            if (Mathf.Abs(cup.position.y - initial.y - offset.y) < 0.001) {
                cup.position = initial + offset;
                moveUPwards = false;
                moveDOWNwards = true;
            }
        }
        if (moveDOWNwards) {
            cup.position = Vector2.Lerp(cup.position, initial, movementSpeed);
            if (Mathf.Abs(cup.position.y - initial.y) < 0.001) {
                cup.position = initial;
                moveDOWNwards = false;
                cupshuffle.StartShuffle();
            }
        }
    }
}
