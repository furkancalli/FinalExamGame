using UnityEngine;

public class KameraTakip : MonoBehaviour
{
    [Header("Takip Edilecekler")]
    public Transform oyuncu;
    public Transform boss;

    [Header("Kamera Açı ve Zoom Ayarları")]
    public Vector3 minOfset = new Vector3(0f, 10f, -10f); 
    public Vector3 maxOfset = new Vector3(0f, 25f, -25f); 
    public float maxAralikMesafesi = 30f; 
    
    public float yumusamaSuresi = 0.25f; 

    private Vector3 anlikHiz = Vector3.zero;

    [Header("Titreme (Screen Shake)")]
    private float sarsintiSuresi = 0f;
    private float sarsintiGucu = 0f;
    private float sarsintiSönümlenme = 1f;

    void LateUpdate()
    {
        if (oyuncu == null) return;

        // 1. Hedef Noktayı Bul
        Vector3 merkezNokta = oyuncu.position;
        float aralarindakiMesafe = 0f;

        if (boss != null)
        {
            merkezNokta = (oyuncu.position + boss.position) / 2f;
            aralarindakiMesafe = Vector3.Distance(oyuncu.position, boss.position);
        }

        // 2. Mesafeye Göre Dinamik Zoom
        float zoomOrani = Mathf.Clamp01(aralarindakiMesafe / maxAralikMesafesi);
        Vector3 guncelOfset = Vector3.Lerp(minOfset, maxOfset, zoomOrani);

        // 3. Pürüzsüz Takip (SmoothDamp) ile Yeni Pozisyon
        Vector3 hedefPozisyon = merkezNokta + guncelOfset;
        Vector3 yeniPozisyon = Vector3.SmoothDamp(transform.position, hedefPozisyon, ref anlikHiz, yumusamaSuresi);

        // KAMERAYI YENİ YERİNE KOY
        transform.position = yeniPozisyon;

        // --- İŞTE EKSİK OLAN SİHİRLİ SATIR (ROTASYON DÜZELTMESİ) ---
        // Kamera her zaman ikisinin tam ortasına doğru eğilip baksın!
        transform.LookAt(merkezNokta);

        // 4. Titreme (Screen Shake) Efekti (Titremenin doğal durması için LookAt'ten sonra eklenmeli)
        if (sarsintiSuresi > 0)
        {
            transform.position += Random.insideUnitSphere * sarsintiGucu;
            sarsintiSuresi -= Time.deltaTime;
            sarsintiGucu = Mathf.Lerp(sarsintiGucu, 0f, Time.deltaTime * sarsintiSönümlenme);
        }
    }

    public void Titret(float guc, float sure)
    {
        sarsintiGucu = guc;
        sarsintiSuresi = sure;
        sarsintiSönümlenme = guc / sure;
    }
}