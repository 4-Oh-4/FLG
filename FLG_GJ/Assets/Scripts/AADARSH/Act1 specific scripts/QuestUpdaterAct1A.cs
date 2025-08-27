using UnityEngine;

public class QuestUpdaterAct1A : MonoBehaviour
{

    [SerializeField] Transform SilvioTransform;
    [SerializeField] Transform collectionTransform;
    [SerializeField] Transform GateToGoOut;
    [SerializeField] WindowQuestPointer_A questPointer_A;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void InitialNaration() {
        if (questPointer_A.gameObject.activeInHierarchy == false) {
            EnablePointer();
        }
        questPointer_A.target = collectionTransform;
    }
    public void disablePointer() {
        questPointer_A.gameObject.SetActive(false);
    }
    public void EnablePointer() {
        questPointer_A.gameObject.SetActive(true);
    }
}
