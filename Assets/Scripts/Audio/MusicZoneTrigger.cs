using UnityEngine;

public class MusicZoneTrigger : MonoBehaviour
{
    private AudioManager audioManager;

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            audioManager.EnterMusicZone();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            audioManager.ExitMusicZone();
        }
    }
}