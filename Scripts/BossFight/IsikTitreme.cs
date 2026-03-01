using UnityEngine;

public class IsikTitretme : MonoBehaviour
{
    private Light isik;
    public float minIntensity = 1f;
    public float maxIntensity = 3f;
    public float titremeHizi = 0.1f; // Ne kadar sık titreyecek?

    void Start()
    {
        isik = GetComponent<Light>();
        StartCoroutine(TitretRoutine());
    }

    System.Collections.IEnumerator TitretRoutine()
    {
        while (true)
        {
            // Işığın şiddetini rastgele bir aralıkta değiştir
            isik.intensity = Random.Range(minIntensity, maxIntensity);
            yield return new WaitForSeconds(titremeHizi);
        }
    }
}