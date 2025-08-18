using UnityEngine;
using UnityEngine.Events;

namespace TopDownShooter
{
    // This script now only manages health values and triggers a death event.
    public class Health_D : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 100f;
        private float currentHealth;

        public UnityEvent onDeath;

        private void Start()
        {
            currentHealth = maxHealth;
        }

        public void TakeDamage(float damage)
        {
            if (currentHealth <= 0) return;

            currentHealth -= damage;
            Debug.Log(gameObject.name + " Health: " + currentHealth);

            if (currentHealth <= 0)
            {
                onDeath.Invoke();
            }
        }
    }
}