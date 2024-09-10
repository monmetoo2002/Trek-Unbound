using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject player;
    private Transform playerPos;
    private Vector2 currentPos;
    private Vector2 startPos;
    public float distance;
    public float speedEnemy;
    private Animator enemyAnim;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    private AudioManager audioManager;

    private bool isDying = false;

    void Start()
    {
        playerPos = player.GetComponent<Transform>();
        enemyAnim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        audioManager = FindObjectOfType<AudioManager>();

        currentPos = transform.position;
        startPos = transform.position;
    }

    void Update()
    {
        if (!isDying)
        {
            UpdateMovement();
            Flip();
        }
    }

    void UpdateMovement()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerPos.position);
        if (distanceToPlayer < distance)
        {
            transform.position = Vector2.MoveTowards(transform.position, playerPos.position, speedEnemy * Time.deltaTime);
            enemyAnim.SetBool("Chase", true);
        }
        else
        {
            float distanceToStart = Vector2.Distance(transform.position, startPos);
            if (distanceToStart > 0.1f)
            {
                transform.position = Vector2.MoveTowards(transform.position, startPos, speedEnemy * Time.deltaTime);
                enemyAnim.SetBool("Chase", true);
            }
            else
            {
                enemyAnim.SetBool("Chase", false);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
        {
            enemyAnim.SetTrigger("Death");
            Death();
        }
    }

    void Death()
    {
        isDying = true;
        enemyAnim.SetTrigger("Death");
        audioManager.PlaySFX(audioManager.enemyDeath);
        rb.velocity = Vector2.zero;
        rb.simulated = false;
        rb.isKinematic = true;

        // Tắt nhạc trong vùng
        if (audioManager != null)
        {
            audioManager.ExitMusicZone();
        }
    }

    void Flip()
    {
        if (playerPos.position.x < transform.position.x)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }

    // reset vị trí
    public void ResetPositionEnemy()
    {
        transform.position = startPos;
        currentPos = startPos;
        enemyAnim.SetBool("Chase", false);
        isDying = false;

        rb.simulated = true;
        rb.isKinematic = false;
        enemyAnim.Play("Idle"); // Reset animation to Idle
    }

    // được gọi khi Player hồi sinh
    public void OnPlayerRespawnEnemy()
    {
        ResetPositionEnemy();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distance);
    }
}
