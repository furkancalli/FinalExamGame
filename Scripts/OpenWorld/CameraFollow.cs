using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Hedef Ayarları")]
    public Transform target;       // Buraya Player objesini sürükleyeceksin
    public Vector3 offset;         // Kameranın oyuncudan ne kadar uzakta duracağı

    [Header("Yumuşaklık Ayarları")]
    public float smoothTime = 0.3f; // Takip hızı (ne kadar düşükse o kadar hızlı)
    private Vector3 currentVelocity = Vector3.zero;

    void LateUpdate()
    {
        if (target != null)
        {
            // Kameranın gitmesi gereken ideal pozisyon
            Vector3 targetPosition = target.position + offset;

            // SmoothDamp fonksiyonu, Lerp'ten daha yumuşak ve doğal bir takip sağlar
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);
        }
    }
}