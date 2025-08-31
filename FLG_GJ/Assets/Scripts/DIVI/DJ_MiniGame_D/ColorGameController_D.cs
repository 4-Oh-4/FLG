using UnityEngine;
using TMPro; // Required for TextMeshPro
using UnityEngine.UI; // Required for Buttons

public class ColorGameController_D : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI wordText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public Button[] colorButtons;
    public GameObject gamePanel;

    // --- NEW UI VARIABLES ---
    
    public GameObject menuPanel; // Reference to the menu panel
    public Button restartButton; // Reference to the restart button


    [Header("Game Settings")]
    public int targetScore = 1500;
    public float timeLimit = 60f;

    // --- Private Game State Variables ---
    private string[] colorNames = { "RED", "BLUE", "GREEN" };
    private Color[] colors = { Color.red, Color.blue, Color.green };

    private int correctIndex;
    private int currentScore = 0;
    private float currentTime;
    private bool isGameActive = false;

    // This function is called when the script starts
    void Start()
    {
        menuPanel.SetActive(true);
        gamePanel.SetActive(false);
    }

    // This function runs every single frame
    void Update()
    {
        // Only run the timer if the game is active
        if (isGameActive)
        {
            // The timer should count down as long as it's above zero
            if (currentTime > 0)
            {
                currentTime -= Time.deltaTime;
                timerText.text = "Time: " + Mathf.CeilToInt(currentTime);
            }
            else // This runs once the timer hits zero
            {
                EndGame(false); // Player lost because time ran out
            }
        }
    }

    // Sets up the game to begin
    public void StartGame()
    {

        // Hide the menu and show the game
        menuPanel.SetActive(false);
        gamePanel.SetActive(true);
        restartButton.gameObject.SetActive(false);


        currentScore = 0;
        currentTime = timeLimit;
        gamePanel.SetActive(true);
        isGameActive = true;

        // Make sure buttons are clickable at the start
        foreach (var button in colorButtons)
        {
            button.interactable = true;
        }

        UpdateScoreText();
        SetNewRound();
    }

    // Sets up the next word puzzle
    void SetNewRound()
    {
        correctIndex = Random.Range(0, colorNames.Length);

        int displayColorIndex;
        // Keep picking until it's different
        do
        {
            displayColorIndex = Random.Range(0, colors.Length);
        }
        while (displayColorIndex == correctIndex);

        wordText.text = colorNames[correctIndex];
        wordText.color = colors[displayColorIndex];
    }


    // This function is called when a color button is clicked
    public void OnColorButtonClick(int chosenIndex)
    {
        if (!isGameActive) return; // Don't do anything if game is over

        if (chosenIndex == correctIndex)
        {
            currentScore += 100;
        }
        else
        {
            currentScore -= 50;
            if (currentScore < 0) currentScore = 0;
        }

        UpdateScoreText();

        if (currentScore >= targetScore)
        {
            EndGame(true); // Player won by reaching the score
        }
        else
        {
            SetNewRound(); // Present the next challenge
        }
    }

    // Updates the score display text
    void UpdateScoreText()
    {
        scoreText.text = $"Score: {currentScore} / {targetScore}";
    }

    // Ends the game
    void EndGame(bool didWin)
    {
        isGameActive = false;
        currentTime = 0; // Stop the timer
        wordText.text = didWin ? "YOU WIN!" : "TIME'S UP!";

        // Disable all the buttons so they can't be clicked
        foreach (var button in colorButtons)
        {
            button.interactable = false;
        }

        if (didWin)
        {
            // --- THIS IS YOUR WINNING CONDITION ---
            wordText.text = "YOU WIN!";

            // Here, you can tell your main game manager that the minigame was won.
            // For example: GameManager.Instance.UpdateQuestProgress();

            // For now, we'll just show the restart button.
            restartButton.gameObject.SetActive(true);
        }
        else
        {
            wordText.text = "TIME'S UP!";
            restartButton.gameObject.SetActive(true);
        }
    }
}