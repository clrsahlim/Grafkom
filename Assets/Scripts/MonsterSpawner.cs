using UnityEngine;
using System.Collections;

public class MonsterSpawner : MonoBehaviour
{
    [Header("Spawning Settings")]
    public GameObject monsterPrefab;
    public float spawnInterval = 8f;
    public float spawnDistanceMin = 8f;
    public float spawnDistanceMax = 12f;

    [Header("Spawn Area")]
    public float minX = -20f;
    public float maxX = 55f;
    public float minY = -4f;
    public float maxY = 8f;

    [Header("Monster Settings")]
    public Color[] monsterColors = new Color[]
    {
        new Color(1f, 0.2f, 0.2f),  // Red
        new Color(0.8f, 0.2f, 0.8f), // Purple
        new Color(1f, 0.5f, 0f),     // Orange
        new Color(0.2f, 0.8f, 0.2f)  // Green
    };

    private Transform playerTransform;
    private float nextSpawnTime;

    void Start()
    {
        // Find player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
            Debug.Log("[SPAWNER] Player found!");
        }
        else
        {
            Debug.LogWarning("[SPAWNER] Player not found!");
        }

        nextSpawnTime = Time.time + spawnInterval;
    }

    void Update()
    {
        // Stop spawning if game has ended
        if (GameUIManager.GameEnded)
        {
            return;
        }

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

        // Check if it's time to spawn
        if (Time.time >= nextSpawnTime)
        {
            SpawnMonster();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    void SpawnMonster()
    {
        if (monsterPrefab == null)
        {
            Debug.LogWarning("[SPAWNER] Monster prefab is not assigned!");
            return;
        }

        Vector2 spawnPosition = GetRandomSpawnPosition();

        // Create monster
        GameObject monster = Instantiate(monsterPrefab, spawnPosition, Quaternion.identity);
        monster.name = "Monster_" + Time.time;

        // Set random color
        MonsterBehavior behavior = monster.GetComponent<MonsterBehavior>();
        if (behavior != null)
        {
            Color randomColor = monsterColors[Random.Range(0, monsterColors.Length)];
            behavior.monsterColor = randomColor;
        }

        Debug.Log($"[SPAWNER] Monster spawned at {spawnPosition}");
    }

    Vector2 GetRandomSpawnPosition()
    {
        Vector2 spawnPos = Vector2.zero;
        int maxAttempts = 10;
        int attempts = 0;

        while (attempts < maxAttempts)
        {
            // Generate random position near player
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float distance = Random.Range(spawnDistanceMin, spawnDistanceMax);

            spawnPos.x = playerTransform.position.x + Mathf.Cos(angle) * distance;
            spawnPos.y = playerTransform.position.y + Mathf.Sin(angle) * distance;

            // Clamp to spawn area boundaries
            spawnPos.x = Mathf.Clamp(spawnPos.x, minX, maxX);
            spawnPos.y = Mathf.Clamp(spawnPos.y, minY, maxY);

            // Check if position is valid (not too close to player)
            float distToPlayer = Vector2.Distance(spawnPos, playerTransform.position);
            if (distToPlayer >= spawnDistanceMin)
            {
                break;
            }

            attempts++;
        }

        return spawnPos;
    }

    void OnDrawGizmos()
    {
        if (playerTransform != null)
        {
            // Draw spawn range around player
            Gizmos.color = Color.yellow;
            DrawCircle(playerTransform.position, spawnDistanceMin);

            Gizmos.color = Color.red;
            DrawCircle(playerTransform.position, spawnDistanceMax);
        }
    }

    void DrawCircle(Vector3 center, float radius)
    {
        int segments = 32;
        float angle = 0f;
        float angleStep = 360f / segments;

        Vector3 lastPoint = center + new Vector3(Mathf.Cos(0) * radius, Mathf.Sin(0) * radius, 0);

        for (int i = 1; i <= segments; i++)
        {
            angle = angleStep * i * Mathf.Deg2Rad;
            Vector3 newPoint = center + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
            Gizmos.DrawLine(lastPoint, newPoint);
            lastPoint = newPoint;
        }
    }
}