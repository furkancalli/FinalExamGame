using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Ses Efektleri")]
    public AudioSource sesKaynagi;
    public AudioClip carpmaSesi;
    private float sonKahveAlmaZamani = 0f; 
    public float forwardSpeed = 15f; 
    public float sideSpeed = 10f;
    
    // YENİ: Yumuşak geçiş (Smooth) için eklediğimiz değişkenler
    private float currentSideSpeed = 0f; // Arabanın o anki anlık sağ/sol hızı
    public float yatayIvme = 10f; // Sağa/sola ne kadar sürede hızlanıp yavaşlayacağı (Kayganlık hissi)

    // Aşırı Doz Sistemi
    private int kahveSayaci = 0;       
    private bool asiriDoz = false;     
    private float dozSuresi = 0f;     

    // Dokunulmazlık ve Sekme Sistemi
    private float dokunulmazlikSuresi = 0f; 
    private float geriSekmeGucu = 0f;

   void Update()
    {
        // 1. Dokunulmazlık ve Geri Sekme 
        if (dokunulmazlikSuresi > 0f) dokunulmazlikSuresi -= Time.deltaTime;

        if (geriSekmeGucu > 0f)
        {
            transform.Translate(Vector3.back * geriSekmeGucu * Time.deltaTime, Space.World);
            geriSekmeGucu -= Time.deltaTime * 40f; 
        }

        // 2. İleri gidiş
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime, Space.World);

        // 3. Sağ/Sol Yönlendirme Girdisi (A/D)
        float horizontalInput = 0f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) horizontalInput = 1f;
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) horizontalInput = -1f;

        // Aşırı Doz Kontrolü (Yönleri tersine çevirir)
        if (asiriDoz)
        {
            horizontalInput *= -1f; 
            dozSuresi -= Time.deltaTime; 
            if (dozSuresi <= 0) { asiriDoz = false; kahveSayaci = 0; }
        }

        // ----------------------------------------------------
        // YENİ: SMOOTH SAĞ/SOL HAREKETİ
        // ----------------------------------------------------
        // Hedeflenen hız, bastığımız tuşa göre (10 veya -10)
        float targetSideSpeed = horizontalInput * sideSpeed;
        
        // Arabanın anlık hızını, hedeflenen hıza doğru yavaşça (Lerp ile) kaydırıyoruz.
        // Bu, klavyeden elini çekince arabanın anında kazık gibi durmasını engeller, hafif kayarak durur.
        currentSideSpeed = Mathf.Lerp(currentSideSpeed, targetSideSpeed, Time.deltaTime * yatayIvme);

        // Arabayı anlık yumuşatılmış hızla sağa/sola hareket ettir
        transform.Translate(Vector3.right * currentSideSpeed * Time.deltaTime, Space.World);
        // ----------------------------------------------------

        // Kusursuz Görünmez Duvar (Sınırlandırma)
        float sinirliX = Mathf.Clamp(transform.position.x, -4.5f, 4.5f);
        transform.position = new Vector3(sinirliX, transform.position.y, transform.position.z);

        // Direksiyon (Dönüş) Efekti - Artık currentSideSpeed'e bağlı olduğu için dönüş de daha yumuşak olacak
        float hedefAci = (currentSideSpeed / sideSpeed) * 25f; // Yumuşak hıza oranla açıyı hesapla
        Quaternion hedefRotasyon = Quaternion.Euler(0f, hedefAci, 0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, hedefRotasyon, Time.deltaTime * 10f);

        // 4. Manuel Gaz ve Fren Sistemi (W/S)
        float hedefHiz = 15f; 
        if (asiriDoz) { hedefHiz = 50f; }
        else
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) { hedefHiz = 25f; }
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) { hedefHiz = 8f; }
        }

        if (forwardSpeed < hedefHiz) { forwardSpeed += Time.deltaTime * 6f; }
        else if (forwardSpeed > hedefHiz) { forwardSpeed -= Time.deltaTime * 4f; }
    }

   void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle") && dokunulmazlikSuresi <= 0f)
        {
            // YENİ: Çarpma sesini çal (Eğer ses dosyası ve kaynağı bağlandıysa)
            if (sesKaynagi != null && carpmaSesi != null)
            {
                sesKaynagi.PlayOneShot(carpmaSesi); // Sesi bir kere üst üste binmeye izin vererek çal
            }

            forwardSpeed = 0f; 
            geriSekmeGucu = 15f;
            dokunulmazlikSuresi = 1.5f; 
            
            Debug.Log("ÇARPIŞMA! Araba geri sekti ve ses çaldı.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boost"))
        {
            if (Time.time - sonKahveAlmaZamani < 0.1f) return;
            sonKahveAlmaZamani = Time.time; 
            
            kahveSayaci++; 
            if (kahveSayaci >= 3) { asiriDoz = true; dozSuresi = 3f; forwardSpeed = 50f; }
            else { forwardSpeed = 35f; } 
            
            Destroy(other.gameObject); 
        }
        else if (other.CompareTag("FinishPoint"))
        {
            FindObjectOfType<GameManager>().Kazandin();
        }
    }
}