using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Suriken : MonoBehaviour
{
    public float speed;
    public Transform pos1;
    public Transform pos2;
    Vector3 targetPos;

    private void Start()
    {
        targetPos = pos2.position;
    }

    private void Update()
    {
        if(Vector2.Distance(transform.position, pos1.position) < 0.05)
        {
            targetPos = pos2.position;
        }

        if (Vector2.Distance(transform.position, pos2.position) < 0.05)
        {
            targetPos = pos1.position;
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
    }
}
