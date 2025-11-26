using UnityEngine;

public class SceneSetup : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject groundPrefab; 
    public GameObject wallPrefab;
    public GameObject playerPrefab;
    public GameObject platformPrefab;
    public GameObject collectiblePrefab;
    public GameObject decorationPrefab;

    [Header("Settings")]
    public int decorationCount = 60;

    void Start()
    {
        CreateGround();
        CreateWalls();
        CreatePlayer();
        CreatePlatforms();
        CreateCollectibles();
        CreateDecorations();
    }

    void CreateGround()
    {
        GameObject ground = Instantiate(groundPrefab, new Vector2(0, -5), Quaternion.identity);
        ground.transform.localScale = new Vector3(30, 2, 1);
        ground.AddComponent<BoxCollider2D>(); 
    }

    void CreateWalls()
    {
        GameObject leftWall = Instantiate(wallPrefab, new Vector2(-15, 0), Quaternion.identity);
        leftWall.transform.localScale = new Vector3(1, 20, 1);
        leftWall.AddComponent<BoxCollider2D>(); 

        GameObject rightWall = Instantiate(wallPrefab, new Vector2(15, 0), Quaternion.identity);
        rightWall.transform.localScale = new Vector3(1, 20, 1);
        rightWall.AddComponent<BoxCollider2D>(); 
    }

    void CreatePlayer()
    {
        if (playerPrefab != null)
        {
            GameObject player = Instantiate(playerPrefab, new Vector2(-8, 6), Quaternion.identity);
            player.name = "Player";
        }
    }

    void CreatePlatforms()
    {
        if (platformPrefab == null) return;


        // Platform pertama (tengah bawah)
        CreatePlatform(new Vector2(-1.2f, -2.3f), new Vector2(3, 0.5f), true, false);

        // Platform kedua (kiri)
        CreatePlatform(new Vector2(-8, -1), new Vector2(7, 0.5f), false, true);

        // Platform ketiga (tengah atas)
        CreatePlatform(new Vector2(0.5f, 1), new Vector2(3, 0.5f), false, true);

        // Platform keempat (kanan atas)
        CreatePlatform(new Vector2(5, 3), new Vector2(3, 0.5f), true, true);
    }

    void CreatePlatform(Vector2 position, Vector2 scale, bool moving, bool rotating)
    {
        GameObject platform = Instantiate(platformPrefab, position, Quaternion.identity);
        platform.transform.localScale = scale;

        PlatformBehavior behavior = platform.GetComponent<PlatformBehavior>();
        if (behavior != null)
        {
            // Set initial state if needed
        }
    }

    void CreateCollectibles()
    {
        if (collectiblePrefab == null) return;

        Vector2[] positions = new Vector2[]
        {
            new Vector2(-6, 0),
            new Vector2(-3, 2),
            new Vector2(0, 4),
            new Vector2(3, 2),
            new Vector2(6, 0),
            new Vector2(-4, -2),
            new Vector2(4, -2)
        };

        foreach (Vector2 pos in positions)
        {
            GameObject collectible = Instantiate(collectiblePrefab, pos, Quaternion.identity);
            collectible.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
        }
    }

    void CreateDecorations()
    {
        if (decorationPrefab == null) return;

        Color[] decorationColors = new Color[]
        {
            new Color(1f, 0.5f, 0.5f),    // Pink
            new Color(0.5f, 1f, 0.5f),    // Green
            new Color(0.5f, 0.5f, 1f),    // Blue
            new Color(1f, 1f, 0.5f),      // Yellow
            new Color(1f, 0.5f, 1f),      // Magenta
            new Color(0.5f, 1f, 1f)       // Cyan
        };

        // === REQUIREMENT: Procedurally placed in large quantities ===
        for (int i = 0; i < decorationCount; i++)
        {
            // Random position
            Vector2 randomPosition = new Vector2(
                Random.Range(-12f, 12f),
                Random.Range(-6f, 8f)
            );

            // Random scale
            Vector2 randomScale = new Vector2(
                Random.Range(0.1f, 0.4f),
                Random.Range(0.1f, 0.4f)
            );

            // Random color from palette
            Color randomColor = decorationColors[Random.Range(0, decorationColors.Length)];

            // Random opacity
            float randomOpacity = Random.Range(0.2f, 0.5f);

            // Random animation speeds
            float floatSpeed = Random.Range(0.3f, 1f);
            float rotationSpeed = Random.Range(-30f, 30f);

            // Create decoration with all random properties
            CreateDecoration(randomPosition, randomScale, randomColor, randomOpacity, floatSpeed, rotationSpeed);
        }
    }

    void CreateDecoration(Vector2 position, Vector2 scale, Color color, float opacity, float floatSpeed, float rotationSpeed)
    {
        GameObject decoration = Instantiate(decorationPrefab, position, Quaternion.identity);
        decoration.transform.localScale = scale;

        DecorationBehavior behavior = decoration.GetComponent<DecorationBehavior>();
        if (behavior != null)
        {
            behavior.Initialize(position, scale, color, opacity, floatSpeed, rotationSpeed);
        }
    }
}