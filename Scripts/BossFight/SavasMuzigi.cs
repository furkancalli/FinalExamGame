using UnityEngine;

public class SavasMuzigi : MonoBehaviour
{
    [Header("Müzik Dosyaları")]
    public AudioClip introMuzigi; // Sadece 1 kere çalacak giriş kısmı
    public AudioClip loopMuzigi;  // Sonsuza kadar dönecek asıl savaş kısmı

    private AudioSource introSource;
    private AudioSource loopSource;

    void Start()
    {
        // Kodla iki tane gizli AudioSource (Kaset Çalar) oluşturuyoruz ki kafan karışmasın
        introSource = gameObject.AddComponent<AudioSource>();
        loopSource = gameObject.AddComponent<AudioSource>();

        // 1. Kaset Çalar (İntro) ayarları
        introSource.clip = introMuzigi;
        introSource.playOnAwake = false;

        // 2. Kaset Çalar (Loop) ayarları
        loopSource.clip = loopMuzigi;
        loopSource.loop = true; // Bu sürekli dönecek!
        loopSource.playOnAwake = false;
    }

    public void MuzigiBaslat()
    {
        // Unity'nin içsel ses saati (Çok hassastır, salise bile sekmez)
        double baslamaZamani = AudioSettings.dspTime;
        
        // İntro müziğinin tam uzunluğunu hesapla
        double introSuresi = (double)introMuzigi.samples / introMuzigi.frequency;

        // İntroyu ŞU AN başlat
        introSource.PlayScheduled(baslamaZamani);

        // Loop müziğini, tam intro bittiği saniyeye "KUR" (Zamanla)
        loopSource.PlayScheduled(baslamaZamani + introSuresi);
    }

    public void MuzigiDurdur()
    {
        // Boss ölünce ikisini de susturacağız
        introSource.Stop();
        loopSource.Stop();
    }
}