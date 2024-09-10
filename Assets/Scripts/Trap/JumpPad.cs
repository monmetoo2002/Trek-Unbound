using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] private float bounce = 20f;
    private Animator animator;
    AudioManager audioManager;


    private void Start()
    {
        animator = GetComponent<Animator>();
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            animator.SetTrigger("Bounce");
            audioManager.PlaySFX(audioManager.jumpPadBounce);

            // Áp dụng lực đẩy lên nhân vật
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.AddForce(Vector2.up * bounce, ForceMode2D.Impulse);
            }
        }
    }
}
