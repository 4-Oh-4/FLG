using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LoadUnloadMiniGamesPlayerA : MonoBehaviour {
    [SerializeField] Camera maincamera;
    [SerializeField] GameObject player;
    [SerializeField] QuestUpdaterAct1A questupdater;
    [SerializeField] QuestUpdaterAct2A quesupdate;
    [SerializeField] QuestUpdaterAct3A quesupdater3;
    [SerializeField] GameObject light;
    [SerializeField] GameObject mainGameEvent;
    bool state;
    private int quest = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void LoadMiniGame(string miniGameName) {
        //Time.timeScale = 0f;
        if (SceneManager.GetSceneByName(miniGameName).isLoaded) {
            Debug.LogWarning($"Attempted to load scene '{miniGameName}', but it is already loaded.");
            return;
        }
        StartCoroutine(LoadMiniGameRoutine(miniGameName));
        //if (mainGameEvent!=null)
        //mainGameEvent.SetActive(false);
        //if (light != null) {
        //    state = light.activeInHierarchy;
        //    light.SetActive(false);
        //}
        //Scene miniScene = SceneManager.GetSceneByName(miniGameName);
        //maincamera.gameObject.SetActive(false);
        //player.SetActive(false);
        //SceneManager.LoadScene(miniGameName,LoadSceneMode.Additive);
        //if (quesupdate == null) {
        //    if (questupdater == null) {
        //        quesupdater3.disablePointerb();
        //        quest = 3;
        //    } else {
        //        questupdater.disablePointer();
        //        quest = 1;
        //    }
        //} else {
        //    quesupdate.disablePointerb();
        //    quest = 2;
        //}
    }
    private IEnumerator LoadMiniGameRoutine(string miniGameName) {
        // 1. Disable main scene objects first. This happens instantly.
        if (mainGameEvent != null)
            mainGameEvent.SetActive(false);
        if (light != null) {
            state = light.activeInHierarchy;
            light.SetActive(false);
        }
        maincamera.gameObject.SetActive(false);
        player.SetActive(false);

        // 2. Begin loading the minigame scene in the background.
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(miniGameName, LoadSceneMode.Additive);

        // 3. Wait here until the scene is fully loaded. This prevents any further code
        // from running until the new scene is ready.
        yield return new WaitUntil(() => asyncLoad.isDone);

        // 4. Now that the new scene is loaded, handle the quest logic.
        if (quesupdate == null) {
            if (questupdater == null) {
                quesupdater3.disablePointerb();
                quest = 3;
            } else {
                questupdater.disablePointer();
                quest = 1;
            }
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
        if (quest == 1) questupdater.EnablePointer();
        else if (quest == 3) quesupdater3.EnablePointerb();
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
    public void ReloadMiniGame(string miniGameName) {
        StartCoroutine(ReloadMiniGameRoutine(miniGameName));
    }

    private IEnumerator ReloadMiniGameRoutine(string miniGameName) {
        // 1. Unload the current instance of the mini-game scene.
        if (SceneManager.GetSceneByName(miniGameName).isLoaded) {
            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(miniGameName);
            yield return new WaitUntil(() => asyncUnload.isDone);
        }

        // 2. Load a fresh instance of the mini-game scene.
        // We don't need to disable the player/camera again because they are already disabled.
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(miniGameName, LoadSceneMode.Additive);
        yield return new WaitUntil(() => asyncLoad.isDone);

        Debug.Log($"Scene '{miniGameName}' has been reloaded.");
    }
}