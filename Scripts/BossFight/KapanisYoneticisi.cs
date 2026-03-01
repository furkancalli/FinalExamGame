using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class KapanisYoneticisi : MonoBehaviour
{
    [Header("Ses Efektleri")]
    public AudioSource sesKaynagi; // Sesi çalacak olan kaset çalar
    public AudioClip daktiloSesi;  // İndirdiğin bip sesi

    [Header("Hedef Karakterler")]
    public Transform bossTransform;   
    public Transform oyuncuTransform; 

    [Header("Kameralar ve Açılar")]
    public KameraTakip anaKameraKodu;
    public Transform bossOluKameraAcisi; 
    public Transform oyuncuKameraAcisi;  

    [Header("Arayüz (UI)")]
    public GameObject altyaziPaneli;
    public TextMeshProUGUI altyaziText;
    public Image siyahPerde;
    public GameObject oyunBittiYazisi;

    [Header("Diyaloglar")]
    public string bossSonSozu = "Beni yendin... Ama asıl karanlık yeni başlıyor...";
    public string oyuncuSonSozu = "Karanlıktan korkmuyorum. Göster bana!";
    public float yaziHizi = 0.05f;

    [Header("Sahne Geçişi")]
    public string acilacakSahneAdi = "OpenWorldScene"; 

    // YENİ: Çift çalışmayı engelleyen kilit!
    private bool kapanisBasladi = false; 

    void Start()
    {
        // Sadece siyah perdeyi ve oyun bitti yazısını gizle, alt yazı paneline KARIŞMA!
        if (siyahPerde != null) siyahPerde.gameObject.SetActive(false);
        if (oyunBittiYazisi != null) oyunBittiYazisi.SetActive(false);
    }

    public void KapanisiBaslat()
    {
        // EĞER KAPANIŞ ZATEN BAŞLADIYSA, GELEN DİĞER EMRİ İPTAL ET!
        if (kapanisBasladi) return; 
        
        kapanisBasladi = true; // Kapıyı kilitle
        StartCoroutine(KapanisSekansi());
    }

    IEnumerator KapanisSekansi()
    {
        yield return new WaitForSeconds(3f);

        anaKameraKodu.enabled = false;
        FindObjectOfType<PlayerBossController>().enabled = false;

        siyahPerde.gameObject.SetActive(true);
        siyahPerde.color = new Color(0, 0, 0, 0.5f); 
        altyaziPaneli.SetActive(true);

        Camera.main.transform.position = bossOluKameraAcisi.position;
        Camera.main.transform.LookAt(bossTransform.position + Vector3.up * 1f); 
        
        yield return StartCoroutine(DaktiloYazisi(bossSonSozu));
        yield return new WaitForSeconds(2f);

        Camera.main.transform.position = oyuncuKameraAcisi.position;
        Camera.main.transform.LookAt(oyuncuTransform.position + Vector3.up * 1f);
        
        yield return StartCoroutine(DaktiloYazisi(oyuncuSonSozu));
        yield return new WaitForSeconds(2f);

        altyaziPaneli.SetActive(false); 
        float alpha = 0.5f;
        while (alpha < 1f)
        {
            alpha += Time.deltaTime; 
            siyahPerde.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        oyunBittiYazisi.SetActive(true);
        yield return new WaitForSeconds(3f); 

        SceneManager.LoadScene(acilacakSahneAdi);
    }

    IEnumerator DaktiloYazisi(string metin)
    {
        altyaziText.text = "";
        foreach (char harf in metin.ToCharArray())
        {
            altyaziText.text += harf;
            
            // Eğer harf boşluk değilse ve ses eklenmişse o "bip" sesini çal!
            if (harf != ' ' && sesKaynagi != null && daktiloSesi != null)
            {
                // Sesi üst üste binmeden çalması için biraz perdesini (pitch) rastgele yapabilirsin ama şimdilik düz çalalım
                sesKaynagi.PlayOneShot(daktiloSesi);
            }

            yield return new WaitForSeconds(yaziHizi);
        }
    }
}