using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager_SP : MonoBehaviour
{
    public static GameManager_SP Instance { get; private set; }

    [Header("Round Config (3 items)")]
    public RoundSettings[] rounds = new RoundSettings[3];

    [Header("Refs")]
    [SerializeField] private CanSpawner_D spawner;
    [SerializeField] private Text livesText;
    [SerializeField] private Text roundText;
    [SerializeField] private Text timerText;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private Image background; // optional: set a sprite here

    private int currentRoundIndex;
    private int lives;
    private float roundTimer;
    private bool gameOver;
    private Coroutine roundCo;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        gameOver = false;
        currentRoundIndex = 0;
        winPanel?.SetActive(false);
        losePanel?.SetActive(false);

        if (roundCo != null) StopCoroutine(roundCo);
        roundCo = StartCoroutine(RunRounds());
    }

    private IEnumerator RunRounds()
    {
        for (int i = 0; i < rounds.Length; i++)
        {
            currentRoundIndex = i;
            lives = 3; // reset lives each round
            UpdateUI();

            // Start round
            var rs = rounds[i];
            roundTimer = rs.duration;
            spawner.Begin(rs);

            while (roundTimer > 0f && !gameOver)
            {
                roundTimer -= Time.deltaTime;
                UpdateUI();
                yield return null;
            }

            spawner.End();
            if (gameOver) yield break;

            // Small pause between rounds
            yield return new WaitForSeconds(1f);
        }

        Win();
    }

    private void UpdateUI()
    {
        if (livesText) livesText.text = $"Lives: {lives}";
        if (roundText) roundText.text = $"Round: {currentRoundIndex + 1}/3";
        if (timerText) timerText.text = $"Time: {Mathf.CeilToInt(Mathf.Max(0f, roundTimer))}s";
    }

    private void Win()
    {
        gameOver = true;
        winPanel?.SetActive(true);
    }

    private void Lose()
    {
        gameOver = true;
        losePanel?.SetActive(true);
    }

    // Called by MissZone when a can falls
    public void OnCanMissed()
    {
        if (gameOver) return;
        lives--;
        UpdateUI();
        if (lives <= 0)
        {
            spawner.End();
            Lose();
        }
    }

    // Optional score hook
    public void OnCanShot() { /* add score if you want */ }
}
