using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
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
    public float slowMotionFactor = 0.5f; // Tốc độ chậm (0.5 = 50% tốc độ bình thường)
    public float slowMotionDuration = 1f; // Thời gian kéo dài hiệu ứng chậm

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
        if (Vector2.Distance(transform.position, playerPos.position) < distance)
        {
            transform.position = Vector2.MoveTowards(transform.position, playerPos.position, speedEnemy * Time.deltaTime);
            enemyAnim.SetBool("Chase", true);
        }
        else
        {
            if (Vector2.Distance(transform.position, currentPos) <= 0)
            {
                enemyAnim.SetBool("Chase", false);
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, currentPos, speedEnemy * Time.deltaTime);
                enemyAnim.SetBool("Chase", true);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Door"))
        {
            enemyAnim.SetTrigger("Death");
            StartCoroutine(Death());
        }
    }

    IEnumerator Death()
    {
        isDying = true;
        enemyAnim.SetTrigger("Death");

        audioManager.PlaySFX(audioManager.bossDeath);

        rb.velocity = Vector2.zero;
        rb.simulated = false;

        // Bắt đầu slow motion
        Time.timeScale = slowMotionFactor;

        // Đợi trong thời gian slow motion
        yield return new WaitForSecondsRealtime(slowMotionDuration);

        // Trả lại tốc độ bình thường
        Time.timeScale = 1f;

        // Đợi cho animation kết thúc
        yield return new WaitForSeconds(enemyAnim.GetCurrentAnimatorStateInfo(0).length * (1 - slowMotionFactor));

        Destroy(gameObject);
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

    // Phương thức mới để reset vị trí của Boss
    public void ResetPositionBoss()
    {
        transform.position = startPos;
        currentPos = startPos;

        rb.simulated = true;
        enemyAnim.SetBool("Chase", false);
    }

    // Phương thức này sẽ được gọi khi Player hồi sinh
    public void OnPlayerRespawnBoss()
    {
        ResetPositionBoss();
    }

    void OnDrawGizmos()
    {
        // Set the Gizmo color to something noticeable, like red
        Gizmos.color = Color.red;

        // Draw a wireframe sphere (appears as a circle in 2D view) to represent the distance
        Gizmos.DrawWireSphere(transform.position, distance);
    }
}