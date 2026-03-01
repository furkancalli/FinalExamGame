using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Süre Ayarları")]
    public float kalanSure = 60f;
    public TextMeshProUGUI sureText;
    private bool oyunBittiMi = false;

    [Header("UI Panelleri (Arayüz)")]
    public GameObject howToPlayPanel;
    public GameObject winPanel;
    public GameObject losePanel;

    [Header("Sahne ve Geçiş Ayarları")]
    public string openWorldSahneAdi = "OpenWorld";
    public CanvasGroup fadeSistemi;

    [Header("Ses Ayarları")]
    public AudioSource arkaPlanMuzigi;

    public static bool yenidenBasladiMi = false;

    void Start()
    {
        winPanel.SetActive(false);
        losePanel.SetActive(false);

        if (fadeSistemi != null) fadeSistemi.alpha = 0f;

        if (yenidenBasladiMi)
        {
            howToPlayPanel.SetActive(false);
            Time.timeScale = 1f;
            if (arkaPlanMuzigi != null) arkaPlanMuzigi.Play();
        }
        else
        {
            howToPlayPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    void Update()
    {
        if (oyunBittiMi) return;

        if (kalanSure > 0)
        {
            kalanSure -= Time.deltaTime;
            if (sureText != null) sureText.text = "Time: " + Mathf.RoundToInt(kalanSure).ToString();
        }
        else
        {
            Kaybettin();
        }
    }

    public void OyunaBasla()
    {
        howToPlayPanel.SetActive(false);
        Time.timeScale = 1f;
        if (arkaPlanMuzigi != null) arkaPlanMuzigi.Play();
    }

    public void Kazandin()
    {
        if (oyunBittiMi) return;
        oyunBittiMi = true;

        // --- GÜNCEL GÖREV SİSTEMİ ---
        // ARTIK INDEX ARTIRMIYORUZ! 
        // Sadece "Görev bitti, NPC'ye dönüp onay al" işaretini koyuyoruz.
        PlayerPrefs.SetInt("QuestWaitingForReturn", 1);
        PlayerPrefs.Save();
        Debug.Log("<color=yellow>YARIŞ BİTTİ:</color> NPC onayı bekleniyor, Open World'e dönülüyor.");
        // ----------------------------

        winPanel.SetActive(true);
        Time.timeScale = 0f;

        StartCoroutine(KazanmaGecisi());
    }

    public void Kaybettin()
    {
        if (oyunBittiMi) return;
        oyunBittiMi = true;

        losePanel.SetActive(true);
        Time.timeScale = 0f;
        if (arkaPlanMuzigi != null) arkaPlanMuzigi.Stop();
    }

    public void YenidenBasla()
    {
        yenidenBasladiMi = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator KazanmaGecisi()
    {
        yield return new WaitForSecondsRealtime(2f);

        float gecenZaman = 0f;
        while (gecenZaman < 1f)
        {
            gecenZaman += Time.unscaledDeltaTime;
            if (fadeSistemi != null) fadeSistemi.alpha = gecenZaman;
            yield return null;
        }

        yenidenBasladiMi = false;
        Time.timeScale = 1f;

        // Open World sahnesine dön
        SceneManager.LoadScene(openWorldSahneAdi);
    }
}