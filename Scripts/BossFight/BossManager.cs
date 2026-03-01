using UnityEngine;
using System.Collections;

public class BossManager : MonoBehaviour
{
    public KapanisYoneticisi kapanisYonetmeni;
    public SavasMuzigi savasMuzigiKontrol;

    [Header("Ses Efektleri")]
    public AudioSource bossSesKaynagi;
    public AudioClip kagitHasarSesi;
    public AudioClip yereYigilmaSesi;

    [Header("Can Ayarları")]
    public int maxCan = 500;
    private int guncelCan;

    [Header("UI Bağlantısı")]
    public CanBariUI bossCanBariUI;

    private MeshRenderer bossRenderer;
    private Color orijinalRenk;
    private Animator anim;

    // Boss'un 2 kere ölmesini engelleyecek kilit
    private bool isDead = false;

    void Start()
    {
        guncelCan = maxCan;

        bossRenderer = GetComponent<MeshRenderer>();
        if (bossRenderer != null) orijinalRenk = bossRenderer.material.color;

        anim = GetComponent<Animator>();

        // Oyun başlarken barı fulleyelim
        if (bossCanBariUI != null)
        {
            bossCanBariUI.CaniGuncelle(guncelCan, maxCan);
        }
    }

    public void HasarAl(int hasarMiktari)
    {
        if (isDead) return;

        guncelCan -= hasarMiktari;
        guncelCan = Mathf.Max(guncelCan, 0);

        if (bossSesKaynagi != null && kagitHasarSesi != null)
        {
            bossSesKaynagi.PlayOneShot(kagitHasarSesi);
        }

        if (bossCanBariUI != null)
        {
            bossCanBariUI.CaniGuncelle(guncelCan, maxCan);
        }

        StartCoroutine(HasarEfektiRoutine());

        if (guncelCan <= 0)
        {
            Olum();
        }
    }

    IEnumerator HasarEfektiRoutine()
    {
        if (bossRenderer != null)
        {
            bossRenderer.material.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            bossRenderer.material.color = orijinalRenk;
        }
    }

    void Olum()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("<color=green>BOSS ÖLDÜ!</color> Görev indexi güncelleniyor...");

        // --- GÖREV SİSTEMİ ENTEGRASYONU ---
        // Boss öldüğünde "Onay Bekleme" moduna alıyoruz. 
        // OpenWorld'e dönünce NPC ile konuşunca Index artacak.
        PlayerPrefs.SetInt("QuestWaitingForReturn", 1);
        PlayerPrefs.Save();
        // ----------------------------------

        if (bossSesKaynagi != null && yereYigilmaSesi != null)
        {
            bossSesKaynagi.PlayOneShot(yereYigilmaSesi);
        }

        if (savasMuzigiKontrol != null) savasMuzigiKontrol.MuzigiDurdur();

        if (anim != null) anim.SetBool("isDead", true);

        // Yapay zekayı durdur
        if (GetComponent<BossAI>() != null)
        {
            GetComponent<BossAI>().StopAllCoroutines();
            GetComponent<BossAI>().enabled = false;
        }

        // Kapanış sinematiğini başlat!
        if (kapanisYonetmeni != null)
        {
            kapanisYonetmeni.KapanisiBaslat();
        }
    }
}