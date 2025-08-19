using UnityEngine;
using UnityEngine.Events;

namespace TopDownShooter
{
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

            if (currentHealth <= 0)
            {
                // AMENDED: Added new logic to automatically handle loot drops.
                // Try to find a LootDrop_D component on this same GameObject.
                LootDrop_D lootDropper = GetComponent<LootDrop_D>();
                // If one was found, tell it to spawn its loot.
                if (lootDropper != null)
                {
                    lootDropper.SpawnLoot();
                }

                // The onDeath event can still be used for other things (like disabling the object).
                onDeath.Invoke();
            }
        }
    }
}