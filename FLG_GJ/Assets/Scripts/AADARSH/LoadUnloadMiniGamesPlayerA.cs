using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LoadUnloadMiniGamesPlayerA : MonoBehaviour {
    [SerializeField] Camera maincamera;
    [SerializeField] GameObject player;
    [SerializeField] QuestUpdaterAct1A questupdater;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void LoadMiniGame(string miniGameName) {
        //Time.timeScale = 0f;
        Scene miniScene = SceneManager.GetSceneByName(miniGameName);
        maincamera.gameObject.SetActive(false);
        player.SetActive(false);
        SceneManager.LoadScene(miniGameName,LoadSceneMode.Additive);
        questupdater.disablePointer();
    }
    public void UnloadMiniGame(string name="DrugTheft") {
        Time.timeScale = 1f;
        questupdater.EnablePointer();
        maincamera.gameObject.SetActive(true);
        player.SetActive(true);
        FindAnyObjectByType<ShowingStoryUpdates1>().ShowUpdate("asd");
        StoryManagertAct1A.Instance.SetFlag(name.ToString()+"Complete", true);
    }
}