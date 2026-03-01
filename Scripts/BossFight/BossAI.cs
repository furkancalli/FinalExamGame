using UnityEngine;
using System.Collections;

public class BossAI : MonoBehaviour
{
    [Header("Gerekli Objeler")]
    public Transform oyuncu;
    public GameObject mermiPrefab;   
    public Transform atesNoktasi;    
    public GameObject dominoKupPrefab; 

    [Header("Saldırı Ayarları")]
    public float itmeGucu = 40f; 
    public float itmeMenzili = 12f; 
    
    private int bossFazi = 1; 
    private float hizCarpani = 1f;
    private int donguSayaci = 0; 

    private Animator anim; 
    private MeshRenderer bossRengi;
    private Color orijinalRenk;

    void Start()
    {
        anim = GetComponent<Animator>();
        bossRengi = GetComponent<MeshRenderer>();
        if (bossRengi != null) orijinalRenk = bossRengi.material.color;

        StartCoroutine(BossDongusu());
    }

    void OyuncuyaDon()
    {
        Vector3 yon = (oyuncu.position - transform.position).normalized;
        yon.y = 0;
        transform.rotation = Quaternion.LookRotation(yon);
    }

    IEnumerator BossDongusu()
    {
        yield return new WaitForSeconds(2f); 

        while (true) 
        {
            // ==========================================================
            // AŞAMA 1: UZAKTAN MERMİ ATMA
            // ==========================================================
            OyuncuyaDon(); // Sadece saldırı BAŞLAMADAN önce döner
            anim.SetTrigger("doFarAttack");
            yield return new WaitForSeconds(0.5f * hizCarpani);
            YelpazeAtesEt(); 
            yield return new WaitForSeconds(0.5f * hizCarpani);
            YelpazeAtesEt(); 
            
            // SENKRON FİX: Animasyonun tamamen bitip Idle'a dönmesi için kesin bekleme
            yield return new WaitForSeconds(2f * hizCarpani); 
            
            OyuncuyaDon(); 
            anim.SetTrigger("doFarAttack");
            yield return new WaitForSeconds(0.5f * hizCarpani);
            YelpazeAtesEt(); 
            yield return new WaitForSeconds(0.5f * hizCarpani);
            YelpazeAtesEt(); 

            yield return new WaitForSeconds(2f * hizCarpani); // Toparlanma payı

            // ==========================================================
            // AŞAMA 2: YAKIN ÇEKİM VE DOMİNO KÜPLERİ
            // ==========================================================
            OyuncuyaDon();
            anim.SetTrigger("doCloseAttack");
            yield return new WaitForSeconds(1f * hizCarpani); 
            // --- EKRAN SALLANSIN! (Güç: 0.5f, Süre: 0.3 saniye) ---
            if (Camera.main != null)
            {
                Camera.main.GetComponent<KameraTakip>().Titret(0.5f, 0.3f);
            }
            StartCoroutine(DominoKupleriniYarat());

            // SENKRON FİX: Domino saldırısı bitene kadar Boss başka bir şey yapmasın
            yield return new WaitForSeconds(3f * hizCarpani);

            donguSayaci++; 

            // ==========================================================
            // AŞAMA 3: YERE DÜŞME VE FAZ ATLATMA
            // ==========================================================
            if (donguSayaci >= 3)
            {
                anim.SetTrigger("doFall");
                if (bossRengi != null) bossRengi.material.color = Color.gray; 
                
                yield return new WaitForSeconds(4f); 
                
                if (bossRengi != null) bossRengi.material.color = orijinalRenk;

                anim.SetTrigger("doGetUp");
                yield return new WaitForSeconds(1.5f * hizCarpani); 

                anim.SetTrigger("doRegularAttack");
                yield return new WaitForSeconds(0.5f * hizCarpani);
                // --- DAHA ŞİDDETLİ EKRAN SALLANTISI (Güç: 1.0f, Süre: 0.5 saniye) ---
                if (Camera.main != null)
                {
                    Camera.main.GetComponent<KameraTakip>().Titret(1.0f, 0.5f);
                } 
                AlanHasariVeSavurma(); 

                // Savurmadan sonra duruşunu düzeltmesi için bekle
                yield return new WaitForSeconds(2.5f * hizCarpani); 
                
                donguSayaci = 0;
                hizCarpani = Mathf.Clamp(hizCarpani - 0.15f, 0.4f, 1f);
                anim.speed = (1f / hizCarpani); 

                if (bossFazi < 3) 
                {
                    bossFazi++;
                }
            }
        }
    }

