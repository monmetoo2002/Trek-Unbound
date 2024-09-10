using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject door;
    AudioManager audioManager;

    public bool isPickedUp;
    private Vector2 vel;
    public float smoothTime;
    private Vector3 initialPosition;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        if(isPickedUp) 
        {
            // Thay đổi vị trí của key 1 chút (đỡ dính vào Player)
            Vector3 offset = new Vector3(0, 1, 0);
            // Thay đổi vị trí của key theo player một cách mượt mà sử dụng SmoothDamp
            transform.position = Vector2.SmoothDamp(transform.position, player.transform.position + offset, ref vel, smoothTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player") && !isPickedUp)
        {
            audioManager.PlaySFX(audioManager.arrowDashPickup);
            isPickedUp = true;

            door.GetComponent<Door>().keyPickedUp = true;
        }
    }

    public void ResetKey()
    {
        isPickedUp = false;
        door.GetComponent<Door>().keyPickedUp = false;
        transform.position = initialPosition;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, door.transform.position);
    }
}
