using System.Collections;
using UnityEngine;

public class ShakeRock : MonoBehaviour
{
    private Animator animator;
    private Vector2 defaultPos;
    private SpriteRenderer spriteRenderer;
    private Collider2D platformCollider;

    [SerializeField] private float shakeDelay = 0.5f;
    [SerializeField] private float disappearDelay = 1f;
    [SerializeField] private float respawnTime = 3f;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        platformCollider = GetComponent<Collider2D>();
        defaultPos = transform.position;

        // Đảm bảo animation bắt đầu ở trạng thái Idle
        animator.SetBool("Shake", false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(PlatformSequence());
        }
    }

    IEnumerator PlatformSequence()
    {
        animator.SetBool("Shake", true);

        // Đợi trong thời gian shakeDelay
        yield return new WaitForSeconds(shakeDelay);

        // Đợi thêm một khoảng thời gian trước khi biến mất
        yield return new WaitForSeconds(disappearDelay);

        // Ẩn platform
        DisablePlatform();

        // Đợi một khoảng thời gian trước khi xuất hiện lại
        yield return new WaitForSeconds(respawnTime);

        // Hiện platform
        EnablePlatform();
    }

    private void DisablePlatform()
    {
        spriteRenderer.enabled = false;
        platformCollider.enabled = false;
        animator.SetBool("Shake", false);
    }

    private void EnablePlatform()
    {
        transform.position = defaultPos;
        spriteRenderer.enabled = true;
        platformCollider.enabled = true;
        animator.SetBool("Shake", false);
    }
}