using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Paneller")]
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;

    public void PlayGame()
    {
        // 1 numaralı sahneyi (oyun sahnesini) yükler
        SceneManager.LoadScene("OpenWorld");
    }

    public void OpenSettings()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void SetVolume(float volume)
    {
        // Tüm oyunun genel sesini hızlıca kontrol eder
        AudioListener.volume = volume;
    }

    public void QuitGame()
    {
        Debug.Log("Oyundan Çıkıldı!"); // Unity editöründe çalıştığını görmek için
        Application.Quit();
    }

    void Awake()
{
    // Oyun her açıldığında tüm kayıtlı verileri siler (Sadece test aşamasında kullan!)
     PlayerPrefs.DeleteAll(); 
}
}