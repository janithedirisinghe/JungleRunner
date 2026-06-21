using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("In-game")]
    public TextMeshProUGUI scoreText;
    public GameObject pauseButton;      // small pause button shown while playing

    [Header("Start")]
    public GameObject startPanel;

    [Header("Pause")]
    public GameObject pausePanel;

    [Header("Game Over")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI highScoreText;
    private static bool skipStartScreen = false;

    private bool gameOverShown;

    void Start()
    {
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);

        if (skipStartScreen)
        {
            // came from a Restart — jump straight into play
            skipStartScreen = false;
            startPanel.SetActive(false);
            pauseButton.SetActive(true);
            Time.timeScale = 1f;
        }
        else
        {
            // fresh launch — show the Start screen, frozen
            startPanel.SetActive(true);
            pauseButton.SetActive(false);
            Time.timeScale = 0f;
        }
    }

    void Update()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        scoreText.text = gm.Score.ToString();

        if (gm.IsGameOver && !gameOverShown)
        {
            gameOverShown = true;
            gameOverPanel.SetActive(true);
            pauseButton.SetActive(false);
            finalScoreText.text = "Score: " + gm.Score;
            highScoreText.text  = "Best: " + gm.HighScore;
        }
    }

    public void OnPlayButton()       // Start screen → Play
    {
        startPanel.SetActive(false);
        pauseButton.SetActive(true);
        Time.timeScale = 1f;
    }

    public void OnPauseButton()      // Pause button during play
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void OnResumeButton()     // Pause screen → Resume
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OnRestartButton()
    {
        skipStartScreen = true;       // skip the Start screen after reload
        GameManager.Instance.Restart();
    }
}