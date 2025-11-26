using UnityEngine;
using UnityEngine.UI;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;
    public int coinCount = 0;
    public Text coinText; 

    void Awake()
    {
        Instance = this;
    }

    public void AddCoin()
    {
        coinCount++;
        if (coinText != null)
            coinText.text = "Coins: " + coinCount;
    }
}