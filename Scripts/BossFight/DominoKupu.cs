using UnityEngine;

public class DominoKupu : MonoBehaviour
{
    [Header("Çıkış Ayarları")]
    public float yukselmeHizi = 15f; 
    public float cikisMiktari = 2f; 
    
    [Header("Ses Efektleri")]
    public AudioClip cikmaSesi; // YENİ: Kalem çıkma sesi
    private AudioSource sesKaynagi;

    private Vector3 hedefPozisyon;
    private bool hasarVerdi = false; 

    void Start()
    {
        // Ses kaynağını bul ve sesi çal
        sesKaynagi = GetComponent<AudioSource>();
        if (sesKaynagi != null && cikmaSesi != null)
        {
            sesKaynagi.PlayOneShot(cikmaSesi); // Çıkarken 1 kere çal
        }

        hedefPozisyon = transform.position + new Vector3(0, cikisMiktari, 0);
        Destroy(gameObject, 2.5f); 
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, hedefPozisyon, Time.deltaTime * yukselmeHizi);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!hasarVerdi && other.CompareTag("Player"))
        {
            PlayerBossController oyuncu = other.GetComponent<PlayerBossController>();
            if (oyuncu != null && !oyuncu.isInvincible)
            {
                oyuncu.HasarAl(15); 
                Vector3 firlatmaYonu = (other.transform.position - transform.position).normalized;
                oyuncu.Firlat(firlatmaYonu, 15f, 0.3f); 
                hasarVerdi = true;
            }
        }
    }
}