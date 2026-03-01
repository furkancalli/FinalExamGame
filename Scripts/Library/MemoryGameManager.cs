using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MemoryGameManager : MonoBehaviour
{
    [Header("UI Panelleri")]
    public GameObject memoryIntroPanel; // <-- YENİ EKLEME: Hafıza oyunu bilgi ekranı
    // ... (diğer paneller)

    [Header("Eski Oyundan Gelenler")]
    public TextMeshProUGUI feedbackText; // Ekrana yazılacak ortak yazı
    public StudyGameManager studyManager; // Canımızı azaltacak olan patron kod

    [Header("UI Panelleri")]
    public GameObject memoryGamePanel;
    public GameObject grid2x2;
    public GameObject grid3x3;
    public TextMeshProUGUI sentenceDisplayText; // <-- YENİ EKLEME: Yukarıdaki Cümle Barı

    [Header("Kart Resimleri (Image)")]
    public Image[] cards2x2;
    public Image[] cards3x3;

    [Header("Kart Yazıları (TextMeshPro)")]
    public TextMeshProUGUI[] texts2x2; 
    public TextMeshProUGUI[] texts3x3; 

    [Header("Aşama 1 (2x2)")]
    public string[] stage1Words; 
    public int[] stage1Pattern;  

    [Header("Aşama 2 (3x3)")]
    public string[] stage2Words; 
    public int[] stage2Pattern;  

    [Header("Aşama 3 (3x3)")]
    public string[] stage3Words; 
    public int[] stage3Pattern;  

    [Header("Renkler")]
    public Color normalColor = Color.white;
    public Color highlightColor = Color.green;

    private List<int> currentPattern = new List<int>();
    private int playerClickIndex = 0;
    private int currentStage = 1; 
    private bool canPlayerClick = false;

    public void StartMemoryGame()
    {
        memoryGamePanel.SetActive(true);
        
        // Oyunu hemen başlatmak yerine önce bilgi panelini aç!
        if (memoryIntroPanel != null)
        {
            memoryIntroPanel.SetActive(true);
        }
        else
        {
            StartStage(1); // Eğer panel koymayı unutursak oyun çökmesin, direkt başlasın
        }
    }

    void StartStage(int stage)
    {
        currentStage = stage;
        playerClickIndex = 0;
        currentPattern.Clear();
        canPlayerClick = false;
        
        // <-- YENİ EKLEME: Tur başlarken ekranın üstündeki yazıyı temizle
        if (sentenceDisplayText != null) sentenceDisplayText.text = ""; 

        if (stage == 1) {
            grid2x2.SetActive(true);
            grid3x3.SetActive(false);
            for (int i = 0; i < 4; i++) { if(i < stage1Words.Length) texts2x2[i].text = stage1Words[i]; }
            currentPattern.AddRange(stage1Pattern);
        }
        else if (stage == 2) {
            grid2x2.SetActive(false);
            grid3x3.SetActive(true);
            for (int i = 0; i < 9; i++) { if(i < stage2Words.Length) texts3x3[i].text = stage2Words[i]; }
            currentPattern.AddRange(stage2Pattern);
        }
        else if (stage == 3) {
            grid2x2.SetActive(false);
            grid3x3.SetActive(true);
            for (int i = 0; i < 9; i++) { if(i < stage3Words.Length) texts3x3[i].text = stage3Words[i]; }
            currentPattern.AddRange(stage3Pattern);
        }

        StartCoroutine(ShowPatternRoutine(stage == 1 ? cards2x2 : cards3x3));
    }

    IEnumerator ShowPatternRoutine(Image[] activeCards)
    {
        yield return new WaitForSeconds(1f); 

        foreach (int cardIndex in currentPattern)
        {
            activeCards[cardIndex].color = highlightColor; 
            yield return new WaitForSeconds(0.6f); 
            activeCards[cardIndex].color = normalColor; 
            yield return new WaitForSeconds(0.2f); 
        }

        canPlayerClick = true; 
    }

    public void OnCardClicked(int clickedCardID)
    {
        if (!canPlayerClick) return;

        if (clickedCardID == currentPattern[playerClickIndex])
        {
            playerClickIndex++;
            StartCoroutine(FlashCardColor(clickedCardID, highlightColor)); 

            string addedWord = currentStage == 1 ? texts2x2[clickedCardID].text : texts3x3[clickedCardID].text;
            sentenceDisplayText.text += addedWord + " ";

            // DOĞRU BİLDİ: Ekrana yazdır
            StartCoroutine(ShowFeedbackRoutine("Great!", Color.green));

            if (playerClickIndex >= currentPattern.Count)
            {
                LevelComplete();
            }
        }
        else
        {
            // YANLIŞ TIKLADI!
            canPlayerClick = false;
            StartCoroutine(FlashCardColor(clickedCardID, Color.red)); 
            
            bool isGameOver = false;

            // StudyGameManager'daki ortak can azaltma kodumuzu çalıştırıyoruz!
            if(studyManager != null) 
            {
                isGameOver = studyManager.LoseLife("WRONG CARD!"); 
            }

            // Eğer canı sıfırlanıp "Game Over" OLMADIYSA, hafıza turunu baştan başlat.
            if (!isGameOver)
            {
                Invoke("RestartCurrentStage", 1.5f); 
            }
        }
    }

    IEnumerator FlashCardColor(int cardID, Color flashColor)
    {
        Image[] activeCards = currentStage == 1 ? cards2x2 : cards3x3;
        activeCards[cardID].color = flashColor;
        yield return new WaitForSeconds(0.3f);
        activeCards[cardID].color = normalColor;
    }

    void RestartCurrentStage() { StartStage(currentStage); }

    void LevelComplete()
    {
        canPlayerClick = false;
        if (currentStage == 1) Invoke("StartStage2", 1f);
        else if (currentStage == 2) Invoke("StartStage3", 1f);
        else if (currentStage == 3) 
        {
            // OYUN BİTTİ - KAZANDI SİNEMATİĞİNİ TETİKLE!
            if(studyManager != null) 
            {
                studyManager.TriggerEndGame(true);
            }
        }
    }

    void StartStage2() { StartStage(2); }
    void StartStage3() { StartStage(3); }

    IEnumerator ShowFeedbackRoutine(string message, Color color)
    {
        if(feedbackText != null)
        {
            feedbackText.color = color;
            feedbackText.text = message;
            yield return new WaitForSeconds(1f);
            feedbackText.text = "";
        }
    }
    // Oyuncu Memory Intro paneline tıklayınca bu çalışacak
    public void CloseIntroAndStartMemoryGame()
    {
        if (memoryIntroPanel != null) 
        {
            memoryIntroPanel.SetActive(false); // Paneli gizle
        }
        
        StartStage(1); // Ve kartları yakmaya başla!
    }
}