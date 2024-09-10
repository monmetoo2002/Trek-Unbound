using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private GameObject speechBubble;

    private bool playerInRange;

    private void Awake()
    {
        playerInRange = false;
        speechBubble.SetActive(false);
    }

    private void Update()
    {
        if (playerInRange)
        {
            speechBubble.SetActive(true);
        }
        else
        {
            speechBubble.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            playerInRange = true;            
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            playerInRange = false;
        }
    }
}
