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
    public GameObject flagPrefab;
    public GameObject monsterPrefab;

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
        CreateFlag();
        CreateMonsterSpawner();
    }

    void CreateGround()
    {
        GameObject ground = Instantiate(groundPrefab, new Vector2(0, -5), Quaternion.identity);
        ground.AddComponent<BoxCollider2D>();
    }

    void CreateWalls()
    {
        // Left Wall
        GameObject leftWall = Instantiate(wallPrefab, new Vector2(-28, 0), Quaternion.identity);
        leftWall.transform.localScale = new Vector3(1, 20, 1);
        leftWall.layer = LayerMask.NameToLayer("Ground");
        leftWall.name = "LeftWall";
        SpriteRenderer leftRenderer = leftWall.GetComponent<SpriteRenderer>();
       

        // Right Wall
        GameObject rightWall = Instantiate(wallPrefab, new Vector2(35, 0), Quaternion.identity);
        rightWall.transform.localScale = new Vector3(1, 20, 1);
        BoxCollider2D rightCollider = rightWall.AddComponent<BoxCollider2D>();
        rightCollider.size = new Vector2(1, 20);
        rightWall.layer = LayerMask.NameToLayer("Ground"); 
        rightWall.name = "RightWall";
        SpriteRenderer rightRenderer = rightWall.GetComponent<SpriteRenderer>();
    }

    void CreatePlayer()
    {
        if (playerPrefab != null)
        {
            GameObject player = Instantiate(playerPrefab, new Vector2(-20, 6), Quaternion.identity);
            player.name = "Player";
        }
    }

    void CreatePlatforms()
    {
        if (platformPrefab == null) return;

        CreatePlatform(new Vector2(-13, -2.5f), new Vector2(3.8f, 0.5f), true, false);
        CreatePlatform(new Vector2(-7.5f, 2.5f), new Vector2(3.8f, 0.5f), false, true);
        CreatePlatform(new Vector2(-2f, -2.5f), new Vector2(3.8f, 0.5f), true, false);
        CreatePlatform(new Vector2(3.5f, 0), new Vector2(3.8f, 0.5f), false, true);
        CreatePlatform(new Vector2(9, -2), new Vector2(3.8f, 0.5f), false, true);
        CreatePlatform(new Vector2(14.5f, 2f), new Vector2(3.8f, 0.5f), true, true);
        CreatePlatform(new Vector2(20, -1.8f), new Vector2(3.8f, 0.5f), false, true);
        CreatePlatform(new Vector2(25, 4f), new Vector2(3.8f, 0.5f), true, true);
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
            new Vector2(-13, -1.5f),
            new Vector2(-7.5f, 3.5f),
            new Vector2(-2f, -1.5f),
            new Vector2(3.5f, 1),
            new Vector2(9, -1),
            new Vector2(14.5f, 3f),
            new Vector2(20, -0.8f),
            new Vector2(25, 5f),
            new Vector2(-7.5f, -3.1f),
            new Vector2(3.5f, -3.1f),
            new Vector2(14.5f, -3.1f),
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
            new Color(1f, 0.5f, 0.5f),
            new Color(0.5f, 1f, 0.5f),
            new Color(0.5f, 0.5f, 1f),
            new Color(1f, 1f, 0.5f),
            new Color(1f, 0.5f, 1f),
            new Color(0.5f, 1f, 1f)
        };

        for (int i = 0; i < decorationCount; i++)
        {
            Vector2 randomPosition = new Vector2(
                Random.Range(-40f, 60f),
                Random.Range(-6f, 8f)
            );

            Vector2 randomScale = new Vector2(
                Random.Range(0.1f, 0.4f),
                Random.Range(0.1f, 0.4f)
            );

            Color randomColor = decorationColors[Random.Range(0, decorationColors.Length)];
            float randomOpacity = Random.Range(0.2f, 0.5f);
            float floatSpeed = Random.Range(0.3f, 1f);
            float rotationSpeed = Random.Range(-30f, 30f);

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

    void CreateFlag()
    {
        if (flagPrefab == null) return;

        GameObject flag = Instantiate(flagPrefab, new Vector2(30, -2.5f), Quaternion.identity);
        flag.transform.localScale = new Vector3(2f, 2f, 1f);
        flag.name = "FinishFlag";

        BoxCollider2D collider = flag.GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            collider = flag.AddComponent<BoxCollider2D>();
        }
        collider.isTrigger = true;
    }

    void CreateMonsterSpawner()
    {
        if (monsterPrefab == null)
        {
            Debug.LogWarning("[SCENE SETUP] Monster prefab not assigned!");
            return;
        }

        GameObject spawnerObj = new GameObject("MonsterSpawner");
        MonsterSpawner spawner = spawnerObj.AddComponent<MonsterSpawner>();
        spawner.monsterPrefab = monsterPrefab;
        spawner.spawnInterval = 8f;
        spawner.spawnDistanceMin = 8f;
        spawner.spawnDistanceMax = 12f;

        Debug.Log("[SCENE SETUP] Monster spawner created!");
    }
}