using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;

    [Header("Coin Tracking")]
    public int coinCount = 0;
    public int totalCoins = 0;

    [Header("UI")]
    public TextMeshProUGUI coinText;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Delay counting to ensure all coins have initialized
        Invoke("CountTotalCoins", 0.1f);
        Invoke("UpdateUI", 0.15f);
    }

    void CountTotalCoins()
    {
        GameObject[] coins = GameObject.FindGameObjectsWithTag("Collectible");
        totalCoins = coins.Length;
        Debug.Log($"[COIN MANAGER] Total coins in scene: {totalCoins}");

        // If no coins found, check if tag is correct
        if (totalCoins == 0)
        {
            Debug.LogWarning("[COIN MANAGER] No coins found! Make sure coins have 'Collectible' tag!");
        }
    }

    public void AddCoin()
    {
        coinCount++;
        Debug.Log($"[COIN MANAGER] Coin collected! {coinCount}/{totalCoins}");
        UpdateUI();

        // Check if all coins collected (and there are coins to collect)
        if (coinCount == totalCoins && totalCoins > 0)
        {
            Debug.Log("[COIN MANAGER] ALL COINS COLLECTED!");
            OnAllCoinsCollected();
        }
    }

    void UpdateUI()
    {
        if (coinText != null)
        {
            coinText.text = $"Coins: {coinCount}/{totalCoins}";
            Debug.Log($"[COIN MANAGER] UI Updated: {coinText.text}");
        }
        else
        {
            Debug.LogError("[COIN MANAGER] coinText is NOT assigned in Inspector!");
        }
    }

    void OnAllCoinsCollected()
    {
        if (coinText != null)
        {
            coinText.color = Color.green;
        }
    }

    public bool AllCoinsCollected()
    {
        return coinCount >= totalCoins && totalCoins > 0;
    }
}