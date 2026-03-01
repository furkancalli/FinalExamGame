using UnityEngine;
using UnityEngine.UI; // UI kullanmak için şart

public class CanBariUI : MonoBehaviour
{
    [Header("Bar Görselleri")]
    public Image anaBar;       // Kırmızı olan
    public Image gecikmeliBar; // Sarı/Beyaz olan

    [Header("Ayarlar")]
    public float erimeHizi = 2f; // Sarı barın kırmızıya yetişme hızı

    private float hedefDoluluk = 1f;

    // Bu fonksiyonu dışarıdan (BossManager'dan) çağıracağız
    public void CaniGuncelle(int guncelCan, int maxCan)
    {
        // 500 canın 250'si kaldıysa, doluluk oranı 0.5 olur
        hedefDoluluk = (float)guncelCan / maxCan;
        
        // Kırmızı bar anında düşer
        anaBar.fillAmount = hedefDoluluk; 
    }

    void Update()
    {
        // Sarı bar, kırmızı bara (hedefDoluluk) doğru yavaşça erir
        if (gecikmeliBar.fillAmount > hedefDoluluk)
        {
            gecikmeliBar.fillAmount = Mathf.Lerp(gecikmeliBar.fillAmount, hedefDoluluk, Time.deltaTime * erimeHizi);
        }
    }
}