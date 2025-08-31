using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LoadUnloadMiniGamesPlayerA : MonoBehaviour {
    [SerializeField] Camera maincamera;
    [SerializeField] GameObject player;
    [SerializeField] QuestUpdaterAct1A questupdater;
    [SerializeField] QuestUpdaterAct2A quesupdate;
    [SerializeField] GameObject light;
    [SerializeField] GameObject mainGameEvent;
    bool state;
    private int quest = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void LoadMiniGame(string miniGameName) {
        //Time.timeScale = 0f;
        if(mainGameEvent!=null)
        mainGameEvent.SetActive(false);
        if (light != null) {
            state = light.activeInHierarchy;
            light.SetActive(false);
        }
        Scene miniScene = SceneManager.GetSceneByName(miniGameName);
        maincamera.gameObject.SetActive(false);
        player.SetActive(false);
        SceneManager.LoadScene(miniGameName,LoadSceneMode.Additive);
        if (quesupdate == null) {
            questupdater.disablePointer();
            quest = 1;
        } else {
            quesupdate.disablePointerb();
            quest = 2;
        }
    }
    public void UnloadMiniGame(string name="DrugTheft") {

        Time.timeScale = 1f;
        if (light != null) {


            light.SetActive(state);
        }
        if (quest==1)questupdater.EnablePointer();
        else {
            quesupdate.EnablePointerb();
        }
        maincamera.gameObject.SetActive(true);
        player.SetActive(true);
        FindAnyObjectByType<ShowingStoryUpdates1>().ShowUpdate("asd");
        StoryManagertAct1A.Instance.SetFlag(name.ToString()+"Complete", true);
        if (mainGameEvent != null)
            mainGameEvent.SetActive(true);
    }
}