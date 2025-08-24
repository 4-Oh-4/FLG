using UnityEngine;

namespace TruckChase
{
    public class GameManager_D : MonoBehaviour
    {
        // A "Singleton" instance so other scripts can easily access it.
        public static GameManager_D Instance;

        [Header("Progression")]
        [Tooltip("How long the escape takes in seconds.")]
        [SerializeField] public float timeToEscape = 120f; // e.g., 2 minutes
        public float currentProgress { get; private set; } // Tracks current escape progress
        public bool isGameOver { get; private set; } = false;

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            // If the game is not over, increase the escape progress over time.
            if (!isGameOver)
            {
                currentProgress += Time.deltaTime;

                if (currentProgress >= timeToEscape)
                {
                    WinGame();
                }
            }
        }

        public void WinGame()
        {
            if (isGameOver) return; // Prevent this from being called multiple times.
            isGameOver = true;
            Debug.Log("YOU ESCAPED! VICTORY!");
            // Here you can stop enemy spawning and show a win screen.
        }

        public void LoseGame()
        {
            if (isGameOver) return;
            isGameOver = true;
            Debug.Log("LORRY DESTROYED! GAME OVER!");
            // Here you can stop enemy spawning and show a lose screen.
        }
    }
}