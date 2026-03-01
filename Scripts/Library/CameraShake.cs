using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    // Titreme işlemini yapacak olan zamanlayıcı (Coroutine)
    public IEnumerator Shake(float duration, float magnitude)
    {
        // Kameranın orijinal pozisyonunu hafızaya al (titreme bitince buraya dönecek)
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            // Belirlenen şiddette rastgele X ve Y koordinatları üret
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            // Kamerayı bu rastgele noktalara hızlıca oynat
            transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            elapsed += Time.deltaTime;
            yield return null; // Bir sonraki frame'i bekle
        }

        // Süre bitince kamerayı orijinal yerine pürüzsüzce geri koy
        transform.localPosition = originalPos;
    }
}