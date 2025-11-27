using UnityEngine;
using System.Collections;

public class CollectibleBehavior : MonoBehaviour
{
    private CustomTransform customTransform;
    private CustomRenderer customRenderer;
    private Vector2 startPosition;
    private float bobSpeed = 2f;
    private float bobAmount = 0.3f;

    void Awake()
    {
        customTransform = GetComponent<CustomTransform>();
        if (customTransform == null)
            customTransform = gameObject.AddComponent<CustomTransform>();

        customRenderer = GetComponent<CustomRenderer>();
        if (customRenderer == null)
            customRenderer = gameObject.AddComponent<CustomRenderer>();
    }

    void Start()
    {
        startPosition = transform.position;
        customTransform.localPosition = startPosition;
    }

    void Update()
    {
        // Bob up and down
        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobAmount;
        customTransform.localPosition = new Vector2(startPosition.x, newY);
        customTransform.UpdateMatrix();

        // Spin
        customTransform.Rotate(90f * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Check if CoinManager exists before using it
            if (CoinManager.Instance != null)
            {
                CoinManager.Instance.AddCoin();
            }
            else
            {
                Debug.LogWarning("CoinManager.Instance is null!");
            }

            // Start the collection animation instead of destroying immediately
            StartCoroutine(CollectAnimation());
        }
    }

    IEnumerator CollectAnimation()
    {
        float duration = 0.3f;
        float elapsed = 0f;
        Vector2 startScale = customTransform.localScale;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Fade out
            if (customRenderer != null)
            {
                Color col = customRenderer.color;
                col.a = Mathf.Lerp(1f, 0f, t);
                customRenderer.SetColor(col);
            }

            // Scale up
            if (customTransform != null)
            {
                customTransform.localScale = startScale * (1f + t);
                customTransform.UpdateMatrix();
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}