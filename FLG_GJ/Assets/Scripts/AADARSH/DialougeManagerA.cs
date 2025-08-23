using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DialougeManagerA : MonoBehaviour {
    public Queue<string> sentences;
    public TextMeshProUGUI text;
    public TextMeshProUGUI nameText;  // renamed to avoid shadowing MonoBehaviour.name
    public Animator animator;

    private bool isTyping = false;
    private string currentSentence = "";
    private Coroutine typingCoroutine;

    void Start() {
        sentences = new Queue<string>();
    }

    public void StartDialouge(DialougeA dialouge) {
        Debug.Log("Starting " + dialouge.name);
        animator.SetBool("isOpen", true);
        nameText.text = dialouge.name;

        sentences.Clear();
        foreach (string sentence in dialouge.sentences) {
            sentences.Enqueue(sentence);
        }

        DisplayNextDirect(); // show the first line
    }

    // Called internally (no input context)
    public void DisplayNextDirect() {
        if (sentences.Count == 0) {
            EndDialouge();
            return;
        }

        currentSentence = sentences.Dequeue();

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeSentence(currentSentence));
    }

    // Called by input system
    public void DisplayNext(InputAction.CallbackContext context) {
        if (!context.performed) return;

        if (isTyping) {
            // If still typing  instantly complete sentence
            StopCoroutine(typingCoroutine);
            text.text = currentSentence;
            isTyping = false;
            return;
        }

        // Otherwise, go to next sentence
        if (sentences.Count == 0) {
            EndDialouge();
            return;
        }

        currentSentence = sentences.Dequeue();
        typingCoroutine = StartCoroutine(TypeSentence(currentSentence));
    }

    void EndDialouge() {
        Debug.Log("Convo Ended");
        animator.SetBool("isOpen", false);
    }

    IEnumerator TypeSentence(string sentence) {
        isTyping = true;
        text.text = "";

        foreach (char letter in sentence.ToCharArray()) {
            text.text += letter;
            yield return new WaitForSeconds(0.02f);
        }

        isTyping = false;
    }
}
