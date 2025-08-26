using UnityEngine;

public class FoodCollectionA : MonoBehaviour {
    // A private variable to hold a reference to our game manager.
    private GameManagerAct1A gameManager;

    // Start is called once before the first frame update.
    // It's a good place to find and store references.
    void Start() {
        // Find the instance of GameManagerAct1A in the scene and store it.
        // This is efficient because it only runs once when the food is created.
        gameManager = FindObjectOfType<GameManagerAct1A>();

        // A safety check to make sure the manager was actually found.
        if (gameManager == null) {
            Debug.LogError("GameManagerAct1A was not found in the scene! Food cannot be collected.", this.gameObject);
        }
    }

    /// <summary>
    /// This is a special Unity function that is automatically called
    /// whenever another collider enters this object's trigger collider.
    /// </summary>
    /// <param name="other">The collider of the object that entered the trigger.</param>
    private void OnTriggerEnter2D(Collider2D other) {
        // We check if the object that entered has the tag "Player".
        // This is the standard way to identify the player.
        if (other.CompareTag("Player")) {
            Debug.Log("Player collected food.");

            // If the game manager was found...
            if (gameManager != null) {
                // ...call its public method to update the food count.
                gameManager.UpdateFood();
            }

            // Destroy the food item GameObject that this script is attached to.
            Destroy(gameObject);
        }
    }
}