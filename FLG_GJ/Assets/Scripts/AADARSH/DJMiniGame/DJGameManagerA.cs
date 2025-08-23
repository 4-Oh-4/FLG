using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DJGameManagerA : MonoBehaviour {
    [Header("UI")]
    public Button[] colorButtons;
    public TextMeshProUGUI wordText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;

    [Header("Game Settings")]
    public AudioSource musicSource;
    public int bpm = 120;

    private string[] colorNames = { "RED", "GREEN", "BLUE", "YELLOW", "PURPLE" };
    private Color[] colors = { Color.red, Color.green, Color.blue, Color.yellow, new Color(0.5f, 0, 0.5f) };

    private int correctIndex;
    private int score = 0;
    private int combo = 0;

    private double nextBeatTime;
    private double beatInterval;

    void Start() {
        beatInterval = 60.0 / bpm;
        nextBeatTime = AudioSettings.dspTime + 2.0; // start after 2 sec delay
        musicSource.Play();
    }

    void Update() {
        // Spawn word on beat
        if (AudioSettings.dspTime >= nextBeatTime) {
            SetNewRound();
            nextBeatTime += beatInterval;
        }
    }

    void SetNewRound() {
        correctIndex = Random.Range(0, 3); // Start with 3 colors, expand later
        wordText.text = colorNames[correctIndex];
        wordText.color = colors[Random.Range(0, colors.Length)];

        // Assign button listeners
        for (int i = 0; i < colorButtons.Length; i++) {
            int index = i;
            colorButtons[i].GetComponent<Image>().color = colors[i];
            colorButtons[i].onClick.RemoveAllListeners();
            colorButtons[i].onClick.AddListener(() => CheckAnswer(index));
        }
    }

    void CheckAnswer(int chosenIndex) {
        double currentTime = AudioSettings.dspTime;
        double timeDiff = Mathf.Abs((float)(currentTime - nextBeatTime + beatInterval));

        if (chosenIndex == correctIndex) {
            if (timeDiff < 0.2f) // near beat = perfect
            {
                score += 100 * (combo + 1);
                combo++;
            } else // slightly off
              {
                score += 50;
                combo = 0;
            }
        } else {
            score -= 50;
            combo = 0;
        }

        // Update UI
        scoreText.text = "Score: " + score;
        comboText.text = "Combo: " + combo + "x";
    }
}
