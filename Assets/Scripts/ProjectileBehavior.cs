using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    private Vector2 direction;
    private float speed = 5f; // Reduced from 8f to make it slower and more visible
    private float maxDistance = 15f;
    private Vector2 startPosition;
    private CircleCollider2D projectileCollider;
    private SpriteRenderer spriteRenderer;
    private bool isDestroyed = false;
    private bool isInitialized = false;

    // Manual transformation variables
    private Vector2 currentPosition;
    private float currentRotation = 0f;

    void Awake()
    {
        Debug.Log("[PROJECTILE] Awake called");

        // Get or add SpriteRenderer
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            Debug.Log("[PROJECTILE] Added SpriteRenderer");
        }

        // Make sure sprite renderer is enabled
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
            Debug.Log($"[PROJECTILE] SpriteRenderer enabled, has sprite: {spriteRenderer.sprite != null}");
        }

        // Add collider for collision detection
        projectileCollider = GetComponent<CircleCollider2D>();
        if (projectileCollider == null)
        {
            projectileCollider = gameObject.AddComponent<CircleCollider2D>();
            projectileCollider.radius = 0.3f;
            projectileCollider.isTrigger = true;
            Debug.Log("[PROJECTILE] Added CircleCollider2D");
        }

        Debug.Log("[PROJECTILE] Awake complete - NO CustomTransform/Renderer to avoid freeze");
    }

    public void Initialize(Vector2 dir, Vector2 spawnPos, Color color)
    {
        Debug.Log($"[PROJECTILE] Initialize called - Position: {spawnPos}, Direction: {dir}");

        direction = dir.normalized;
        startPosition = spawnPos;
        currentPosition = spawnPos;
        currentRotation = 0f;

        // MANUAL TRANSFORMATION: Set initial position
        transform.position = new Vector3(currentPosition.x, currentPosition.y, 0);

        // Set color and make sure sprite is visible
        if (spriteRenderer != null)
        {
            spriteRenderer.color = color;
            spriteRenderer.enabled = true;

            // If no sprite assigned, create a default square
            if (spriteRenderer.sprite == null)
            {
                Debug.LogWarning("[PROJECTILE] No sprite assigned! Creating default sprite.");
                // Create a simple white square texture
                Texture2D tex = new Texture2D(32, 32);
                Color[] pixels = new Color[32 * 32];
                for (int i = 0; i < pixels.Length; i++)
                    pixels[i] = Color.white;
                tex.SetPixels(pixels);
                tex.Apply();

                Sprite defaultSprite = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32);
                spriteRenderer.sprite = defaultSprite;
            }

            Debug.Log($"[PROJECTILE] Color set to {color}, sprite: {spriteRenderer.sprite.name}");
        }
        else
        {
            Debug.LogError("[PROJECTILE] SpriteRenderer is null!");
        }

        isInitialized = true;
        Debug.Log($"[PROJECTILE] Initialized successfully at {spawnPos}");
    }

    void Update()
    {
        if (isDestroyed)
        {
            return;
        }

        if (!isInitialized)
        {
            Debug.LogWarning("[PROJECTILE] Update called but not initialized yet!");
            return;
        }

        // Debug every 10 frames
        if (Time.frameCount % 10 == 0)
        {
            Debug.Log($"[PROJECTILE] Moving: pos={currentPosition}, dir={direction}, speed={speed}");
        }

        // MANUAL TRANSFORMATION: Calculate new position manually
        float deltaX = direction.x * speed * Time.deltaTime;
        float deltaY = direction.y * speed * Time.deltaTime;

        currentPosition.x += deltaX;
        currentPosition.y += deltaY;

        // MANUAL TRANSFORMATION: Apply position to transform
        Vector3 newPosition = transform.position;
        newPosition.x = currentPosition.x;
        newPosition.y = currentPosition.y;
        transform.position = newPosition;

        // MANUAL TRANSFORMATION: Rotate manually
        currentRotation += 200f * Time.deltaTime; // Reduced from 300f for slower rotation
        if (currentRotation > 360f)
            currentRotation -= 360f;

        // MANUAL TRANSFORMATION: Apply rotation to transform
        Vector3 currentEuler = transform.eulerAngles;
        currentEuler.z = currentRotation;
        transform.eulerAngles = currentEuler;

        // Manual collision detection with monsters
        CheckMonsterCollision();

        // MANUAL CALCULATION: Check distance traveled
        float distanceX = currentPosition.x - startPosition.x;
        float distanceY = currentPosition.y - startPosition.y;
        float distanceTraveled = Mathf.Sqrt(distanceX * distanceX + distanceY * distanceY);

        if (distanceTraveled > maxDistance)
        {
            Debug.Log("[PROJECTILE] Out of range, destroying...");
            Destroy(gameObject);
        }
    }

    void CheckMonsterCollision()
    {
        if (projectileCollider == null) return;

        Bounds projectileBounds = projectileCollider.bounds;

        // Find all monsters
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");

        if (monsters.Length > 0 && Time.frameCount % 30 == 0)
        {
            Debug.Log($"[PROJECTILE] Checking collision with {monsters.Length} monsters");
        }

        foreach (GameObject monster in monsters)
        {
            if (monster == null) continue;

            Collider2D monsterCollider = monster.GetComponent<Collider2D>();
            if (monsterCollider == null || !monsterCollider.enabled) continue;

            // MANUAL COLLISION CHECK: Bounds intersection
            if (projectileBounds.Intersects(monsterCollider.bounds))
            {
                Debug.Log($"[PROJECTILE] ►►► HIT DETECTED! Monster: {monster.name} at {monster.transform.position}");

                // Call the monster's OnHit method
                MonsterBehavior monsterBehavior = monster.GetComponent<MonsterBehavior>();
                if (monsterBehavior != null)
                {
                    Debug.Log("[PROJECTILE] Calling monster.OnHit()");
                    monsterBehavior.OnHit();
                }
                else
                {
                    Debug.LogWarning("[PROJECTILE] Monster has no MonsterBehavior!");
                }

                // Destroy this projectile with small delay so you can see the impact
                Debug.Log("[PROJECTILE] Destroying projectile after brief delay");
                isDestroyed = true;
                Destroy(gameObject, 0.1f); // 0.1 second delay
                return;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isDestroyed) return;

        Debug.Log($"[PROJECTILE] Trigger with: {other.gameObject.name} (Tag: {other.tag})");

        // Handle collectibles
        if (other.CompareTag("Collectible"))
        {
            Debug.Log("[PROJECTILE] Hit collectible!");
            Destroy(other.gameObject);
            isDestroyed = true;
            Destroy(gameObject);
        }

        // Handle monsters
        else if (other.CompareTag("Monster"))
        {
            Debug.Log("[PROJECTILE] Hit monster (trigger)!");
            MonsterBehavior monsterBehavior = other.GetComponent<MonsterBehavior>();
            if (monsterBehavior != null)
            {
                monsterBehavior.OnHit();
            }

            isDestroyed = true;
            Destroy(gameObject);
        }
    }
}