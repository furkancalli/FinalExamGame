using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    [Header("Arabalar (Engeller)")]
    public GameObject[] arabaPrefablari; 

    [Header("Ödül")]
    public GameObject boostPrefab;

    [Header("Oyuncu Referansı")]
    public Transform player;

    [Header("Üretim Ayarları (KUSURSUZ MESAFE SİSTEMİ)")]
    public float spawnUzakligi = 90f; 
    public float spawnAraligi = 25f;  
    
    // YENİ: Artık hayali bir Z noktası değil, gerçekten son doğan objeyi takip edeceğiz!
    private Transform sonDoganObje; 

    public float arabaYuksekligi = 0f;
    public float boostYuksekligi = 2f; 

    [Header("Boost (Kahve) Ayarları")]
    [Range(0f, 1f)] public float boostIhtimali = 0.1f; 
    public int boostBeklemeSatiri = 3; 
    private int gecenSatirSayisi = 3; 

    private float[] seritler = { -3.5f, 0f, 3.5f };

    void Start()
    {
        // Oyun başlar başlamaz ilk engeli üretelim
        ObjeyiDogur();
    }

    void Update()
    {
        // Yeni doğurma noktası hep oyuncunun 90 metre ilerisi
        float spawnNoktasiZ = player.position.z + spawnUzakligi;

        // EĞER: Henüz hiç obje doğmadıysa VEYA 
        // Yeni doğurma noktası ile son doğan objenin arasındaki fiziksel mesafe "spawnAraligi" (25) kadarsa yeni obje doğur!
        if (sonDoganObje == null || (spawnNoktasiZ - sonDoganObje.position.z) >= spawnAraligi)
        {
            ObjeyiDogur();
        }
    }

    void ObjeyiDogur()
    {
        int ayniHizadaKacObje = Random.Range(1, 3);
        List<float> musaitSeritler = new List<float>(seritler);

        bool buSatirdaKahveCikti = false; 
        gecenSatirSayisi++; 
        
        float spawnNoktasiZ = player.position.z + spawnUzakligi; 

        for (int i = 0; i < ayniHizadaKacObje; i++)
        {
            int rastgeleIndex = Random.Range(0, musaitSeritler.Count);
            float secilenX = musaitSeritler[rastgeleIndex];
            musaitSeritler.RemoveAt(rastgeleIndex);

            int rastgeleArabaIndex = Random.Range(0, arabaPrefablari.Length);
            GameObject dogacakObje = arabaPrefablari[rastgeleArabaIndex];

            // Kahve (Boost) çıkma ihtimali hesaplaması
            if (buSatirdaKahveCikti == false && gecenSatirSayisi >= boostBeklemeSatiri)
            {
                if (Random.value < boostIhtimali) 
                {
                    dogacakObje = boostPrefab;
                    buSatirdaKahveCikti = true;
                    gecenSatirSayisi = 0; 
                }
            }
            
            float dogumY = (dogacakObje == boostPrefab) ? boostYuksekligi : arabaYuksekligi;
            Vector3 dogumNoktasi = new Vector3(secilenX, dogumY, spawnNoktasiZ);
            
            GameObject yaratilanObje = Instantiate(dogacakObje, dogumNoktasi, Quaternion.identity);
            Destroy(yaratilanObje, 20f); 

            // YENİ: Yarattığımız bu objeyi "son doğan obje" olarak aklımızda tutuyoruz.
            // Aynı hizada birden fazla araba çıksa da Z eksenleri aynı olduğu için birini tutmak yeterli.
            sonDoganObje = yaratilanObje.transform;
        }
    }
}