using UnityEngine;

public class RacingCameraFollow : MonoBehaviour
{
    [Header("Takip Edilecek Obje")]
    public Transform target; // Oyuncu arabamız

    private Vector3 offset; // Kamera ile araba arasındaki o ilk mesafeyi aklında tutacak

    void Start()
    {
        // Oyun başladığı an, kameranın durduğu yer ile arabanın yeri arasındaki mesafeyi hesapla ve kaydet.
        // Böylece kamerayı Unity sahnesinde nereye koyarsan koy, hep o açıyı koruyacak.
        offset = transform.position - target.position;
    }

    // DİKKAT: Kameralar her zaman Update yerine LateUpdate içinde çalıştırılır.
    // Çünkü önce arabanın hareketini bitirmesi beklenir, sonra kamera onun peşinden gider. Yoksa ekran titrer!
    void LateUpdate()
    {
        if (target != null)
        {
            // Kameranın sadece POZİSYONUNU arabanın peşinden götürüyoruz.
            // Rotasyon (dönüş) kodunu yazmadığımız için kamera hep dümdüz yola bakmaya devam edecek!
            transform.position = target.position + offset;
        }
    }
}