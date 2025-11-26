using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CustomTransform customTransform;
    private CustomRenderer customRenderer;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 8f;

    [Header("Projectile")]
    public GameObject projectilePrefab;
    public float projectileSpawnDistance = 1f;
    public float shootCooldown = 0.3f;
    private float lastShootTime;

    private Vector2 velocity;
    private bool isGrounded;
    private Color playerColor = new Color(0.3f, 0.5f, 1f);

    void Awake()
    {
        // Add CustomTransform
        customTransform = GetComponent<CustomTransform>();
        if (customTransform == null)
            customTransform = gameObject.AddComponent<CustomTransform>();

        // Add CustomRenderer
        customRenderer = GetComponent<CustomRenderer>();
        if (customRenderer == null)
            customRenderer = gameObject.AddComponent<CustomRenderer>();

        // === FIX 1: Add BoxCollider2D ===
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(0.5f, 0.5f);
            collider.isTrigger = false; // CRITICAL: Must be solid, not trigger!
        }

        // === FIX 2: Add Rigidbody2D ===
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 0; // We handle gravity manually
            rb.freezeRotation = true; // Prevent spinning
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        // Ensure tag is set
        if (!gameObject.CompareTag("Player"))
            gameObject.tag = "Player";
    }

    void Start()
    {
        customRenderer.SetColor(playerColor);
    }

    void Update()
    {
        HandleMovementInput();
        HandleTransformInput();
        HandleShootInput();
        HandleRandomColor();
        ApplyPhysics();
    }

    void HandleMovementInput()
    {
        // WASD / Arrow keys
        float horizontal = Input.GetAxisRaw("Horizontal");
        if (horizontal != 0)
        {
            customTransform.Translate(new Vector2(horizontal * moveSpeed * Time.deltaTime, 0));
        }

        // Jump (only if grounded)
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = jumpForce;
            isGrounded = false;
            customRenderer.PulseBrightness(0.2f, 0.5f);
            Debug.Log("Player jumped!");
        }
    }

    void HandleTransformInput()
    {
        // Q/E - Scale
        if (Input.GetKey(KeyCode.Q))
        {
            customTransform.Scale(new Vector2(1.01f, 1.01f));
        }
        if (Input.GetKey(KeyCode.E))
        {
            customTransform.Scale(new Vector2(0.99f, 0.99f));
        }

        // Z/C - Rotate
        if (Input.GetKey(KeyCode.Z))
        {
            customTransform.Rotate(100f * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.C))
        {
            customTransform.Rotate(-100f * Time.deltaTime);
        }
    }

    void HandleShootInput()
    {
        bool shootPressed = Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(1);
        if (shootPressed && Time.time > lastShootTime + shootCooldown)
        {
            ShootProjectile();
            lastShootTime = Time.time;
        }
    }

    void HandleRandomColor()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Color newColor = Random.ColorHSV();
            customRenderer.SetColor(newColor);
            playerColor = newColor;
            Debug.Log("Color changed to: " + newColor);
        }
    }

    void ShootProjectile()
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning("No projectile prefab assigned!");
            return;
        }

        customRenderer.PulseBrightness(0.2f, 0.3f);

        // Calculate direction based on player rotation
        float angle = customTransform.localRotation * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        Vector2 spawnPosition = customTransform.localPosition + direction * projectileSpawnDistance;

        // Spawn projectile
        GameObject projectileObj = Instantiate(projectilePrefab);
        ProjectileBehavior projectile = projectileObj.GetComponent<ProjectileBehavior>();
        if (projectile != null)
            projectile.Initialize(direction, spawnPosition, playerColor);

        Debug.Log("Projectile fired!");
    }

    void ApplyPhysics()
    {
        if (customTransform == null) return;

        // Apply gravity (only if not grounded)
        if (!isGrounded)
        {
            velocity.y -= 20f * Time.deltaTime; // Gravity
        }
        else
        {
            velocity.y = 0; // Reset velocity when grounded
        }

        // Apply vertical movement
        customTransform.Translate(new Vector2(0, velocity.y * Time.deltaTime));

        // Simple ground check (fallback)
        if (customTransform.localPosition.y <= -3.5f)
        {
            customTransform.localPosition = new Vector2(customTransform.localPosition.x, -3.5f);
            customTransform.UpdateMatrix();
            velocity.y = 0;
            isGrounded = true;
        }
    }

    // === FIX 3: Collision Detection ===
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Player collided with: " + collision.gameObject.name);

        // Check if hit platform from above
        if (collision.gameObject.CompareTag("Platform") || collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            // Check if player is above the platform
            if (collision.contacts.Length > 0)
            {
                Vector2 contactNormal = collision.contacts[0].normal;

                // If normal points upward (player landed on top)
                if (contactNormal.y > 0.5f)
                {
                    isGrounded = true;
                    velocity.y = 0;
                    Debug.Log("Grounded on: " + collision.gameObject.name);
                }
            }
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        // Stay grounded while on platform
        if (collision.gameObject.CompareTag("Platform") || collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // Left platform
        if (collision.gameObject.CompareTag("Platform") || collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGrounded = false;
            Debug.Log("Left platform: " + collision.gameObject.name);
        }
    }

    // === Debug Visualization ===
    void OnDrawGizmos()
    {
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireCube(transform.position, col.size);
        }
    }
}