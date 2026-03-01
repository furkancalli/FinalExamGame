using UnityEngine;
using System.Collections; 
using UnityEngine.SceneManagement; // Sahneyi yeniden yüklemek için şart!

public class PlayerBossController : MonoBehaviour
{
    [Header("Ölüm Ekranı Ayarları")]
    public GameObject oldunPaneli; // Yarattığımız paneli buraya sürükleyeceğiz

    [Header("2D Görsel ve Animasyon")]
    public Animator anim; // YENİ: 2D Karakterin Animatörü
    private string gecerliAnimasyon; // Hangi animasyon oynuyor takip edeceğiz

    [Header("Ses Efektleri")]
    public AudioSource oyuncuSesKaynagi;
    public AudioClip kilicSavurmaSesi;

    [Header("Can Ayarları")]
    public int maxCan = 100;
    private int guncelCan;
    public CanBariUI oyuncuCanBariUI;
    public bool isDead = false;

    [Header("Saldırı (Melee) Ayarları")]
    public Transform saldiriNoktasi;     
    public float saldiriMenzili = 1.5f;  
    public float saldiriNoktasiUzakligi = 1.2f; // YENİ: Kılıç karakterden ne kadar uzakta dursun?
    public LayerMask bossKatmani;        
    public int saldiriHasari = 20;       
    public float saldiriHizi = 0.5f;     
    
    private float sonrakiSaldiriZamani = 0f;

    [Header("Hareket Ayarları")]
    public float hareketHizi = 10f;

    [Header("Dash (Atılma) Ayarları")]
    public float dashHizi = 30f;        
    public float dashSuresi = 0.2f;     
    public float dashBeklemeSuresi = 1f;
    
    public bool isInvincible = false; 
    
    private bool isDashing = false;
    private bool isKnockedBack = false; 
    private float sonDashZamani = -100f;

    private Rigidbody rb;
    private Vector3 hareketGirdisi;
    private Camera anaKamera;
    private Vector3 fareninBaktigiYon; // YENİ: Yüzümüzü döneceğimiz yön

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anaKamera = Camera.main;

