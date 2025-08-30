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
    [SerializeField] Transform larryChanga;
    [SerializeField] Transform gateWarehouse;
    [SerializeField] Transform BBarea;
    [SerializeField] Transform johan;
    [SerializeField] Transform truckGate;
    [SerializeField] Transform DariusChase;
    [SerializeField] Transform clubDarius;
    [SerializeField] Transform clubRoxana;
    [SerializeField] Transform clubRositta;
    [SerializeField] Transform clubNino;
    [SerializeField] Transform ninoOfficeDoor;

    [SerializeField] WindowQuestPointer_A questPointer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void disablePointerb() {
        questPointer.gameObject.SetActive(false);
    }
    public void EnablePointerb() {
        questPointer.gameObject.SetActive(true);
    }
    public void GoHomeb() {
        if (questPointer.gameObject.activeInHierarchy == false) {
            EnablePointerb();
        }
        questPointer.target = Homeb;
    }
    public void GoMarket() {
        if (questPointer.gameObject.activeInHierarchy == false) {
            EnablePointerb();
        }
        questPointer.target = Market;
    }
    public void CheckDisturbance() {
        if (questPointer.gameObject.activeInHierarchy == false) {
            EnablePointerb();
        }
        questPointer.target = Disturbance;
    }
    public void MeetingArea() {
        if (questPointer.gameObject.activeInHierarchy == false) {
            EnablePointerb();
        }
        questPointer.target = Meetingarea;
    }
    public void DariusIntro() {
        if (questPointer.gameObject.activeInHierarchy == false) {
            EnablePointerb();
        }
        questPointer.target = darius;
    }
    public void Tunnel() {
        if (questPointer.gameObject.activeInHierarchy == false) {
            EnablePointerb();
        }
        questPointer.target = tunnel;
    }
    public void Club() {
        if (questPointer.gameObject.activeInHierarchy == false) {
            EnablePointerb();
        }
        questPointer.target = club;
    }
    public void Churchb() {
        if (questPointer.gameObject.activeInHierarchy == false) {
            EnablePointerb();
        }
        questPointer.target = churchb;
    }
    public void LarryChanga() {
        if (questPointer.gameObject.activeInHierarchy == false) {
            EnablePointerb();
        }
        questPointer.target = larryChanga;
    }
    public void GateWareHouse() {
        if (questPointer.gameObject.activeInHierarchy == false) {
            EnablePointerb();
        }
        questPointer.target = gateWarehouse;
    }
    public void BbArea() {
        if (questPointer.gameObject.activeInHierarchy == false) {
            EnablePointerb();
        }
        questPointer.target = BBarea;
    }
    public void Johan() {
        if (questPointer.gameObject.activeInHierarchy == false) {
            EnablePointerb();
        }
        questPointer.target = johan;
    }
    public void TruckGate() {
        if (questPointer.gameObject.activeInHierarchy == false) {
            EnablePointerb();
        }
        questPointer.target = truckGate;
    }
    public void BackFromTruckChase() {
        if (questPointer.gameObject.activeInHierarchy == false) {
            EnablePointerb();
        }
        questPointer.target = DariusChase;
    }
    public void ClubDarius() {
        if (questPointer.gameObject.activeInHierarchy == false) {
            EnablePointerb();
        }
        questPointer.target = clubDarius;
    }
    public void ClubRoxana() {
        if (questPointer.gameObject.activeInHierarchy == false) {
            EnablePointerb();
        }
        questPointer.target = clubRoxana;
    }
    public void ClubRositta() {
        if (questPointer.gameObject.activeInHierarchy == false) {
            EnablePointerb();
        }
        questPointer.target = clubRositta;
    }
    public void ClubNino() {
        if (questPointer.gameObject.activeInHierarchy == false) {
            EnablePointerb();
        }
        questPointer.target = clubNino;
    }
    public void OfficeDoor() {
        if (questPointer.gameObject.activeInHierarchy == false) {
            EnablePointerb();
        }
        questPointer.target = ninoOfficeDoor;
    }
}
