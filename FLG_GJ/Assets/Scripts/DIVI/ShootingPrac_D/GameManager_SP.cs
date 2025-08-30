using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager_SP : MonoBehaviour
{
    public static GameManager_SP Instance { get; private set; }

    [Header("Round Config (3 items)")]
    public RoundSettings[] rounds = new RoundSettings[3];

    [Header("Refs")]
    [SerializeField] private CanSpawner_D spawner;
    [SerializeField] private TextMeshProUGUI roundText;
    [SerializeField] private TextMeshProUGUI remainingCansText;
    [SerializeField] private UIManager_SP uiManager;

    private int currentRoundIndex;
    private int lives;
    private float roundTimer;
    private bool gameOver;
    private int remainingCans; 
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

        if (roundCo != null) StopCoroutine(roundCo);
        roundCo = StartCoroutine(RunRounds());
    }

    private IEnumerator RunRounds()
    {
        for (int i = 0; i < rounds.Length; i++)
        {
            currentRoundIndex = i;
            lives = 3; // reset lives each round
            

            // Start round
            var rs = rounds[i];
            roundTimer = rs.duration;
            remainingCans = rs.totalCans;

            spawner.Begin(rs);
            UpdateUI();

            while (roundTimer > 0f && !gameOver && remainingCans > 0)
            {
                roundTimer -= Time.deltaTime;
                yield return null;
            }

            spawner.End();
            if (gameOver) yield break;

            Debug.Log($"Round {i + 1} finished!");
            yield return new WaitForSeconds(3f);
        }

        
        Debug.Log("All rounds completed!");
    }


    private void UpdateUI()
    {
        roundText.text = $"Round: {currentRoundIndex + 1}";
        remainingCansText.text = $"Cans: {remainingCans}";

        uiManager.UpdateHearts(lives, 3);
    }

    private void Lose()
    {
        gameOver = true;
        Debug.Log("Game Over!");
    }

   
    public void OnCanMissed()
    {
        if (gameOver) return;
        lives--;
        remainingCans--;
        UpdateUI();

        if (lives <= 0)
        {
            spawner.End();
            Lose();
        }
    }

   
    public void OnCanShot()
    {
        if (gameOver) return;

        remainingCans--; 
        UpdateUI();
    }
}
