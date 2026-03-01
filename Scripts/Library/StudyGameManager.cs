using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class StudyGameManager : MonoBehaviour
{
    [Header("Ses Efektleri & Ayarları")]
    public AudioClip correctSound;
    public AudioClip wrongSound;
    public AudioSource backgroundMusic;

    [Header("UI Elementleri")]
    public GameObject introPanel;
    public MemoryGameManager memoryGameManager;
    public GameObject qtePrefab;
    public Transform canvasTransform;
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI endGameText;

    [Header("Oyun Ayarları")]
    public float shrinkSpeed = 1.5f;
    public int maxMistakes = 3;
    private int currentMistakes = 0;
    private int correctAnswers = 0;

    [Header("Efektler & Görsellik")]
    public CameraShake cameraShake;
    public PenWriter penWriter;
    public Image fadeScreen;
    public float fadeSpeed = 2f;
    public float speedIncreaseAmount = 0.5f;

    [Header("Kitap Objeleri (Renderer & Texture)")]
    public Renderer kitapSolSayfa;
    public Renderer kitapSagSayfa;
    public Renderer kenarSayfa1;
    public Renderer kenarSayfa2;
    public Texture[] kitapSolResimleri;
    public Texture[] kitapSagResimleri;
    public Texture[] kenarSayfa1Resimleri;
    public Texture[] kenarSayfa2Resimleri;

    private int currentPageIndex = 0;
    private GameObject currentQTE;
    private Transform shrinkingRing;
    private string targetKey;
    private bool isWaitingForInput = false;
    private string[] possibleKeys = { "q", "v", "g", "s", "w", "e", "r", "a", "d", "f" };

    void Start()
    {
        if (introPanel != null) introPanel.SetActive(true);
    }

    void Update()
    {
        if (isWaitingForInput && currentQTE != null)
        {
            shrinkingRing.localScale -= Vector3.one * shrinkSpeed * Time.deltaTime;
            if (shrinkingRing.localScale.x <= 0.5f) WrongAnswer("TOO LATE!");
            if (Input.anyKeyDown) CheckInput();
        }
    }

    // --- OYUN MANTIĞI FONKSİYONLARI ---
    void SpawnNewKey()
    {
        targetKey = possibleKeys[Random.Range(0, possibleKeys.Length)];
        currentQTE = Instantiate(qtePrefab, canvasTransform);
        RectTransform rect = currentQTE.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(Random.Range(-660f, 660f), Random.Range(-340f, 340f));
        currentQTE.GetComponentInChildren<TextMeshProUGUI>().text = targetKey.ToUpper();
        shrinkingRing = currentQTE.transform.Find("RingImage");
        shrinkingRing.localScale = new Vector3(2.5f, 2.5f, 2.5f);
        isWaitingForInput = true;
    }

    void CheckInput()
    {
        if (Input.GetKeyDown(targetKey))
        {
            if (shrinkingRing.localScale.x <= 1.2f && shrinkingRing.localScale.x >= 0.8f) CorrectAnswer();
            else WrongAnswer("BAD TIMING!");
        }
        else WrongAnswer("WRONG KEY!");
    }

    void CorrectAnswer()
    {
        isWaitingForInput = false;
        Destroy(currentQTE);
        correctAnswers++;
        feedbackText.text = "GREAT!";
        if (penWriter != null) penWriter.StartWriting();
        StartCoroutine(PopTextRoutine());
        if (backgroundMusic != null && correctSound != null) backgroundMusic.PlayOneShot(correctSound);

        if (correctAnswers % 5 == 0) StartCoroutine(TurnPageRoutine());
        else Invoke("SpawnNewKey", 1f);
    }

    void WrongAnswer(string reason)
    {
        isWaitingForInput = false;
        if (currentQTE != null) Destroy(currentQTE);
        if (!LoseLife(reason)) Invoke("SpawnNewKey", 1f);
    }

    public bool LoseLife(string reason)
    {
        if (cameraShake != null) StartCoroutine(cameraShake.Shake(0.2f, 0.1f));
        currentMistakes++;
        feedbackText.text = reason + " Mistake: " + currentMistakes + "/" + maxMistakes;
        StartCoroutine(PopTextRoutine());
        if (backgroundMusic != null && wrongSound != null) backgroundMusic.PlayOneShot(wrongSound);

        if (currentMistakes >= maxMistakes) { TriggerEndGame(false); return true; }
        return false;
    }

    IEnumerator TurnPageRoutine()
    {
        yield return StartCoroutine(FadeRoutine(1f));
        shrinkSpeed += speedIncreaseAmount;
        if (currentPageIndex < kitapSolResimleri.Length)
        {
            if (kitapSolSayfa != null) kitapSolSayfa.material.mainTexture = kitapSolResimleri[currentPageIndex];
            if (kitapSagSayfa != null) kitapSagSayfa.material.mainTexture = kitapSagResimleri[currentPageIndex];
            if (kenarSayfa1 != null) kenarSayfa1.material.mainTexture = kenarSayfa1Resimleri[currentPageIndex];
            if (kenarSayfa2 != null) kenarSayfa2.material.mainTexture = kenarSayfa2Resimleri[currentPageIndex];
            currentPageIndex++;
        }
        yield return new WaitForSeconds(0.5f);
        if (currentPageIndex >= 3) { feedbackText.text = ""; memoryGameManager.StartMemoryGame(); }
        yield return StartCoroutine(FadeRoutine(0f));
        if (currentPageIndex < 3) SpawnNewKey();
    }

    // --- GÖREV BİTİŞ VE SAHNE GEÇİŞİ ---
    public void TriggerEndGame(bool isWin)
    {
        if (isWin)
        {
            // Index artırma artık OpenWorld'deki NPC diyaloğunun sonunda yapılacak
            PlayerPrefs.SetInt("QuestWaitingForReturn", 1);
            PlayerPrefs.Save();
        }
        StartCoroutine(EndGameRoutine(isWin));
    }

    IEnumerator EndGameRoutine(bool isWin)
    {
        isWaitingForInput = false;
        if (endGameText != null)
        {
            endGameText.gameObject.SetActive(true);
            endGameText.text = isWin ? "STUDY COMPLETE!" : "STUDY FAILED!";
            endGameText.color = isWin ? Color.green : Color.red;
            endGameText.transform.localScale = Vector3.zero;
            float t = 0;
            while (t < 0.5f) { t += Time.deltaTime; endGameText.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * 1.2f, t / 0.5f); yield return null; }
        }
        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(FadeRoutine(1f));
        SceneManager.LoadScene("OpenWorld"); // Open World sahne adın
    }

    IEnumerator FadeRoutine(float targetAlpha)
    {
        float startAlpha = fadeScreen.color.a;
        float timer = 0;
        while (timer < 1f)
        {
            timer += Time.deltaTime * fadeSpeed;
            Color c = fadeScreen.color;
            c.a = Mathf.Lerp(startAlpha, targetAlpha, timer);
            fadeScreen.color = c;
            yield return null;
        }
    }

    public void CloseIntroAndStartGame()
    {
        if (introPanel != null) introPanel.SetActive(false);
        if (backgroundMusic != null && !backgroundMusic.isPlaying) backgroundMusic.Play();
        SpawnNewKey();
    }

    IEnumerator PopTextRoutine()
    {
        feedbackText.transform.localScale = Vector3.one * 1.5f;
        float timer = 0;
        while (timer < 0.2f) { timer += Time.deltaTime; feedbackText.transform.localScale = Vector3.Lerp(Vector3.one * 1.5f, Vector3.one, timer / 0.2f); yield return null; }
    }
}