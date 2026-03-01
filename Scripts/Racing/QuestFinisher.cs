using UnityEngine;
using UnityEngine.SceneManagement; // Sahne yönetimi için şart

public class QuestFinisher : MonoBehaviour
{
    [Header("Ayarlar")]
    [Tooltip("Geri dönülecek ana haritanın adı (Build Settings'tekiyle aynı olmalı)")]
    public string openWorldSceneName = "OpenWorld";

    // 1. YÖNTEM: Bir alana (Trigger) girince görevi bitirmek için
    private void OnTriggerEnter(Collider other)
    {
        // Çarpan objenin tag'i "Player" ise çalışır
        if (other.CompareTag("Player"))
        {
            CompleteQuestAndReturn();
        }
    }

    // 2. YÖNTEM: Bir butona basınca veya başka bir scriptten çağırmak için
    public void CompleteQuestAndReturn()
    {
        // Mevcut ilerlemeyi oku (yoksa 0 kabul et)
        int current = PlayerPrefs.GetInt("GlobalQuestIndex", 0);

        // İlerlemeyi 1 artır
        PlayerPrefs.SetInt("GlobalQuestIndex", current + 1);

        // Veriyi kalıcı olarak kaydet
        PlayerPrefs.Save();

        // Konsola bilgi yazdır (Hata ayıklama için çok önemli)
        Debug.Log("<color=green>GÖREV TAMAMLANDI!</color> Yeni GlobalQuestIndex: " + PlayerPrefs.GetInt("GlobalQuestIndex"));

        // Ana sahneye geri dön
        SceneManager.LoadScene(openWorldSceneName);
    }
}