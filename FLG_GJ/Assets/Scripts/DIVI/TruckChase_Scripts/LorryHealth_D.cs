using UnityEngine;

namespace TruckChase
{
    public class LorryHealth_D : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 200f;
        private float currentHealth;

        private void Start()
        {
            currentHealth = maxHealth;
        }

        public void TakeDamage(float damage)
        {
            if (currentHealth <= 0) return;
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                Die();

            }
        }

        void Die()
        {
            // AMENDED: Tell the GameManager that the player has lost.
            if (GameManager_D.Instance != null)
            {
                GameManager_D.Instance.LoseGame();
            }
            FindAnyObjectByType<GM_DJ_A>().Fail();

            gameObject.SetActive(false);
        }

        // AMENDED: Added getter functions for the UI Manager to read.
        public float GetCurrentHealth() { return currentHealth; }
        public float GetMaxHealth() { return maxHealth; }
    }
}