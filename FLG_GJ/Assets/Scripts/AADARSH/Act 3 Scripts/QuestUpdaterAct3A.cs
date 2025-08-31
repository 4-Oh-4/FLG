using UnityEngine;

public class QuestUpdaterAct3A : QuestUpdaterAct1A {
    [SerializeField] Transform churchb;
    [SerializeField] Transform Homeb;
    [SerializeField] Transform Market;
    [SerializeField] Transform darius;
    [SerializeField] Transform club;
    [SerializeField] Transform clubNino;
    [SerializeField] Transform ninoOfficeDoor;
    [SerializeField] Transform EmilioShack;
    [SerializeField] Transform Bigshack;
    [SerializeField] Transform middelman;
    [SerializeField] Transform smallshack;

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
    
    public void DariusIntro() {
        if (questPointer.gameObject.activeInHierarchy == false) {
            EnablePointerb();
        }
        questPointer.target = darius;
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
    public void Emilioshack() {
        if (questPointer.gameObject.activeInHierarchy == false) {
            EnablePointerb();
        }
        questPointer.target = EmilioShack;
    }
    public void BigShack() {
        if (questPointer.gameObject.activeInHierarchy == false) {
            EnablePointerb();
        }
        questPointer.target = Bigshack;
    }
    public void Middelman() {
        if (questPointer.gameObject.activeInHierarchy == false) {
            EnablePointerb();
        }
        questPointer.target = middelman;
    }
    public void SmallShack() {
        if (questPointer.gameObject.activeInHierarchy == false) {
            EnablePointerb();
        }
        questPointer.target = smallshack;
    }
}
