using UnityEngine;

public class GateToGoUpAct1_A : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        Debug.Log("Collision detected");
        if (collision.gameObject.name=="Silvio2") {
            Destroy(collision.gameObject);
            StoryManagertAct1A.Instance.SetFlag("SilvioBeatenToRoof", true);
        }
    }

}
