using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement; // Sahne degisimi icin gerekli

public class NPCDialogue : MonoBehaviour
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

    [Header("Settings")]
    public DialogueLine[] conversation;
    public float typingSpeed = 0.04f;
    public AudioClip typingSound;

    [Header("Quest Settings")]
    public bool isQuestNPC = false;        // Bu NPC gorev veriyor mu?
    public string questSceneName;          // Gidilecek sahnenin adi

    private int index;
    private bool isPlayerNearby = false;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    void Start()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (skipPrompt != null) skipPrompt.SetActive(false);
    }

    void Update()
    {
        if (isPlayerNearby)
        {
            if (!dialoguePanel.activeSelf && Input.GetKeyDown(KeyCode.E))
            {
                StartDialogue();
            }
            else if (dialoguePanel.activeSelf && Input.GetKeyDown(KeyCode.Space))
            {
                if (isTyping) FinishSentenceInstantly();
                else NextSentence();
            }
        }
    }

    void StartDialogue()
    {
        index = 0;
        dialoguePanel.SetActive(true);
        if (skipPrompt != null) skipPrompt.SetActive(true);
        typingCoroutine = StartCoroutine(TypeSentence());
    }

    void NextSentence()
    {
        if (index < conversation.Length - 1)
        {
            index++;
            typingCoroutine = StartCoroutine(TypeSentence());
        }
        else
        {
            EndDialogue();
        }
    }

    IEnumerator TypeSentence()
{
    isTyping = true;
    nameText.text = conversation[index].characterName;
    dialogueText.text = "";

    foreach (char letter in conversation[index].sentence.ToCharArray())
    {
        dialogueText.text += letter;

        // NPC objesindeki AudioSource'u kullanarak sesi çal
        AudioSource audio = GetComponent<AudioSource>();
        if (audio != null && typingSound != null)
        {
            audio.PlayOneShot(typingSound);
        }

        yield return new WaitForSeconds(typingSpeed);
    }
    isTyping = false;
}

    void FinishSentenceInstantly()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        dialogueText.text = conversation[index].sentence;
        isTyping = false;
    }

    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        if (skipPrompt != null) skipPrompt.SetActive(false);

        // Gorev NPC'si ise sahne yukle
        if (isQuestNPC && !string.IsNullOrEmpty(questSceneName))
        {
            SceneManager.LoadScene(questSceneName);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) isPlayerNearby = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            EndDialogue();
        }
    }
}