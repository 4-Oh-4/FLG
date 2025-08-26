using UnityEngine;
using TMPro;

public class GameManagerAct1A : MonoBehaviour {
    [Header("Collection Goals")]
    [SerializeField] int trashTobeCollected = 5;
    [SerializeField] int foodTobeCollected = 5;
    [SerializeField] Transform trash;
    [SerializeField] WindowQuestPointer_A questPointer;

    [Header("UI References")]
    [SerializeField] GameObject uiPanel;
    [SerializeField] TextMeshProUGUI trashText;
    [SerializeField] TextMeshProUGUI foodText;

    // --- ADDED ---
    // A reference to the food spawner script.
    private FoodSpawnerAct1A foodSpawner;

    void Start() {
        // --- ADDED ---
        // Find the FoodSpawnerAct1A component on any of this object's children.
        foodSpawner = GetComponentInChildren<FoodSpawnerAct1A>();
        if (foodSpawner == null) {
            Debug.LogError("GameManager could not find the FoodSpawner in its children!", this.gameObject);
        }
        // -------------

        questPointer = FindAnyObjectByType<WindowQuestPointer_A>();
        UpdateUI();
        if (uiPanel != null)
            uiPanel.SetActive(false);
    }

    // ... (UpdateTrash and UpdateFood methods are unchanged) ...
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


    // --- MODIFIED ShowUI Method ---
    public void ShowUI() {
        if (uiPanel != null)
            uiPanel.SetActive(true);

        questPointer.target = trash;

        // --- ADDED ---
        // Tell the spawner to start its work.
        if (foodSpawner != null) {
            foodSpawner.StartSpawning();
        }
    }

    // --- MODIFIED HideUI Method ---
    public void HideUI() {
        if (uiPanel != null)
            uiPanel.SetActive(false);

        // This line might cause an error if the target has been destroyed.
        // It's safer to check first.
        if (questPointer.target != null) {
            questPointer.gameObject.SetActive(false);
        }

        // --- ADDED ---
        // Tell the spawner to stop its work.
        if (foodSpawner != null) {
            foodSpawner.StopSpawning();
        }
    }
}