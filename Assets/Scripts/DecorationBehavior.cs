using UnityEngine;
public class DecorationBehavior : MonoBehaviour
{
    private CustomTransform customTransform;
    private CustomRenderer customRenderer;
    private float floatSpeed;
    private float rotationSpeed;
    private float startY;
    private float floatAmount = 0.5f;

    public void Initialize(Vector2 pos, Vector2 scale, Color color, float opacity, float fSpeed, float rSpeed)
    {
        customTransform = GetComponent<CustomTransform>();
        if (customTransform == null)
            customTransform = gameObject.AddComponent<CustomTransform>();

        customRenderer = GetComponent<CustomRenderer>();
        if (customRenderer == null)
            customRenderer = gameObject.AddComponent<CustomRenderer>();

        customTransform.localPosition = pos;
        customTransform.localScale = scale;
        customTransform.UpdateMatrix();

        startY = pos.y;
        color.a = opacity;
        customRenderer.SetColor(color);

        floatSpeed = fSpeed;
        rotationSpeed = rSpeed;
    }

    void Update()
    {
        // Float up and down
        float newY = startY + Mathf.Sin(Time.time * floatSpeed) * floatAmount;
        customTransform.localPosition = new Vector2(customTransform.localPosition.x, newY);
        customTransform.UpdateMatrix();

        // Rotate
        customTransform.Rotate(rotationSpeed * Time.deltaTime);
    }
}