using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool locked;
    public bool keyPickedUp;

    private Animator doorAnim;
    AudioManager audioManager;
    private Collider2D doorTriggerCollider; 
    private Collider2D doorNonTriggerCollider;
    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
    void Start()
    {
        doorAnim = GetComponent<Animator>();

        Collider2D[] colliders = GetComponents<Collider2D>();

        foreach (var col in colliders)
        {
            if (col.isTrigger)
            {
                doorTriggerCollider = col;
            }
            else
            {
                doorNonTriggerCollider = col;
            }
        }
        locked = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Key") && keyPickedUp)
        {
            audioManager.PlaySFX(audioManager.doorUnlock);
            doorAnim.SetTrigger("Open");
            locked = false;
            doorNonTriggerCollider.enabled = false;            
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Key") && keyPickedUp)
        {
            doorAnim.SetTrigger("Close");
            locked = true;
            doorNonTriggerCollider.enabled = true;

            // Key sẽ reset khi chạm vào door
            Key key = collision.gameObject.GetComponent<Key>();
            if (key != null)
            {
                key.ResetKey();
            }
        }
    }
}
