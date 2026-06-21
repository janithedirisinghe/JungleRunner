using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public Transform player;            // assign the Player in the Inspector

    public bool IsGameOver { get; private set; }
    public int Score { get; private set; }
    public int HighScore { get; private set; }

    private int coinScore = 0;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        HighScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    void Update()
    {
        if (IsGameOver || player == null) return;
        Score = Mathf.FloorToInt(player.position.z) + coinScore;  // distance + coins
    }

    public void AddScore(int amount) => coinScore += amount;

    public void GameOver()
    {
        if (IsGameOver) return;
        IsGameOver = true;
        Time.timeScale = 0f;

        if (Score > HighScore)
        {
            HighScore = Score;
            PlayerPrefs.SetInt("HighScore", HighScore);
        }
    }

    public void Restart()
    {
        Time.timeScale = 1f;            // MUST unfreeze before reloading
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}