using UnityEngine;

public class EvangalicTriggerScript : MonoBehaviour
{
    bool once = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")&&StoryManagertAct1A.Instance.GetFlag("SilvioBeatenToRoof")&&once) {
            once = false;
            gameObject.GetComponent<NPC_A>().TriggerDialouge();
            gameObject.GetComponent<EvangalicTriggerScript>().enabled = false;
        }
    }
}
