using UnityEngine;

public class HouseFade : MonoBehaviour
{
    [Header("Ayarlar")]
    public float targetAlpha = 0.3f; // Şeffaflık miktarı
    public float fadeSpeed = 5f;    // Geçiş hızı

    private SpriteRenderer[] allSprites;
    private float currentAlpha = 1f;
    private float targetValue = 1f;

    void Start()
    {
        // Evin içindeki tüm SpriteRenderer bileşenlerini otomatik bulur
        allSprites = GetComponentsInChildren<SpriteRenderer>();
    }

    void Update()
    {
        // Yumuşak geçiş yap (Lerp)
        currentAlpha = Mathf.Lerp(currentAlpha, targetValue, Time.deltaTime * fadeSpeed);

        foreach (var sprite in allSprites)
        {
            Color c = sprite.color;
            c.a = currentAlpha;
            sprite.color = c;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Eğer giren nesne oyuncuysa (Tag'inin Player olduğundan emin ol)
        if (other.CompareTag("Player"))
        {
            targetValue = targetAlpha;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            targetValue = 1f;
        }
    }
}