using UnityEngine;

public class GateOutColliderChurchA : MonoBehaviour
{
    [SerializeField] BackToMainGameA backToMainGame;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            Debug.Log("Going Out");
            backToMainGame.CompleteMiniGame();
        }
    }
}
