using UnityEngine;
public class PlatformBehavior : MonoBehaviour
{
    private CustomTransform customTransform;
    private CustomRenderer customRenderer;
    private Vector2 startPosition;
    private bool isMoving = false;
    private bool isRotating = false;

    void Awake()
    {
        customTransform = gameObject.AddComponent<CustomTransform>();
        customRenderer = gameObject.AddComponent<CustomRenderer>();
        startPosition = transform.position;
        customTransform.localPosition = startPosition;
        customRenderer.SetColor(new Color(0.6f, 0.4f, 0.2f));

        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col == null)
            gameObject.AddComponent<BoxCollider2D>();
    }

    void Update()
    {
        if (isMoving)
        {
            float offsetX = Mathf.Sin(Time.time * 2f) * 2f;
            customTransform.localPosition = startPosition + new Vector2(offsetX, 0);
            customTransform.UpdateMatrix();
        }
        if (isRotating)
        {
            customTransform.Rotate(45f * Time.deltaTime);
        }
    }

    void OnMouseDown()
    {
        isMoving = !isMoving;
        isRotating = !isRotating;
        customRenderer.SetColor(Random.ColorHSV());
        customRenderer.PulseBrightness(0.5f, 0.2f);
    }
}