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
    private Color playerColor = new Color(0.3f, 0.5f, 1f); // Biru

    void Awake()
    {
        // Add components kalau belum ada
        customTransform = GetComponent<CustomTransform>();
        if (customTransform == null)
            customTransform = gameObject.AddComponent<CustomTransform>();

        customRenderer = GetComponent<CustomRenderer>();
        if (customRenderer == null)
            customRenderer = gameObject.AddComponent<CustomRenderer>();
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

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = jumpForce;
            isGrounded = false;
            customRenderer.PulseBrightness(0.2f, 0.5f);
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

    void ApplyPhysics()
    {
        if (customTransform == null) return;

        // Gravity
        velocity.y -= 20f * Time.deltaTime;
        customTransform.Translate(new Vector2(0, velocity.y * Time.deltaTime));

        // Ground check
        if (customTransform.localPosition.y <= -3.5f)
        {
            customTransform.localPosition = new Vector2(customTransform.localPosition.x, -3.5f);
            customTransform.UpdateMatrix();
            velocity.y = 0;
            isGrounded = true;
        }
    }
}