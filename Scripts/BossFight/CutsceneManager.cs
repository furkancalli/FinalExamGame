using UnityEngine;
using TMPro; 
using System.Collections;

public class CutsceneManager : MonoBehaviour
{
    [Header("Kameralar ve Açılar")]
    public KameraTakip anaKameraKodu; 
    public Transform bossKameraAcisi;
    public Transform oyuncuKameraAcisi;

    [Header("UI (Arayüz)")]
    public GameObject altyaziPaneli; 
    public TextMeshProUGUI altyaziText;
    public GameObject kontrollerPaneli; 
    
    [Header("Karakterler")]
    public BossAI bossYapayZeka;
    public PlayerBossController oyuncuKontrol;

    [Header("Diyaloglar ve Efekt")]
    public string bossSozu = "Demek buraya kadar gelebildin... Küstah fare!";
    public string oyuncuSozu = "Çok konuşuyorsun. Kılıcım seninle konuşacak!";
    public float yaziHizi = 0.05f; 

    [Header("Sesler ve Müzikler")]
    public AudioSource sesKaynagi; // Daktilo sesini çalacak alet
    public AudioClip daktiloSesi;  // Bip bip sesi
    public SavasMuzigi savasMuzigiKontrol; // YENİDEN EKLENDİ: Savaş müziği yöneticimiz!

    void Start()
    {
        Animator bossAnim = bossYapayZeka.GetComponent<Animator>();
        if (bossAnim != null) bossAnim.Play("Idle"); 

        if (kontrollerPaneli != null) kontrollerPaneli.SetActive(false);

        StartCoroutine(CutsceneOynat());
    }

    IEnumerator CutsceneOynat()
    {
        // 1. KAVGAYI DURDUR VE UI'I HAZIRLA
        bossYapayZeka.enabled = false;
        oyuncuKontrol.enabled = false; 
        anaKameraKodu.enabled = false; 
        
        if (altyaziPaneli != null) altyaziPaneli.SetActive(true);
        
        // 2. BOSS'A ODAKLAN VE YAZ
        Camera.main.transform.position = bossKameraAcisi.position;
        Camera.main.transform.rotation = bossKameraAcisi.rotation;
        
        yield return StartCoroutine(DaktiloYazisi(bossSozu));
        yield return new WaitForSeconds(1.5f); 

        // 3. OYUNCUYA ODAKLAN VE YAZ
        Camera.main.transform.position = oyuncuKameraAcisi.position;
        Camera.main.transform.rotation = oyuncuKameraAcisi.rotation;
        
        yield return StartCoroutine(DaktiloYazisi(oyuncuSozu));
        yield return new WaitForSeconds(1.5f); 

        // 4. SİNEMATİK BİTTİ
        altyaziText.text = ""; 
        if (altyaziPaneli != null) altyaziPaneli.SetActive(false);
        
        anaKameraKodu.enabled = true; 
        yield return new WaitForSeconds(1.5f);

        // 5. KONTROLLERİ GÖSTER VE TIKLAMA BEKLE
        if (kontrollerPaneli != null)
        {
            kontrollerPaneli.SetActive(true); 
            yield return new WaitUntil(() => Input.anyKeyDown || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1));
            kontrollerPaneli.SetActive(false); 
        }

        // ==========================================
        // 6. İŞTE KAYBOLAN SATIR: SAVAŞ MÜZİĞİNİ BAŞLAT!
        if (savasMuzigiKontrol != null) 
        {
            savasMuzigiKontrol.MuzigiBaslat();
        }
        else
        {
            Debug.LogWarning("Savaş müziği kontrolcüsü atanmamış!");
        }
        // ==========================================

        // 7. SAVAŞ BAŞLASIN!
        bossYapayZeka.enabled = true;
        oyuncuKontrol.enabled = true;
    }

    IEnumerator DaktiloYazisi(string metin)
    {
        altyaziText.text = ""; 
        foreach (char harf in metin.ToCharArray())
        {
            altyaziText.text += harf; 
            
            // Boşluk değilse bip sesini çal
            if (harf != ' ' && sesKaynagi != null && daktiloSesi != null)
            {
                sesKaynagi.PlayOneShot(daktiloSesi);
            }
            
            yield return new WaitForSeconds(yaziHizi); 
        }
    }
}