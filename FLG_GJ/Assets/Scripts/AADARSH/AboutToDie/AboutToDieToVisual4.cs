using System.Collections;
using UnityEngine;

public class AboutToDieToVisual4 : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Start the coroutine to call the function after a delay
        StartCoroutine(CallFunctionAfterDelay());
    }

    // This is the coroutine that will handle the time delay
    private IEnumerator CallFunctionAfterDelay()
    {
        // Wait for 20 seconds
        yield return new WaitForSeconds(20f);

        // Call the function you want to execute after the delay
        Debug.Log("20 seconds have passed since the object was spawned. The function has been called.");
        FindAnyObjectByType<ResetterAct2Home>().ResetGame();
    }

    // This is the function that will be executed after the delay
    private void MyFunctionToCall()
    {
        // Add the code you want to execute here
        Debug.Log("This is the custom function that was called.");
        // Example: Destroy the object, trigger an event, etc.
    }
}