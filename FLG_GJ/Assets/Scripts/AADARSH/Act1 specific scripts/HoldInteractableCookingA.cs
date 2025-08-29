using UnityEngine;

public class HoldInteractableCookingA : HoldInteractableA {
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override  void DestroyNow() {
        GetComponent<Renderer>().material.color = Color.yellow;
        gameObject.GetComponent<Collider2D>().enabled = false;
        StoryManagertAct1A.Instance.SetFlag("EvangalicTask", true);
        Debug.Log("Cooking Completed");
        gameObject.GetComponent<HoldInteractableA>().enabled = false;
    }
}
