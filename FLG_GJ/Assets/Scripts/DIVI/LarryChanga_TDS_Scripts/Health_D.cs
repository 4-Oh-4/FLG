using UnityEngine;
using UnityEngine.Events;

namespace TopDownShooter
{
    // A simple component to give any object health and a death event.
    public class Health_D : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 100f;
        private float currentHealth;

        // An event that fires in the Inspector when health reaches zero.
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