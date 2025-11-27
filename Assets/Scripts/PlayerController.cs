using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CustomTransform customTransform;
    private CustomRenderer customRenderer;
    private Rigidbody2D rb;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    public int maxJumps = 2; // Maximum number of jumps (2 = double jump)
    private int jumpsRemaining;

    [Header("Projectile")]
    public GameObject projectilePrefab;
    public float projectileSpawnDistance = 1f;
    public float shootCooldown = 0.3f;
    private float lastShootTime;

    [Header("Ground Check")]
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.2f;

    private bool isGrounded;
    private Color playerColor = new Color(0.3f, 0.5f, 1f); // Biru
    
    // Animator
    private Animator animator;

    void Awake()
    {
        // Add components kalau belum ada
        customTransform = GetComponent<CustomTransform>();
        if (customTransform == null)
            customTransform = gameObject.AddComponent<CustomTransform>();

        customRenderer = GetComponent<CustomRenderer>();
        if (customRenderer == null)
            customRenderer = gameObject.AddComponent<CustomRenderer>();

        // Add BoxCollider2D
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(0.5f, 0.5f);
            collider.isTrigger = false;
        }

        // Proper Rigidbody2D setup
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 2f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        // Ensure tag is set
        if (!gameObject.CompareTag("Player"))
            gameObject.tag = "Player";

        // Get Animator component
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogWarning("No Animator component found on Player!");
        }
    }

    void Start()
    {
        customRenderer.SetColor(playerColor);
        jumpsRemaining = maxJumps;
    }

    void Update()
    {
        HandleMovementInput();
        HandleTransformInput();
        HandleShootInput();
        HandleRandomColor();
        CheckGrounded();
        UpdateAnimator();
    }

    void FixedUpdate()
    {
        ApplyMovement();
    }

    void HandleMovementInput()
    {
        // Jump - can jump up to maxJumps times
        bool jumpPressed = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow);
        
        if (jumpPressed && jumpsRemaining > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpsRemaining--;
            
            // Visual feedback - pulse brighter on second jump
            float pulseDuration = (jumpsRemaining == 0) ? 0.3f : 0.5f;
            customRenderer.PulseBrightness(0.2f, pulseDuration);
            
            Debug.Log($"Jump! Jumps remaining: {jumpsRemaining}");
        }
    }

    void ApplyMovement()
    {
        // Horizontal movement using Rigidbody2D
        float horizontal = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(horizontal * moveSpeed, rb.linearVelocity.y);
    }

    void CheckGrounded()
    {
        // Raycast downward to check if grounded
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col != null)
        {
            Vector2 rayOrigin = new Vector2(transform.position.x, transform.position.y - col.bounds.extents.y);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, groundCheckDistance);

            bool wasGrounded = isGrounded;
            isGrounded = hit.collider != null || (rb.linearVelocity.y <= 0.1f && rb.linearVelocity.y >= -0.1f && wasGrounded);

            // Reset jumps when landing
            if (isGrounded && !wasGrounded)
            {
                jumpsRemaining = maxJumps;
                Debug.Log("Landed! Jumps reset.");
            }

            // Debug visualization
            Debug.DrawRay(rayOrigin, Vector2.down * groundCheckDistance, isGrounded ? Color.green : Color.red);
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
        bool shootPressed = Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(0);
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
        if (projectilePrefab == null) return;

        customRenderer.PulseBrightness(0.2f, 0.3f);

        // Hitung arah
        float angle = customTransform.localRotation * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        Vector2 spawnPosition = customTransform.localPosition + direction * projectileSpawnDistance;

        // Spawn projectile
        GameObject projectileObj = Instantiate(projectilePrefab);
        ProjectileBehavior projectile = projectileObj.GetComponent<ProjectileBehavior>();
        if (projectile != null)
            projectile.Initialize(direction, spawnPosition, playerColor);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform") || collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            // Check if hit from above
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f)
                {
                    isGrounded = true;
                    jumpsRemaining = maxJumps; // Reset jumps on landing
                    Debug.Log("Landed on: " + collision.gameObject.name);
                    break;
                }
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform") || collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGrounded = false;
        }
    }

    void OnDrawGizmos()
    {
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireCube(transform.position, col.size);
        }
    }

    void UpdateAnimator()
    {
        if (animator == null) return;

        // Get horizontal movement input
        float horizontal = Input.GetAxisRaw("Horizontal");
        
        // moveX: horizontal input value (-1, 0, or 1)
        animator.SetFloat("moveX", horizontal);
        
        // moveY: vertical velocity from rigidbody
        animator.SetFloat("moveY", rb.linearVelocity.y);
        
        // isRunning: true when moving horizontally
        bool isRunning = Mathf.Abs(horizontal) > 0.1f;
        animator.SetBool("isRunning", isRunning);
        
        // isJumping: true when in air and moving upward
        bool isJumping = !isGrounded && rb.linearVelocity.y > 0.1f;
        animator.SetBool("isJumping", isJumping);
        
        // Additional parameters for more control
        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("verticalVelocity", rb.linearVelocity.y);
    }
}