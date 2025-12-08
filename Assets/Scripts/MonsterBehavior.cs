using UnityEngine;
using System.Collections;

public class MonsterBehavior : MonoBehaviour
{
    private CustomTransform customTransform;
    private CustomRenderer customRenderer;
    private Transform playerTransform;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float detectionRange = 15f;

    [Header("Visual")]
    public Color monsterColor = Color.red;

    private bool isDestroyed = false;
    private Vector2 currentPosition;

    void Awake()
    {
        customTransform = GetComponent<CustomTransform>();
        if (customTransform == null)
            customTransform = gameObject.AddComponent<CustomTransform>();

        customRenderer = GetComponent<CustomRenderer>();
        if (customRenderer == null)
            customRenderer = gameObject.AddComponent<CustomRenderer>();

        // Add collider for projectile detection
        CircleCollider2D collider = GetComponent<CircleCollider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<CircleCollider2D>();
            collider.radius = 0.5f;
            collider.isTrigger = true;
        }

        // Set tag
        if (!gameObject.CompareTag("Monster"))
        {
            gameObject.tag = "Monster";
        }
    }

    void Start()
    {
        currentPosition = transform.position;
        customTransform.localPosition = currentPosition;
        customRenderer.SetColor(monsterColor);

        // Find player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }

        Debug.Log($"[MONSTER] Spawned at {currentPosition}");
    }

    void Update()
    {
        if (isDestroyed) return;

        if (playerTransform == null)
        {
            // Try to find player again
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }
            return;
        }

        // Calculate distance to player
        float distanceToPlayer = Vector2.Distance(currentPosition, playerTransform.position);

        // Chase player if within detection range
        if (distanceToPlayer < detectionRange)
        {
            // Calculate direction to player
            Vector2 direction = ((Vector2)playerTransform.position - currentPosition).normalized;

            // Move towards player
            Vector2 movement = direction * moveSpeed * Time.deltaTime;
            currentPosition += movement;

            // Update transform manually
            customTransform.localPosition = currentPosition;
            transform.position = currentPosition;
            customTransform.UpdateMatrix();

            // Rotate to face player
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            customTransform.localRotation = angle;
            transform.eulerAngles = new Vector3(0, 0, angle);
        }

        // Idle rotation when not chasing
        else
        {
            customTransform.Rotate(50f * Time.deltaTime);
        }
    }

    // Called when hit by projectile
    public void OnHit()
    {
        if (isDestroyed)
        {
            Debug.Log("[MONSTER] Already destroyed, ignoring...");
            return;
        }

        Debug.Log($"[MONSTER] ✗✗✗ {gameObject.name} HIT BY PROJECTILE! ✗✗✗");
        isDestroyed = true;

        // Disable collider to prevent double hits
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
            Debug.Log("[MONSTER] Collider disabled");
        }

        // Stop chasing
        enabled = false;

        // Start destruction animation
        StartCoroutine(ShrinkAndDestroy());
    }

    IEnumerator ShrinkAndDestroy()
    {
        Debug.Log("[MONSTER] ►►► Starting shrink animation...");

        float shrinkDuration = 0.3f;
        float elapsed = 0f;

        Vector3 initialScale = transform.localScale;
        Vector3 targetScale = Vector3.zero;

        while (elapsed < shrinkDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / shrinkDuration;

            // Scale down to zero
            Vector3 newScale;
            newScale.x = initialScale.x + (targetScale.x - initialScale.x) * t;
            newScale.y = initialScale.y + (targetScale.y - initialScale.y) * t;
            newScale.z = initialScale.z + (targetScale.z - initialScale.z) * t;
            transform.localScale = newScale;

            // Fade out alpha
            if (customRenderer != null)
            {
                Color currentColor = customRenderer.color;
                currentColor.a = 1f - t;
                customRenderer.SetColor(currentColor);
            }

            // Spin faster while shrinking
            Vector3 currentRotation = transform.eulerAngles;
            currentRotation.z += 360f * Time.deltaTime;
            transform.eulerAngles = currentRotation;

            yield return null;
        }

        Debug.Log("[MONSTER] ►►► Destroyed!");
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Handle collision with player
        if (other.CompareTag("Player"))
        {
            Debug.Log("[MONSTER] TOUCHED PLAYER!");

            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.Die();
            }
        }
    }
}