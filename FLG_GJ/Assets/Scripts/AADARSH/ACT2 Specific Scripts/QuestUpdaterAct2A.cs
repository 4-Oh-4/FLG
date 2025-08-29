using UnityEngine;

public class QuestUpdaterAct2A : QuestUpdaterAct1A
{
    [SerializeField] Transform churchb;
    [SerializeField] Transform Homeb;
    [SerializeField] Transform Market;
    [SerializeField] Transform Disturbance;
    [SerializeField] Transform darius;
    [SerializeField] Transform Meetingarea;
    [SerializeField] Transform tunnel;
    [SerializeField] Transform club;

    [SerializeField] WindowQuestPointer_A questPointer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void disablePointer() {
        questPointer.gameObject.SetActive(false);
    }
    public override void EnablePointer() {
        questPointer.gameObject.SetActive(true);
    }
    public override void Church() {
        if (questPointer.gameObject.activeInHierarchy == false) {
            EnablePointer();
        }
        questPointer.target = churchb;
    }
    public override void GoHome() {
        if (questPointer.gameObject.activeInHierarchy == false) {
            EnablePointer();
        }
        questPointer.target = Homeb;
    }
    public void GoMarket() {
        if (questPointer.gameObject.activeInHierarchy == false) {
            EnablePointer();
        }
        questPointer.target = Market;
    }
    public void CheckDisturbance() {
        if (questPointer.gameObject.activeInHierarchy == false) {
            EnablePointer();
        }
        questPointer.target = Disturbance;
    }
    public void MeetingArea() {
        if (questPointer.gameObject.activeInHierarchy == false) {
            EnablePointer();
        }
        questPointer.target = Meetingarea;
    }
    public void DariusIntro() {
        if (questPointer.gameObject.activeInHierarchy == false) {
            EnablePointer();
        }
        questPointer.target = darius;
    }
    public void Tunnel() {
        if (questPointer.gameObject.activeInHierarchy == false) {
            EnablePointer();
        }
        questPointer.target = tunnel;
    }
    public void Club() {
        if (questPointer.gameObject.activeInHierarchy == false) {
            EnablePointer();
        }
        questPointer.target = club;
    }
}
