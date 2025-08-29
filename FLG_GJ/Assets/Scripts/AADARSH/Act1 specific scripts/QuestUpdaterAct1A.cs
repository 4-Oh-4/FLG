using UnityEngine;

public class QuestUpdaterAct1A : MonoBehaviour
{

    [SerializeField] Transform SilvioTransform;
    [SerializeField] Transform collectionTransform;
    [SerializeField] Transform GateToGoOut;
    [SerializeField] Transform GettingBeaten;
    [SerializeField] Transform EvangalicMission;
    [SerializeField] Transform thugCampFire;
    [SerializeField] Transform T_Evangalic;
    [SerializeField] Transform church;
    [SerializeField] Transform WasteToBeCleaned;
    [SerializeField] Transform cook;
    [SerializeField] Transform Home;
    [SerializeField] WindowQuestPointer_A questPointer_A;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void InitialNaration() {
        if (questPointer_A.gameObject.activeInHierarchy == false) {
            EnablePointer();
        }
        questPointer_A.target = collectionTransform;
    }
    public void WeeksLater() {
        
        if (questPointer_A.gameObject.activeInHierarchy == false) {
            EnablePointer();
        }
        questPointer_A.target = GateToGoOut;
    }
    public void IsDown() {
        if (questPointer_A.gameObject.activeInHierarchy == false) {
            EnablePointer();
        }
        questPointer_A.target = GettingBeaten;
    }
    public void EvangalicTalk() {
        if (questPointer_A.gameObject.activeInHierarchy == false) {
            EnablePointer();
        }
        questPointer_A.target = EvangalicMission;
    }
    public void EvangalicTalkDone() {
        if (questPointer_A.gameObject.activeInHierarchy == false) {
            EnablePointer();
        }
        questPointer_A.target = SilvioTransform;
    }
    public void disablePointer() {
        questPointer_A.gameObject.SetActive(false);
    }
    public void EnablePointer() {
        questPointer_A.gameObject.SetActive(true);
    }
    public void ThugCampFire() {
        if (questPointer_A.gameObject.activeInHierarchy == false) {
            EnablePointer();
        }
        questPointer_A.target = thugCampFire;
    }
    public void GiveDrug() {
        if (questPointer_A.gameObject.activeInHierarchy == false) {
            EnablePointer();
        }
        questPointer_A.target = T_Evangalic;
    }

    public void Church() {
        if (questPointer_A.gameObject.activeInHierarchy == false) {
            EnablePointer();
        }
        questPointer_A.target = church;
    }
    public void Cleaning() {
        if (questPointer_A.gameObject.activeInHierarchy == false) {
            EnablePointer();
        }
        questPointer_A.target = WasteToBeCleaned;
    }
    public void Cooking() {
        if (questPointer_A.gameObject.activeInHierarchy == false) {
            EnablePointer();
        }
        questPointer_A.target = cook;
    }
    public void GoHome() {
        if (questPointer_A.gameObject.activeInHierarchy == false) {
            EnablePointer();
        }
        questPointer_A.target = Home;
    }
}
