using UnityEngine;
using UnityEngine.SceneManagement;

public class GM_DJ_A : MonoBehaviour
{
    [SerializeField] private bool isCompleted = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (isCompleted) TryWin();
    }
    public void TryWin() {
        if (true) {
            
            FindAnyObjectByType<LoadUnloadMiniGamesPlayerA>().UnloadMiniGame("DJ_MiniGame_D");
            Debug.Log("You escaped with the package! You win.");
            SceneManager.UnloadSceneAsync("DJ_MiniGame_D");


        }
    }
}
