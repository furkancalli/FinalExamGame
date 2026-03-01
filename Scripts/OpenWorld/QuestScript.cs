using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class QuestScript : MonoBehaviour
{
    [System.Serializable]
    public class DialogueLine
    {
        public string characterName;
        [TextArea(3, 10)]
        public string sentence;
    }

    [Header("UI References")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public GameObject skipPrompt;
    public GameObject questMarker;

    [Header("Etkileşim Yazısı")]
    public GameObject interactCanvas; 

    [Header("Ses Efektleri")]
    public AudioSource dialogueAudioSource; // <-- SES İÇİN YENİ EKLENDİ
    public AudioClip typingSound;           // <-- SES İÇİN YENİ EKLENDİ

    [Header("Dialogue Content")]
    public DialogueLine[] startQuestLines;
    public DialogueLine[] endQuestLines;
    public DialogueLine[] notReadyLines;

    [Header("Quest Settings")]
    public int myQuestIndex;
    public string questSceneName;

    private int index;
    private bool isPlayerNearby = false;
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    private bool isDialogueActive = false;

    void Start()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (skipPrompt != null) skipPrompt.SetActive(false);
        if (interactCanvas != null) interactCanvas.SetActive(false);
        RefreshQuestStatus();
    }

    void OnEnable() { RefreshQuestStatus(); }

    public void RefreshQuestStatus()
    {
        int currentGlobalIndex = PlayerPrefs.GetInt("GlobalQuestIndex", 0);
        if (questMarker != null)
        {
            questMarker.SetActive(currentGlobalIndex == myQuestIndex);
        }
    }

    void Update()
    {
        if (isPlayerNearby)
        {
            if (!isDialogueActive && Input.GetKeyDown(KeyCode.E))
            {
                StartDialogue();
            }
            else if (isDialogueActive && Input.GetKeyDown(KeyCode.Space))
            {
                if (isTyping) FinishSentenceInstantly();
                else NextSentence();
            }
        }
    }

    void StartDialogue()
    {
        if (interactCanvas != null) interactCanvas.SetActive(false);
        index = 0;
        isDialogueActive = true;
        dialoguePanel.SetActive(true);
        if (skipPrompt != null) skipPrompt.SetActive(true);
        typingCoroutine = StartCoroutine(TypeSentence());
    }

    void NextSentence()
    {
        DialogueLine[] currentLines = GetCurrentDialogueArray();

        if (index < currentLines.Length - 1)
        {
            index++;
            typingCoroutine = StartCoroutine(TypeSentence());
        }
        else
        {
            int currentGlobalIndex = PlayerPrefs.GetInt("GlobalQuestIndex", 0);
            int isWaiting = PlayerPrefs.GetInt("QuestWaitingForReturn", 0);

            if (currentGlobalIndex == myQuestIndex)
            {
                if (isWaiting == 1)
                {
                    PlayerPrefs.SetInt("GlobalQuestIndex", currentGlobalIndex + 1);
                    PlayerPrefs.SetInt("QuestWaitingForReturn", 0);
                    PlayerPrefs.Save();
                }
                else if (string.IsNullOrEmpty(questSceneName) || questSceneName == SceneManager.GetActiveScene().name)
                {
                    PlayerPrefs.SetInt("GlobalQuestIndex", currentGlobalIndex + 1);
                    PlayerPrefs.Save();
                }
                else
                {
                    SavePlayerPosition();
                    SceneManager.LoadScene(questSceneName);
                    return;
                }
            }
            CloseDialogue();
            UpdateAllNPCs();
        }
    }

    void UpdateAllNPCs()
    {
        QuestScript[] allNPCs = FindObjectsOfType<QuestScript>();
        foreach (QuestScript npc in allNPCs)
        {
            npc.RefreshQuestStatus();
        }
    }

    DialogueLine[] GetCurrentDialogueArray()
    {
        int currentGlobalIndex = PlayerPrefs.GetInt("GlobalQuestIndex", 0);
        int isWaiting = PlayerPrefs.GetInt("QuestWaitingForReturn", 0);
        if (currentGlobalIndex == myQuestIndex && isWaiting == 1) return endQuestLines;
        if (currentGlobalIndex > myQuestIndex) return endQuestLines;
        if (currentGlobalIndex == myQuestIndex) return startQuestLines;
        return notReadyLines;
    }

    // --- SES SİSTEMİ BURAYA EKLENDİ ---
    IEnumerator TypeSentence()
    {
        isTyping = true;
        DialogueLine[] currentLines = GetCurrentDialogueArray();
        
        if (currentLines.Length > 0)
        {
            nameText.text = currentLines[index].characterName;
            dialogueText.text = "";
            
            int charCount = 0; // Ses kontrolü için sayaç

            foreach (char letter in currentLines[index].sentence.ToCharArray())
            {
                dialogueText.text += letter;
                charCount++;

                // Her 2 harfte bir sesi çal (Makine tüfeği gibi olmasın diye)
                if (charCount % 2 == 0 && dialogueAudioSource != null && typingSound != null)
                {
                    dialogueAudioSource.pitch = Random.Range(0.9f, 1.1f);
                    dialogueAudioSource.PlayOneShot(typingSound);
                }

                yield return new WaitForSeconds(0.04f);
            }
        }
        isTyping = false;
    }

    void FinishSentenceInstantly()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        dialogueText.text = GetCurrentDialogueArray()[index].sentence;
        isTyping = false;
    }

    void CloseDialogue()
    {
        isDialogueActive = false;
        dialoguePanel.SetActive(false);
        if (skipPrompt != null) skipPrompt.SetActive(false);
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        if (isPlayerNearby && interactCanvas != null) interactCanvas.SetActive(true);

        foreach (QuestScript npc in FindObjectsOfType<QuestScript>())
        {
            npc.RefreshQuestStatus();
        }
    }

    void SavePlayerPosition()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerPrefs.SetFloat("SavedX", player.transform.position.x);
            PlayerPrefs.SetFloat("SavedY", player.transform.position.y);
            PlayerPrefs.SetFloat("SavedZ", player.transform.position.z);
            PlayerPrefs.SetInt("HasSavedPosition", 1);
            PlayerPrefs.Save();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            if (interactCanvas != null && !isDialogueActive) interactCanvas.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            if (interactCanvas != null) interactCanvas.SetActive(false);
            CloseDialogue();
        }
    }
}