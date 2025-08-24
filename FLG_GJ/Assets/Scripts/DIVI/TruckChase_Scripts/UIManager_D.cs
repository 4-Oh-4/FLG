using UnityEngine;
using UnityEngine.UI;

namespace TruckChase
{
    public class UIManager_D : MonoBehaviour
    {
        [SerializeField] private Slider healthSlider;
        [SerializeField] private Slider escapeSlider;
        [SerializeField] private LorryHealth_D lorryHealth;

        void Update()
        {
            // Constantly update the UI sliders with the latest game data.
            if (lorryHealth != null)
            {
                healthSlider.value = lorryHealth.GetCurrentHealth() / lorryHealth.GetMaxHealth();
            }

            if (GameManager_D.Instance != null)
            {
                escapeSlider.value = GameManager_D.Instance.currentProgress / GameManager_D.Instance.timeToEscape;
            }
        }
    }
}