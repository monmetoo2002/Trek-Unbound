using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingRock : MonoBehaviour
{
    Rigidbody2D rb;
    Vector2 defaultPos;

    [SerializeField] float fallDelay;
    [SerializeField] float respawnTime;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultPos = transform.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            StartCoroutine("PlatformDrop");
        }
    }

    IEnumerator PlatformDrop()
    {
        yield return new WaitForSeconds(fallDelay);
        rb.bodyType = RigidbodyType2D.Dynamic;
        yield return new WaitForSeconds(respawnTime);
        Reset();
    }

    private void Reset()
    {
        rb.bodyType = RigidbodyType2D.Static;
        transform.position = defaultPos;
    }
}