using UnityEngine;
using System.Collections;

public class ArrowDash : MonoBehaviour
{
    [SerializeField] private float respawnTime = 5f;
    private SpriteRenderer spriteRenderer;
    private Collider2D arrowCollider;
    AudioManager audioManager;
    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        arrowCollider = GetComponent<Collider2D>();
    }

    private void Start()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Movement playerMovement = collision.GetComponent<Movement>();
            if (playerMovement != null)
            {
                // Thêm âm thanh, effect
                audioManager.PlaySFX(audioManager.arrowDashPickup);
                playerMovement.ResetDash();
                StartCoroutine(Respawn());
            }
        }
    }

    private IEnumerator Respawn()
    {
        // Ẩn ArrowDash
        spriteRenderer.enabled = false;
        arrowCollider.enabled = false;

        // Đợi thời gian respawn
        yield return new WaitForSeconds(respawnTime);

        // Hiện ArrowDash
        spriteRenderer.enabled = true;
        arrowCollider.enabled = true;
    }
}