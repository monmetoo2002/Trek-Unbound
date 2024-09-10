using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    [SerializeField] private GameObject dialogueCanvas;
    [SerializeField] private TMP_Text speakerText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Image portraitImage;
    [SerializeField] private Button continueButton;

    [SerializeField] private string[] speaker;
    [SerializeField][TextArea] private string[] dialogueWords;
    [SerializeField] private Sprite[] portrait;
    [SerializeField] private float typingSpeed = 0.05f;

    private bool playerInRange;
    private int step;
    private bool isTyping;
    private bool isFirstTalk = true;

    private void Start()
    {
        continueButton.onClick.AddListener(ContinueDialogue);
        dialogueCanvas.SetActive(false);
    }

    void Update()
    {
        if (Input.GetButtonDown("Interact") && playerInRange)
        {
            if (!dialogueCanvas.activeSelf && !isFirstTalk)
            {
                StartDialogue();
            }
            else
            {
                ContinueDialogue();
            }
        }
    }

    public void StartDialogue()
    {
        step = 0;

        dialogueCanvas.SetActive(true);
        //FindObjectOfType<Movement>().canMove = false; // Vô hiệu hóa di chuyển
        DisplayNextDialogue();
    }

    public void ContinueDialogue()
    {
        if (isTyping)
        {
            CompleteTyping();
        }
        else if (step < speaker.Length)
        {
            DisplayNextDialogue();
        }
        else
        {
            EndDialogue();
        }
    }

    private void DisplayNextDialogue()
    {
        dialogueCanvas.SetActive(true);
        speakerText.text = speaker[step];
        portraitImage.sprite = portrait[step];
        StartCoroutine(TypeDialogue(dialogueWords[step]));
        step++;
    }

    private IEnumerator TypeDialogue(string dialogue)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char letter in dialogue.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }

    private void CompleteTyping()
    {
        StopAllCoroutines();
        dialogueText.text = dialogueWords[step - 1];
        isTyping = false;
    }

    private void EndDialogue()
    {
        dialogueCanvas.SetActive(false);
        //FindObjectOfType<Movement>().canMove = true; // Cho phép di chuyển lại
        isFirstTalk = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            playerInRange = true;
            if(isFirstTalk) 
            {
                StartDialogue();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            playerInRange = false;
            if(dialogueCanvas != null)
            {
                EndDialogue();
            }
        }
    }

    public bool IsDialogueActive()
    {
        return dialogueCanvas.activeSelf;
    }
}