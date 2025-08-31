using UnityEngine;

public class TpPlayerWareHouse : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] GameObject player;
    [SerializeField] Transform warehouse;
    [SerializeField] Transform tunnel;
    [SerializeField] Transform Garrage;
    [SerializeField] Transform Bb;
    [SerializeField] Transform nightClub;
    [SerializeField] Transform home;
    [SerializeField] Transform NinoOffice;
    [SerializeField] Transform bathRoomSpawn;
    [SerializeField] Transform shackArea;
    [SerializeField] Transform bigShack;
    [SerializeField] Transform finalShack;
    [SerializeField] Transform outsideFinalShack;
    public void TPtoWareHouse() {
        FindAnyObjectByType<ShowingStoryUpdates1>().ShowUpdate("as");
        player.transform.position = warehouse.position;
    }
    public void TPtoTunnel() {
        FindAnyObjectByType<ShowingStoryUpdates1>().ShowUpdate("as");
        player.transform.position = tunnel.position;
    }
    public void TPtoGarage() {
        FindAnyObjectByType<ShowingStoryUpdates1>().ShowUpdate("as");
        player.transform.position = Garrage.position;
    }
    public void TPtoBB() {
        FindAnyObjectByType<ShowingStoryUpdates1>().ShowUpdate("as");
        player.transform.position = Bb.position;
    }
    public void TPtoNightClub() {
        FindAnyObjectByType<ShowingStoryUpdates1>().ShowUpdate("as");
        player.transform.position = nightClub.position;
    }
    public void TPtoHome() {
        Debug.Log("TPtoHome() function was called! Teleporting to Home.");
        FindAnyObjectByType<ShowingStoryUpdates1>().ShowUpdate("as");
        player.transform.position = home.position;
    }
    public void NinoOfficespawn() {
        FindAnyObjectByType<ShowingStoryUpdates1>().ShowUpdate("as");
        player.transform.position = NinoOffice.position;
    }
    public void BathRoomSpawn() {
        FindAnyObjectByType<ShowingStoryUpdates1>().ShowUpdate("as");
        player.transform.position = bathRoomSpawn.position;
    }
    public void ShackArea() {
        FindAnyObjectByType<ShowingStoryUpdates1>().ShowUpdate("as");
        player.transform.position = shackArea.position;
    }
    public void BigShack() {
        Debug.Log("TPtoBigShack() function was called! Teleporting to bigshack.");
        FindAnyObjectByType<ShowingStoryUpdates1>().ShowUpdate("as");
        player.transform.position = bigShack.position;
    }
    public void FinalShack() {
        Debug.Log("TPtoBigShack() function was called! Teleporting to bigshack.");
        FindAnyObjectByType<ShowingStoryUpdates1>().ShowUpdate("as");
        player.transform.position = finalShack.position;
    }
    public void OutsideFinalShack() {
        Debug.Log("TPtoBigShack() function was called! Teleporting to bigshack.");
        FindAnyObjectByType<ShowingStoryUpdates1>().ShowUpdate("as");
        player.transform.position = outsideFinalShack.position;
    }
}
