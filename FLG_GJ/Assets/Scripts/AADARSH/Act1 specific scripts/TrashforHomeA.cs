using UnityEngine;

public class TrashforHomeA : MonoBehaviour
{
    private GameManagerAct1A gameM;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameM = FindAnyObjectByType<GameManagerAct1A>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void CollectTrash() {
        if (gameM != null) gameM.UpdateTrash();
        Destroy(gameObject);
    }
}
