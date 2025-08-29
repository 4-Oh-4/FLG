using UnityEngine;

public class QuestUpdaterAct2A : MonoBehaviour
{
    [SerializeField] Transform church;
    [SerializeField] Transform Home;
    [SerializeField] Transform Market;
    [SerializeField] Transform Disturbance;
    [SerializeField] Transform darius;
    [SerializeField] Transform Meetingarea;
    [SerializeField] Transform tunnel;
    [SerializeField] Transform club;

    [SerializeField] WindowQuestPointer_A questPointer_A;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void disablePointer() {
        questPointer_A.gameObject.SetActive(false);
    }
    public void EnablePointer() {
        questPointer_A.gameObject.SetActive(true);
    }
    public void Church() {
        if (questPointer_A.gameObject.activeInHierarchy == false) {
            EnablePointer();
        }
        questPointer_A.target = church;
    }
    public void GoHome() {
        if (questPointer_A.gameObject.activeInHierarchy == false) {
            EnablePointer();
        }
        questPointer_A.target = Home;
    }
    public void GoMarket() {
        if (questPointer_A.gameObject.activeInHierarchy == false) {
            EnablePointer();
        }
        questPointer_A.target = Market;
    }
    public void CheckDisturbance() {
        if (questPointer_A.gameObject.activeInHierarchy == false) {
            EnablePointer();
        }
        questPointer_A.target = Disturbance;
    }
    public void MeetingArea() {
        if (questPointer_A.gameObject.activeInHierarchy == false) {
            EnablePointer();
        }
        questPointer_A.target = Meetingarea;
    }
    public void DariusIntro() {
        if (questPointer_A.gameObject.activeInHierarchy == false) {
            EnablePointer();
        }
        questPointer_A.target = darius;
    }
    public void Tunnel() {
        if (questPointer_A.gameObject.activeInHierarchy == false) {
            EnablePointer();
        }
        questPointer_A.target = tunnel;
    }
    public void Club() {
        if (questPointer_A.gameObject.activeInHierarchy == false) {
            EnablePointer();
        }
        questPointer_A.target = club;
    }
}
