using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerTDSA : MonoBehaviour {
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
            GameObject[] enemiesToDestroy = GameObject.FindGameObjectsWithTag("Enemy");

            // 2. Loop through every enemy found in the array.
            foreach (GameObject enemy in enemiesToDestroy) {
                // 3. Destroy the enemy GameObject.
                Destroy(enemy);
            }
            FindAnyObjectByType<LoadUnloadMiniGamesPlayerA>().UnloadMiniGame("TopDownShooter_Drugs_ACT2");
            Debug.Log("You escaped with the package! You win.");
            SceneManager.UnloadSceneAsync("TopDownShooter_Drugs_ACT2");


        }
    }
}
