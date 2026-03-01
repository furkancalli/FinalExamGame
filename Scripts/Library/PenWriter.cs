using UnityEngine;
using System.Collections;

public class PenWriter : MonoBehaviour
{
    private Vector3 startPos;
    
    [Header("Yazma Ayarları")]
    public float writeSpeed = 20f; // Kalemin hızı (Ne kadar yüksek o kadar telaşlı yazar)
    public float writeRangeX = 0.2f; // Sağa sola ne kadar açılarak yazacak
    public float writeRangeZ = 0.2f; // İleri geri ne kadar hareket edecek

    void Start()
    {
        // Oyun başladığında kolun durduğu o ilk yeri hafızaya al
        startPos = transform.localPosition;
    }

    // StudyGameManager'dan bu fonksiyonu çağıracağız
    public void StartWriting()
    {
        StopAllCoroutines(); // Eğer hala yazıyorsa eski yazmayı kesip yenisine başla
        StartCoroutine(WriteRoutine());
    }

    IEnumerator WriteRoutine()
    {
        // 4 kere rastgele sağa sola hızlıca git-gel yap (Karalama efekti)
        for (int i = 0; i < 4; i++)
        {
            // Orijinal pozisyonun etrafında rastgele yeni bir hedef nokta belirle
            Vector3 randomTarget = startPos + new Vector3(
                Random.Range(-writeRangeX, writeRangeX), 
                0f, // Kalem havaya kalkmasın diye Y eksenini 0 (sabit) tutuyoruz
                Random.Range(-writeRangeZ, writeRangeZ)
            );

            float t = 0;
            Vector3 currentPos = transform.localPosition;
            
            // Hedefe doğru hızlıca kay (Lerp)
            while (t < 1f)
            {
                t += Time.deltaTime * writeSpeed;
                transform.localPosition = Vector3.Lerp(currentPos, randomTarget, t);
                yield return null;
            }
        }

        // Karalama bittikten sonra usulca eski bekleme yerine dön
        float returnT = 0;
        Vector3 lastPos = transform.localPosition;
        while (returnT < 1f)
        {
            returnT += Time.deltaTime * (writeSpeed / 2); // Dönüş biraz daha sakin olsun
            transform.localPosition = Vector3.Lerp(lastPos, startPos, returnT);
            yield return null;
        }
    }
}