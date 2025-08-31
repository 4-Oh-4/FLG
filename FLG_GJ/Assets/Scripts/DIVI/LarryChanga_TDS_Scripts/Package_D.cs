using UnityEngine;

namespace TopDownShooter
{
    public class Package_D : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 2000f;
        private float currentHealth;

        private void Start()
        {
            currentHealth = maxHealth;
        }

        public void TakeDamage(float damage)
        {
            if (currentHealth <= 0) return;

            currentHealth -= damage;
            Debug.Log("Package Health: " + currentHealth);

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        void Die()
        {
            Debug.Log("Package Destroyed! Game Over!");
            // Deactivate the package to show it's been destroyed.
            gameObject.SetActive(false);
            FindAnyObjectByType<GM_DJ_A>().Fail();
        }
    }
}