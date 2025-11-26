using UnityEngine;
using System.Collections;
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(CustomTransform))]
public class CustomRenderer : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private CustomTransform customTransform;
    private Material material;
    public Color color = Color.white;
    public float brightness = 1f;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        customTransform = GetComponent<CustomTransform>();
    }

    void Start()
    {
        if (spriteRenderer.material != null)
        {
            material = new Material(spriteRenderer.material);
            spriteRenderer.material = material;
        }
        UpdateShaderProperties();
    }

    void LateUpdate()
    {
        UpdateShaderProperties();
    }

    void UpdateShaderProperties()
    {
        if (material != null && customTransform != null)
        {
            material.SetMatrix("_CustomTransform", customTransform.GetMatrix());
            material.SetColor("_CustomColor", color);
            material.SetFloat("_Brightness", brightness);
        }
    }

    public void SetColor(Color newColor)
    {
        color = newColor;
        UpdateShaderProperties();
    }

    public void SetBrightness(float value)
    {
        brightness = value;
        UpdateShaderProperties();
    }

    public void SetTransformMatrix(Matrix4x4 matrix)
    {
        if (material != null)
        {
            material.SetMatrix("_CustomTransform", matrix);
        }
    }

    public void PulseBrightness(float duration, float intensity)
    {
        StartCoroutine(PulseBrightnessCoroutine(duration, intensity));
    }

    IEnumerator PulseBrightnessCoroutine(float duration, float intensity)
    {
        float original = brightness;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            brightness = original + Mathf.Sin(t * Mathf.PI) * intensity;
            UpdateShaderProperties();
            yield return null;
        }
        brightness = original;
        UpdateShaderProperties();
    }
}