    void YelpazeAtesEt()
    {
        if (mermiPrefab == null || atesNoktasi == null) return;

        int mermiAdedi = 5;
        float aci = 60f; 

        if (bossFazi == 2) 
        { 
            mermiAdedi = 8; 
            aci = 360f; 
        }
        else if (bossFazi >= 3) 
        { 
            mermiAdedi = 12; // 3. fazda 360 dereceyi daha yoğun tarar
            aci = 360f; 
        }

        float baslangicAcisi = -aci / 2f;
        float aciArtisi = (aci >= 360f) ? (360f / mermiAdedi) : (aci / (mermiAdedi - 1));

        for (int j = 0; j < mermiAdedi; j++)
        {
            float gecerliAci = (aci >= 360f) ? (aciArtisi * j) : (baslangicAcisi + (aciArtisi * j));
            Quaternion mermiRotasyonu = transform.rotation * Quaternion.Euler(0, gecerliAci, 0);
            Instantiate(mermiPrefab, atesNoktasi.position, mermiRotasyonu);
        }
    }

    IEnumerator DominoKupleriniYarat()
    {
        int hatSayisi = 1; 
        float hatlarArasiAci = 30f; 
        float baslangicAcisi = 0f;

        if (bossFazi == 2) 
        {
            hatSayisi = 3;
            baslangicAcisi = -30f; 
        }
        else if (bossFazi >= 3) 
        {
            // YENİ: 3. Fazda 8 farklı yöne (360 derece) yıldız gibi patlar
            hatSayisi = 8; 
            hatlarArasiAci = 45f; // 360 / 8 = 45 derece aralıklarla
            baslangicAcisi = 0f;
        }

        for (int i = 0; i < 20; i++) 
        {
            for (int h = 0; h < hatSayisi; h++)
            {
                float gecerliAci = baslangicAcisi + (h * hatlarArasiAci);
                Quaternion hatRotasyonu = transform.rotation * Quaternion.Euler(0, gecerliAci, 0);
                Vector3 yon = hatRotasyonu * Vector3.forward;

                Vector3 baslangicNoktasi = transform.position + (yon * 2f); 
                Vector3 spawnNoktasi = baslangicNoktasi + (yon * (i * 1.2f));
                spawnNoktasi.y -= 5f; 

                if (dominoKupPrefab != null)
                {
                    Instantiate(dominoKupPrefab, spawnNoktasi, hatRotasyonu);
                }
            }
            yield return new WaitForSeconds(0.1f); 
        }
    }

    void AlanHasariVeSavurma()
    {
        Collider[] vurulanlar = Physics.OverlapSphere(transform.position, itmeMenzili);
        foreach (Collider c in vurulanlar)
        {
            if (c.CompareTag("Player"))
            {
                PlayerBossController oyuncuKod = c.GetComponent<PlayerBossController>();
                if (oyuncuKod != null && !oyuncuKod.isInvincible)
                {
                    // GERÇEK HASAR BURADA!
                    oyuncuKod.HasarAl(25);

                    Vector3 itmeYonu = (c.transform.position - transform.position).normalized;
                    itmeYonu.y = 0; 
                    oyuncuKod.Firlat(itmeYonu, itmeGucu, 0.5f); 
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 12f);
    }
}