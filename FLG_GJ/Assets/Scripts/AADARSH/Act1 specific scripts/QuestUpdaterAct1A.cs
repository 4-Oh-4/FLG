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
    [SerializeField] private Transform foodCollect;
    [SerializeField] private WindowQuestPointer_A questPointer_A;

    [Header("Mission Text")]
    [Tooltip("Reference to the HUD quest text component.")]
    [SerializeField] protected QuestTextA questText;

    // Helper method to reduce boilerplate — enables pointer, sets target, and updates mission text.
    protected void SetWaypoint(Transform target, string missionText) {
        if (questPointer_A != null && questPointer_A.gameObject.activeInHierarchy == false) {
            EnablePointer();
        }
        if (questPointer_A != null) {
            questPointer_A.target = target;
        }
        if (questText != null && !string.IsNullOrEmpty(missionText)) {
            questText.setQuest(missionText);
        }
    }

    public void InitialNaration() {
        SetWaypoint(collectionTransform, "Collect food and clean up trash around the house");
    }
    public void WeeksLater() {
        SetWaypoint(GateToGoOut, "Head to the gate to leave the area");
    }
    public void IsDown() {
        SetWaypoint(GettingBeaten, "Something's happening — go check it out");
    }
    public void EvangalicTalk() {
        SetWaypoint(EvangalicMission, "Talk to the Evangelic preacher");
    }
    public void EvangalicTalkDone() {
        SetWaypoint(SilvioTransform, "Go talk to Silvio");
    }
    public virtual void disablePointer() {
        if (questPointer_A != null) questPointer_A.gameObject.SetActive(false);
    }
    public virtual void EnablePointer() {
        if (questPointer_A != null) questPointer_A.gameObject.SetActive(true);
    }
    public void ThugCampFire() {
        SetWaypoint(thugCampFire, "Head to the thug campfire");
    }
    public void GiveDrug() {
        SetWaypoint(T_Evangalic, "Deliver the package to the Evangelic");
    }

    public virtual void Church() {
        SetWaypoint(church, "Go to the Church");
    }
    public void Cleaning() {
        SetWaypoint(WasteToBeCleaned, "Clean up the waste");
    }
    public void Cooking() {
        SetWaypoint(cook, "Time to cook — head to the kitchen");
    }
    public virtual void GoHome() {
        SetWaypoint(Home, "Go back home");
    }
    public virtual void GoFood() {
        SetWaypoint(foodCollect, "Collect the food");
    }
}
