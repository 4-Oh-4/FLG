using UnityEngine;
using TMPro;

public class GameManagerAct1A : MonoBehaviour {
    [Header("Collection Goals")]
    [SerializeField] int trashTobeCollected = 5;
    [SerializeField] int foodTobeCollected = 5;

    [Header("UI References")]
    [SerializeField] GameObject uiPanel;          // parent UI panel (enable/disable this)
    [SerializeField] TextMeshProUGUI trashText;   // trash counter
    [SerializeField] TextMeshProUGUI foodText;    // food counter

    void Start() {
        UpdateUI();
        HideUI(); // start with UI hidden (optional)
    }

    public void UpdateTrash() {
        if (trashTobeCollected > 0) {
            trashTobeCollected--;
            UpdateUI();
        }
    }

    public void UpdateFood() {
        if (foodTobeCollected > 0) {
            foodTobeCollected--;
            UpdateUI();
        }
    }

    private void UpdateUI() {
        if (trashText != null)
            trashText.text = "Trash Left: " + trashTobeCollected;

        if (foodText != null)
            foodText.text = "Food Left: " + foodTobeCollected;

        if (trashTobeCollected == 0 && foodTobeCollected == 0) HideUI();

    }

    // === UI Show/Hide Functions ===
    public void ShowUI() {
        if (uiPanel != null)
            uiPanel.SetActive(true);
    }

    public void HideUI() {
        if (uiPanel != null)
            uiPanel.SetActive(false);
    }
}
