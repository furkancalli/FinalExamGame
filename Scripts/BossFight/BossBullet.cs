using UnityEngine;

public class BossBullet : MonoBehaviour
{
    public float hiz = 15f;
    public int hasar = 10;
    public Vector3 donmeHizi = new Vector3(300f, 50f, 0f); 

    [Header("Ses Efektleri")]
    public AudioClip firlatmaSesi; // YENİ: Havayı yarma/Fırlatma sesi
    private AudioSource sesKaynagi;

    void Start()
    {
        // Ses kaynağını bul ve mermi doğduğu an sesi patlat
        sesKaynagi = GetComponent<AudioSource>();
        if (sesKaynagi != null && firlatmaSesi != null)
        {
            sesKaynagi.PlayOneShot(firlatmaSesi);
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * hiz;

        Destroy(gameObject, 5f);
    }

    void Update()
    {
        transform.Rotate(donmeHizi * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            PlayerBossController oyuncu = other.GetComponent<PlayerBossController>();
            
            if (oyuncu != null && !oyuncu.isInvincible)
            {
                oyuncu.HasarAl(hasar);
                Destroy(gameObject); 
            }
        }
    }
}