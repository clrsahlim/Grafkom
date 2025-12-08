using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance;
    public static bool GameEnded { get; private set; } = false;

    [Header("Victory UI")]
    public GameObject victoryPanel;
    public TextMeshProUGUI victoryText;
    public Button replayButton;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverText;
    public Button restartButton;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            GameEnded = false;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (replayButton != null)
        {
            replayButton.onClick.AddListener(ReplayGame);
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(ReplayGame);
        }
    }

    public void ShowVictoryScreen()
    {
        if (victoryPanel != null)
        {
            GameEnded = true;
            victoryPanel.SetActive(true);
            Debug.Log("[UI] Victory screen shown!");

            // Pause the game
            Time.timeScale = 0f;
        }
    }

    public void ShowGameOver()
    {
        Debug.Log("[UI] ShowGameOver() called!");

        if (gameOverPanel != null)
        {
            GameEnded = true;
            Debug.Log("[UI] Game Over panel found, activating...");
            gameOverPanel.SetActive(true);
            Debug.Log("[UI] ☠ Game Over screen shown! ☠");

            // Pause the game
            Time.timeScale = 0f;
        }
        else
        {
            Debug.LogError("[UI] ❌ Game Over panel is NULL! Not assigned in Inspector!");
            Debug.LogWarning("[UI] Auto-restarting in 3 seconds...");
            Invoke("ReplayGame", 3f);
        }
    }

    public void ReplayGame()
    {
        Debug.Log("[UI] Restarting game...");
        GameEnded = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}