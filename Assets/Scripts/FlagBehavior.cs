using UnityEngine;

public class FlagBehavior : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private float waveTime = 0f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Level Complete!");
            // Tambahkan logic finish game di sini
        }
    }
}