using UnityEngine;

public class GameManager_Drug : MonoBehaviour
{
    public static GameManager_Drug Instance { get; private set; }
    public bool HasPackage { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Time.timeScale = 1f;
    }

    public void OnPackageCollected()
    {
        HasPackage = true;
        Debug.Log("Package collected – get to extraction!");
    }

    public void OnPlayerCaught()
    {
        if (Time.timeScale == 0f) return;
        Debug.Log("Caught! Game Over.");
        Time.timeScale = 0f; // quick pause; replace with UI later
    }

    public void TryWin()
    {
        if (HasPackage)
        {
            Debug.Log("You escaped with the package! You win.");
            Time.timeScale = 0f;
        }
    }
}
