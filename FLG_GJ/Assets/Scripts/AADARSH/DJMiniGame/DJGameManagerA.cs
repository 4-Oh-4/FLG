using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class SimpleColorGame : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI wordText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public Button[] colorButtons;
    public GameObject gamePanel; // The entire UI panel for the game

    [Header("Game Settings")]
    public int targetScore = 1500;
    public float timeLimit = 60f; // 60 seconds

    // --- Private Game State Variables ---
    private string[] colorNames = { "RED", "BLUE", "GREEN" };
    private Color[] colors = { Color.red, Color.blue, Color.green };

    private int correctIndex;
    private int currentScore = 0;
    private float currentTime;

    void Start()
    {
        // Setup button listeners once
        for (int i = 0; i < colorButtons.Length; i++)
        {
            int buttonIndex = i;
            colorButtons[i].onClick.AddListener(() => OnColorButtonClick(buttonIndex));
        }

        StartGame();
    }

    public void StartGame()
    {
        currentScore = 0;
        currentTime = timeLimit;
        gamePanel.SetActive(true);
        UpdateScoreText();
        SetNewRound();
    }

    void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            timerText.text = "Time: " + Mathf.CeilToInt(currentTime);

            if (currentTime <= 0)
            {
                EndGame(false); // Player lost
            }
        }
    }

    void SetNewRound()
    {
        // The index of the correct word and button
        correctIndex = Random.Range(0, colorNames.Length);

        // The index of the color the word will be displayed in
        int displayColorIndex = Random.Range(0, colors.Length);

        // To make it tricky, ensure the display color isn't the same as the word
        if (displayColorIndex == correctIndex)
        {
            displayColorIndex = (correctIndex + 1) % colors.Length;
        }

        wordText.text = colorNames[correctIndex];
        wordText.color = colors[displayColorIndex];
    }

    void OnColorButtonClick(int chosenIndex)
    {
        if (chosenIndex == correctIndex)
        {
            // Correct Answer
            currentScore += 100;
        }
        else
        {
            // Incorrect Answer
            currentScore -= 50;
            if (currentScore < 0) currentScore = 0; // Prevent score from going below zero
        }

        UpdateScoreText();

        if (currentScore >= targetScore)
        {
            EndGame(true); // Player won
        }
        else
        {
            SetNewRound(); // Present the next challenge
        }
    }

    void UpdateScoreText()
    {
        scoreText.text = $"Score: {currentScore} / {targetScore}";
    }

    void EndGame(bool didWin)
    {
        currentTime = 0; // Stop the timer
        wordText.text = didWin ? "YOU WIN!" : "TIME'S UP!";

        // Here you would add logic to close the overlay and notify the main game
        // For now, we'll just disable the buttons
        foreach (var button in colorButtons)
        {
            button.interactable = false;
        }

        // Example: Hide the overlay after 2 seconds
        // StartCoroutine(HideOverlayAfterDelay(2f));
    }
}