using UnityEngine;
using UnityEngine.Events;

// AMENDED: Added the namespace to match the other scripts.
namespace TopDownShooter
{
    public class Health_D : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 100f;
        private float currentHealth;

        public UnityEvent onDeath;
        public UnityEvent<float, float> OnHealthChanged;

        private void Start()
        {
            currentHealth = maxHealth;
            OnHealthChanged.Invoke(currentHealth, maxHealth);
        }

        public void TakeDamage(float damage)
        {
            if (currentHealth <= 0) return;

            currentHealth -= damage;
            OnHealthChanged.Invoke(currentHealth, maxHealth);
            Debug.Log(gameObject.name + " Health: " + currentHealth);

            if (currentHealth <= 0)
            {
                onDeath.Invoke();
            }
        }
    }
}