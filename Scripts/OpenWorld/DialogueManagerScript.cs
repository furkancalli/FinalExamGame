using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
        [Header("Ses Efektleri")]
    public AudioSource dialogueAudioSource; // Yazı sesini çalacak kaynak
    public AudioClip typingSound;           // Çalınacak ses dosyası
    public static DialogueManager Instance; // Diğer scriptlerden kolayca ulaşmak için

    public GameObject dialoguePanel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI messageText;
    public float typingSpeed = 0.05f; // Harf gelme hızı

    private Coroutine typingCoroutine;

    void Awake()
    {
        Instance = this;
        dialoguePanel.SetActive(false); // Başta kapalı olsun
    }

    public void ShowDialogue(string npcName, string message)
    {
        dialoguePanel.SetActive(true);
        nameText.text = npcName;

        // Eğer zaten bir yazı yazılıyorsa onu durdur ve yenisine başla
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeSentence(message));
    }

    IEnumerator TypeSentence(string sentence)
{
    messageText.text = ""; 
    foreach (char letter in sentence.ToCharArray())
    {
        messageText.text += letter; 

        // Her harf eklendiğinde sesi çal
        if (dialogueAudioSource != null && typingSound != null)
        {
            dialogueAudioSource.PlayOneShot(typingSound);
        }

        yield return new WaitForSeconds(typingSpeed); 
    }
}

    public void CloseDialogue()
    {
        // Paneli tamamen gizle
        dialoguePanel.SetActive(false);

        // Eğer o sırada yazı hala yazılıyorsa (Typewriter efekti), onu da durdur
        StopAllCoroutines();
    }
}