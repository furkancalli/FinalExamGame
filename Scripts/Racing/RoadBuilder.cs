using UnityEngine;

public class RoadBuilder : MonoBehaviour
{
    [Header("Şerit Ayarları")]
    public GameObject seritPrefab;
    public float yolUzunlugu = 800f; 
    public float cizgiAraligi = 6f;  
    private float[] cizgiXKonumlari = { -1.75f, 1.75f }; 

    [Header("Çevre Ayarları")]
    public GameObject kenarObjesiPrefab; // YENİ: Ağaç veya Lamba
    public float objeAraligi = 15f;      // YENİ: Kaç metrede bir ağaç çıkacak?

    // Ağaçların duracağı sağ ve sol kaldırım koordinatları
    private float[] kenarXKonumlari = { -5.5f, 5.5f }; 

    void Start()
    {
        // 1. ŞERİT ÇİZGİLERİNİ DÖŞE
        for (float z = 0; z < yolUzunlugu; z += cizgiAraligi)
        {
            foreach (float x in cizgiXKonumlari)
            {
                Vector3 konum = new Vector3(x, 0.02f, z); 
                Instantiate(seritPrefab, konum, Quaternion.identity, transform);
            }
        }

        // 2. YENİ: YOL KENARI OBJELERİNİ (AĞAÇ/LAMBA) DÖŞE
        for (float z = 0; z < yolUzunlugu; z += objeAraligi)
        {
            foreach (float x in kenarXKonumlari)
            {
                // Yüksekliği 0 yapıyoruz ki tam kaldırımın/yere otursun
                Vector3 konum = new Vector3(x, 0f, z); 
                Instantiate(kenarObjesiPrefab, konum, Quaternion.identity, transform);
            }
        }
    }
}