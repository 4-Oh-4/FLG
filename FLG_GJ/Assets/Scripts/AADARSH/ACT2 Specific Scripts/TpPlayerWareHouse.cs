using UnityEngine;

public class TpPlayerWareHouse : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] GameObject player;
    [SerializeField] Transform warehouse;
    [SerializeField] Transform tunnel;
    public void TPtoWareHouse() {
        FindAnyObjectByType<ShowingStoryUpdates1>().ShowUpdate("as");
        player.transform.position = warehouse.position;
    }
    public void TPtoTunnel() {
        FindAnyObjectByType<ShowingStoryUpdates1>().ShowUpdate("as");
        player.transform.position = tunnel.position;
    }
}
