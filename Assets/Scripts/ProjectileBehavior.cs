using UnityEngine;
public class ProjectileBehavior : MonoBehaviour
{
    private CustomTransform customTransform;
    private CustomRenderer customRenderer;
    private Vector2 direction;
    private float speed = 8f;
    private float maxDistance = 15f;
    private Vector2 startPosition;

    void Awake()
    {
        customTransform = GetComponent<CustomTransform>();
        if (customTransform == null)
            customTransform = gameObject.AddComponent<CustomTransform>();

        customRenderer = GetComponent<CustomRenderer>();
        if (customRenderer == null)
            customRenderer = gameObject.AddComponent<CustomRenderer>();
    }

    public void Initialize(Vector2 dir, Vector2 spawnPos, Color color)
    {
        direction = dir.normalized;
        startPosition = spawnPos;
        customTransform.localPosition = spawnPos;
        customTransform.UpdateMatrix();
        customRenderer.SetColor(color);
        customRenderer.SetBrightness(1.5f);
    }

    void Update()
    {
        // Move forward
        customTransform.Translate(direction * speed * Time.deltaTime);

        // Rotate
        customTransform.Rotate(300f * Time.deltaTime);

        // Destroy if too far
        if (Vector2.Distance(customTransform.localPosition, startPosition) > maxDistance)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Collectible"))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}