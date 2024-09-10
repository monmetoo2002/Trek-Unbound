using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Checkpoint : MonoBehaviour
{
    private DieControl dieControl;
    public Transform respawnPoint;

    private void Awake()
    {
        dieControl = FindObjectOfType<DieControl>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (dieControl != null && respawnPoint != null)
            {
                dieControl.UpdateCheckPoint(respawnPoint.position);
                
                if (GameManager.instance != null)
                {
                    GameManager.instance.SaveGemCount();
                    GameManager.instance.UpdateLastCheckpoint(respawnPoint.position);
                }               
            }
            
        }
    }
}