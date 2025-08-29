using UnityEngine;

public class QuestUpdaterAct1A : MonoBehaviour
{

    [SerializeField] private Transform SilvioTransform;
    [SerializeField] private Transform collectionTransform;
    [SerializeField] private Transform GateToGoOut;
    [SerializeField] private Transform GettingBeaten;
    [SerializeField] private Transform EvangalicMission;
    [SerializeField] private Transform thugCampFire;
    [SerializeField] private Transform T_Evangalic;
    [SerializeField] private Transform church;
    [SerializeField] private Transform WasteToBeCleaned;
    [SerializeField] private Transform cook;
    [SerializeField] private Transform Home;
    [SerializeField] private WindowQuestPointer_A questPointer_A;
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
    public virtual void disablePointer() {
        questPointer_A.gameObject.SetActive(false);
    }
    public virtual void EnablePointer() {
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

    public virtual void Church() {
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
    public virtual void GoHome() {
        if (questPointer_A.gameObject.activeInHierarchy == false) {
            EnablePointer();
        }
        questPointer_A.target = Home;
    }
}
