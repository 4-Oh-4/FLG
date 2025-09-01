using UnityEngine;

public class PersistentAudio : MonoBehaviour
{
    // A static variable to hold the single instance of this object.
    public static PersistentAudio instance;

    void Awake()
    {
        // Check if an instance of this object already exists.
        if (instance == null)
        {
            // If not, set this as the instance and make it persistent.
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // If an instance already exists, destroy this new one.
            Destroy(gameObject);
        }
    }
}