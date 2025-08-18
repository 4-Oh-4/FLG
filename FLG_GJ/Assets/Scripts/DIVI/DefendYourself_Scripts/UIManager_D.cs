using UnityEngine;
using UnityEngine.UI; // Required for working with UI elements like Slider

public class UIManager_D : MonoBehaviour
{
    [SerializeField] private Slider playerHealthSlider;
    [SerializeField] private Slider enemyHealthSlider;

    // This function will be called by the player script
    public void UpdatePlayerHealth(float currentHealth, float maxHealth)
    {
        playerHealthSlider.value = currentHealth / maxHealth;
    }

    // This function will be called by the enemy script
    public void UpdateEnemyHealth(float currentHealth, float maxHealth)
    {
        enemyHealthSlider.value = currentHealth / maxHealth;
    }
}