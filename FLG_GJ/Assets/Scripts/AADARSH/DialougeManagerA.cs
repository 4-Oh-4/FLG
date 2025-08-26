using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using System; // <-- ADDED: Required for using Action

public class DialougeManagerA : MonoBehaviour {
    // Handles sentences inside a single dialogue
    private Queue<string> sentences;

    // Handles multiple dialogues in sequence
    private Queue<DialougeA> dialogueQueue = new Queue<DialougeA>();
    private DialougeA currentDialogue;

    // --- ADDED ---
    // This will store the function to call when the entire sequence is complete.
    private Action onSequenceComplete;
    // -------------

    [Header("UI")]
    public TextMeshProUGUI text;
    public TextMeshProUGUI nameText;
    public Animator animator;

    private bool isTyping = false;
    private string currentSentence = "";
    private Coroutine typingCoroutine;

    void Start() {
        sentences = new Queue<string>();
    }

    public void StartDialouge(DialougeA dialouge) {
        animator.SetBool("isOpen", true); // Moved from StartDialogueSequence for better control
        nameText.text = dialouge.name;

        sentences.Clear();
        // Assuming your DialougeA class has a string array called 'sentences'
        foreach (string sentence in dialouge.sentences) {
            sentences.Enqueue(sentence);
        }

        DisplayNextDirect();
    }

    // --- MODIFIED METHOD ---
    // It now accepts an optional Action to be called on completion.
    public void StartDialogueSequence(DialougeA[] dialogues, Action onCompleteCallback = null) {
        dialogueQueue.Clear();

        // --- ADDED ---
        // Store the callback function for later.
        this.onSequenceComplete = onCompleteCallback;
        // -------------

        foreach (var d in dialogues) {
            dialogueQueue.Enqueue(d);
        }

        DisplayNextDialogue();
    }

    private void DisplayNextDialogue() {
        if (dialogueQueue.Count == 0) {
            EndDialogueSequence();
            return;
        }

        currentDialogue = dialogueQueue.Dequeue();
        StartDialouge(currentDialogue);
    }

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

    public void DisplayNext(InputAction.CallbackContext context) {
        if (!context.performed || !animator.GetBool("isOpen")) return;

        if (isTyping) {
            StopCoroutine(typingCoroutine);
            text.text = currentSentence;
            isTyping = false;
            return;
        }

        if (sentences.Count == 0) {
            EndDialouge();
            return;
        }
        currentSentence = sentences.Dequeue();
        typingCoroutine = StartCoroutine(TypeSentence(currentSentence));
    }

    void EndDialouge() {
        // This is called when a single character's dialogue block is finished.
        if (dialogueQueue.Count > 0) {
            DisplayNextDialogue();
        } else {
            // --- IMPROVEMENT ---
            // If there are no more dialogues in the queue, call the main end sequence method.
            EndDialogueSequence();
        }
    }

    void EndDialogueSequence() {
        animator.SetBool("isOpen", false);
        Debug.Log("Dialogue sequence finished completely!");

        // --- ADDED: THE KEY LOGIC ---
        // If a callback was provided, invoke it now.
        // The '?.' is a null-check to prevent errors if no callback was given.
        onSequenceComplete?.Invoke();

        // It's good practice to clear the callback after it's been used.
        onSequenceComplete = null;
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