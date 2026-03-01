using UnityEngine;

public class CevreOlusturucu : MonoBehaviour
{
    [Header("Kütüphane Malzemeleri")]
    public GameObject[] kullanilacakObjeler; // Buraya kitapları, rafları sürükleyeceksin

    [Header("Yayılım Ayarları")]
    public int objeSayisi = 300;       // Etrafa kaç tane obje saçılacak?
    public float icYaricap = 15f;      // Arenanın BOŞ kalacak temiz alanı (Örn: 15 metre)
    public float disYaricap = 25f;     // Eşyaların yığılacağı sınır (Örn: 25 metre)
    
    [Header("Varyasyon (Rastgelelik)")]
    public float minBoyut = 0.8f;      // Objeler biraz küçülsün
    public float maxBoyut = 1.3f;      // Objeler biraz büyüsün

    // SİHİRLİ KOD: Bu satır sayesinde kodu editörde sağ tıklayıp çalıştırabileceğiz!
    [ContextMenu("Arenanin Etrafini Doldur!")]
    public void EtrafiDoldur()
    {
        if (kullanilacakObjeler.Length == 0)
        {
            Debug.LogWarning("Hiç obje koymadın! Önce listeyi doldur.");
            return;
        }

        // Hiyerarşi kirlenmesin diye hepsini tek bir boş objenin içine koyacağız
        GameObject parentObje = new GameObject("Kutuphane_Yiginlari");
        parentObje.transform.position = transform.position;

        for (int i = 0; i < objeSayisi; i++)
        {
            // 1. Listeden rastgele bir obje seç (Kitap, raf, masa vs.)
            GameObject secilenObje = kullanilacakObjeler[Random.Range(0, kullanilacakObjeler.Length)];

            // 2. Rastgele bir pozisyon bul (Donut mantığı: İç yarıçap ile dış yarıçap arasında)
            Vector2 rastgeleDaire = Random.insideUnitCircle.normalized * Random.Range(icYaricap, disYaricap);
            Vector3 spawnPozisyonu = new Vector3(rastgeleDaire.x, 0f, rastgeleDaire.y) + transform.position;

            // 3. Objenin yönünü rastgele çevir (Dağınık dursunlar)
            Quaternion rastgeleDonus = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

            // 4. Objeyi Yarat!
            GameObject yeniObje = Instantiate(secilenObje, spawnPozisyonu, rastgeleDonus, parentObje.transform);

            // 5. Boyutunu hafifçe değiştir ki hepsi kopyala-yapıştır gibi durmasın
            float rastgeleBoyut = Random.Range(minBoyut, maxBoyut);
            yeniObje.transform.localScale = secilenObje.transform.localScale * rastgeleBoyut;
        }

        Debug.Log("Sihir gerçekleşti! " + objeSayisi + " adet obje arenanın etrafına dizildi.");
    }
}