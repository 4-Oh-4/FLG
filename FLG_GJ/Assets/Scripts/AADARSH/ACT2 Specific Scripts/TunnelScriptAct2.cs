using UnityEngine;

public class TunnelScriptAct2 : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private int count = 1;
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player") && count == 0) {
            StoryManagertAct1A.Instance.SetFlag("TopDownShooterDrugsMiniGame",true);
        }
    }
    public void setCount() {
        count = 0;
    }
}
