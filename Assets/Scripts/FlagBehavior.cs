using UnityEngine;
using UnityEngine.SceneManagement;

public class FlagBehavior : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private float waveTime = 0f;
    private bool levelCompleted = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        Debug.Log("[FLAG] Flag initialized at position: " + transform.position);

        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col != null)
        {
            Debug.Log($"[FLAG] Collider found - isTrigger: {col.isTrigger}, bounds: {col.bounds}");
        }
        else
        {
            Debug.LogError("[FLAG] NO COLLIDER FOUND!");
        }
    }

    void Update()
    {
        // Check if player is nearby every second
        if (Time.frameCount % 60 == 0)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                float distance = Vector2.Distance(transform.position, player.transform.position);
                Debug.Log($"[FLAG] Player distance: {distance:F2}");
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[FLAG] Triggered by: {other.gameObject.name}, Tag: {other.tag}");

        if (other.CompareTag("Player") && !levelCompleted)
        {
            Debug.Log("[FLAG] Player detected!");

            if (CoinManager.Instance != null)
            {
                Debug.Log($"[FLAG] Coins: {CoinManager.Instance.coinCount}/{CoinManager.Instance.totalCoins}");
                Debug.Log($"[FLAG] All collected? {CoinManager.Instance.AllCoinsCollected()}");
            }
            else
            {
                Debug.LogError("[FLAG] CoinManager.Instance is NULL!");
            }

            // Check if all coins are collected
            if (CoinManager.Instance != null && CoinManager.Instance.AllCoinsCollected())
            {
                Debug.Log("LEVEL COMPLETE! All coins collected!");
                levelCompleted = true;
                CompleteLevel();
            }
            else
            {
                Debug.Log("Collect all coins first!");
                if (CoinManager.Instance != null)
                {
                    int remaining = CoinManager.Instance.totalCoins - CoinManager.Instance.coinCount;
                    Debug.Log($"You need {remaining} more coins!");
                }
            }
        }
    }

    void CompleteLevel()
    {
        // Stop player movement
        PlayerController player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>();
        if (player != null)
        {
            player.enabled = false;
        }

        // Show victory UI
        if (GameUIManager.Instance != null)
        {
            GameUIManager.Instance.ShowVictoryScreen();
        }
    }
}