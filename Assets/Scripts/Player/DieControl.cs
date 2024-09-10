using UnityEngine;
using System.Collections;

public class DieControl : MonoBehaviour
{
    public static DieControl instance;
    //[SerializeField] Animator transitionAnim;
    public GameManager gameManager;
    private Rigidbody2D rb;
    private AudioManager audioManager;
    public Movement movement;
    
    public ParticleSystem dieParticle;

    private Vector2 checkpointPos;

    private void Awake()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        audioManager = FindObjectOfType<AudioManager>();
        movement = GetComponent<Movement>();
        gameManager = FindObjectOfType<GameManager>();
        rb = GetComponent<Rigidbody2D>();

    }

    private void Start()
    {
        checkpointPos = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle") || collision.CompareTag("Boss") || collision.CompareTag("Enemy"))
        {
            Die();
        }
    }

    public void UpdateCheckPoint(Vector2 pos)
    {
        checkpointPos = pos;
    }

    private void Die()
    {
        audioManager.PlaySFX(audioManager.death);

        dieParticle.Play();

        gameManager.ResetGemCountToLastCheckpoint();
        gameManager.IncreaseDieCount();

        // Reset key khi player die
        ResetAllKeys();

        StartCoroutine(Respawn(1.5f));
    }

    private IEnumerator Respawn(float time)
    {
        //transitionAnim.SetTrigger("End");
        rb.simulated = false;
        rb.velocity = Vector2.zero;
        transform.localScale = Vector3.zero;

        if (movement != null)
        {
            movement.canMove = false;            
            movement.slideParticle.Stop();

        }

        yield return new WaitForSeconds(time);

        transform.position = checkpointPos;
        transform.localScale = Vector3.one;
        rb.simulated = true;

        if (movement != null)
        {
            movement.canMove = true;
            movement.slideParticle.Play();
        }

        //reset vị trí của Boss
        Boss boss = FindObjectOfType<Boss>();
        if (boss != null)
        {
            boss.OnPlayerRespawnBoss();
        }

        //reset vị trí của Enemy
        Enemy enemy = FindObjectOfType<Enemy>();
        if (enemy != null)
        {
            enemy.OnPlayerRespawnEnemy();
        }

        //transitionAnim.SetTrigger("Start");
    }
    
    private void ResetAllKeys()
    {
        Key[] keys = FindObjectsOfType<Key>();
        foreach (Key key in keys)
        {
            key.ResetKey();
        }
    }
}