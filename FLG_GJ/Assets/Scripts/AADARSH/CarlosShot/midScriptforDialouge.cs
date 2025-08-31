using System.Collections;
using UnityEngine;

public class midScriptforDialouge : MonoBehaviour
{
    [SerializeField]private float seconds = 3f;
    [SerializeField] CameraPanController cameraPan;
    [SerializeField] Animator target1;
    [SerializeField] Animator target2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Wait());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private IEnumerator Wait() {
        yield return new WaitForSeconds(seconds);
        target1.SetBool("isShooting", false);
        target2.SetBool("GotShoot", false);
        cameraPan.enabled = false;
        StoryManagertAct1A.Instance.SetFlag("StartTalk",true);

    }
}