        guncelCan = maxCan;
        if (oyuncuCanBariUI != null) oyuncuCanBariUI.CaniGuncelle(guncelCan, maxCan);
    }

    void Update()
    {
        if (isDead) return;
        if (isDashing) return; 

        // 1. WASD İle Yürüme
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        hareketGirdisi = new Vector3(moveX, 0f, moveZ).normalized;

        // 2. Fare Yönünü Bul ve Kılıcı Oraya Çek
        FareYonuHesapla();

        // 3. YENİ: 2D Animasyonları Oynat
        AnimasyonlariYonet();

        // 4. DASH (Boşluk Tuşu)
        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= sonDashZamani + dashBeklemeSuresi)
        {
            Vector3 dashYonu = hareketGirdisi != Vector3.zero ? hareketGirdisi : fareninBaktigiYon;
            StartCoroutine(DashRoutine(dashYonu));
        }

        // 5. SALDIRI
        if (Input.GetMouseButtonDown(0) && Time.time >= sonrakiSaldiriZamani)
        {
            Saldir();
            sonrakiSaldiriZamani = Time.time + saldiriHizi;
            
            if (oyuncuSesKaynagi != null && kilicSavurmaSesi != null)
            {
                oyuncuSesKaynagi.PlayOneShot(kilicSavurmaSesi);
            } 
        }
    }

    void FareYonuHesapla()
    {
        Ray isin = anaKamera.ScreenPointToRay(Input.mousePosition);
        Plane hayaliZemin = new Plane(Vector3.up, transform.position);
        
        if (hayaliZemin.Raycast(isin, out float isinMesafesi))
        {
            Vector3 fareninOyundakiYeri = isin.GetPoint(isinMesafesi);
            
            // Farenin karaktere göre hangi yönde olduğunu bul
            fareninBaktigiYon = (fareninOyundakiYeri - transform.position);
            fareninBaktigiYon.y = 0; // Yukarı/Aşağı bakmayı iptal et
            fareninBaktigiYon.Normalize();

            // YENİ: Saldırı noktasını (kılıcı) farenin olduğu tarafa çek (Karakteri döndürme!)
            if (saldiriNoktasi != null)
            {
                saldiriNoktasi.position = transform.position + (fareninBaktigiYon * saldiriNoktasiUzakligi);
            }
        }
        // Oyuncudan farenin olduğu yere kırmızı bir çizgi çizer (Sadece Unity'nin Scene ekranında görünür)
        Debug.DrawRay(transform.position, fareninBaktigiYon * 5f, Color.red);
    }

    // YENİ VE EFSANEVİ 2D ANİMASYON SİSTEMİ
    void AnimasyonlariYonet()
    {
        if (anim == null) return;

        string yon = "down"; // Varsayılan yön

        // Farenin X eksenindeki (sağ/sol) gücü, Z ekseninden (yukarı/aşağı) büyük mü?
        if (Mathf.Abs(fareninBaktigiYon.x) > Mathf.Abs(fareninBaktigiYon.z))
        {
            yon = fareninBaktigiYon.x > 0 ? "right" : "left";
        }
        else
        {
            yon = fareninBaktigiYon.z > 0 ? "up" : "down";
        }

        // Yürüyor muyuz duruyor muyuz?
        bool isMoving = hareketGirdisi.magnitude > 0.1f;
        string oynamasiGerekenAnim = "";

        if (isMoving)
        {
            // İsimleri senin yazdığın listeye göre birleştiriyoruz:
            oynamasiGerekenAnim = "player_walk_" + yon;
        }
        else
        {
            // Senin listende down için sadece "player_idle" yazıyor, diğerleri yönlü.
            if (yon == "down") oynamasiGerekenAnim = "player_idle";
            else oynamasiGerekenAnim = "player_idle_" + yon;
        }

        // Eğer o an çalan animasyon değiştiyse, yenisini çal
        if (gecerliAnimasyon != oynamasiGerekenAnim)
        {
            anim.Play(oynamasiGerekenAnim);
            gecerliAnimasyon = oynamasiGerekenAnim;
        }
    }

    void Saldir()
    {
        Debug.Log("Kılıç Savruldu!");
        Collider[] vurulanDusmanlar = Physics.OverlapSphere(saldiriNoktasi.position, saldiriMenzili, bossKatmani);

        foreach (Collider dusman in vurulanDusmanlar)
        {
            BossManager boss = dusman.GetComponent<BossManager>();
            if (boss != null)
            {
                boss.HasarAl(saldiriHasari);
                Debug.Log("BAM! Boss'a " + saldiriHasari + " hasar verildi!");
            }
        }
    }

    void FixedUpdate()
    {
        if (!isDashing && !isKnockedBack)
        {
            rb.linearVelocity = hareketGirdisi * hareketHizi;
        }
    }

    IEnumerator DashRoutine(Vector3 dashYonu)
    {
        isDashing = true;
        isInvincible = true; 
        rb.linearVelocity = dashYonu * dashHizi;
        yield return new WaitForSeconds(dashSuresi);
        isDashing = false;
        isInvincible = false; 
        sonDashZamani = Time.time;
    }

    public void Firlat(Vector3 yon, float guc, float sure)
    {
        StartCoroutine(KnockbackRoutine(yon, guc, sure));
    }

    IEnumerator KnockbackRoutine(Vector3 yon, float guc, float sure)
    {
        isKnockedBack = true; 
        
        // 1. HAYATİ ÇÖZÜM: Yönün "Yukarı" kısmını sıfırla ki havaya uçmasın! Sadece yerde sürüklensin.
        yon.y = 0; 
        yon.Normalize();

        // Önceki hızı sıfırla, sonra sadece X ve Z (Yatay) ekseninde it
        rb.linearVelocity = Vector3.zero; 
        rb.AddForce(yon * guc, ForceMode.Impulse);

        yield return new WaitForSeconds(sure);

        isKnockedBack = false; 

        // 2. HAYATİ ÇÖZÜM: Süre bitince karakteri havada dondurma! 
        // Sadece sağa/sola kaymasını durdur, ama yerçekimi (Y hızı) çalışmaya devam etsin.
        rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0); 
    }

    public void HasarAl(int hasarMiktari)
    {
        if (isInvincible || isDead) return; 

        guncelCan -= hasarMiktari;
        guncelCan = Mathf.Max(guncelCan, 0); 

        if (oyuncuCanBariUI != null) oyuncuCanBariUI.CaniGuncelle(guncelCan, maxCan);

        if (guncelCan <= 0)
        {
            Olum();
        }
    }


    void Olum()
    {
        if (isDead) return;
        isDead = true;
        
        Debug.Log("OYUNCU ÖLDÜ!");
        
        // Karakterin hareketini ve fiziğini durdur
        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true; 

        // 2D karakterin eğer ölme animasyonu varsa onu çalalım
        if (anim != null) anim.Play("player_idle"); // Buraya varsa "player_die" yazabilirsin

        // ÖLÜM PANELİNİ AÇ!
        if (oldunPaneli != null)
        {
            oldunPaneli.SetActive(true);
        }

        // Fare imlecini görünür yap (Paneldeki butona basabilmek için)
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // Bu fonksiyonu butona bağlayacağız!
    public void TekrarDene()
    {
        // Şu anki sahneyi (Boss sahnesini) en baştan yükle
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}