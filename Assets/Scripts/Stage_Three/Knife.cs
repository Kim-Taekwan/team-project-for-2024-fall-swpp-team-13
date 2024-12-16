using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : MonoBehaviour
{
    public float speed = 2f;
    public float range = 4f;
    private float initialY;

    void Start()
    {
        initialY = transform.position.y;
    }

    void Update()
    {
        float y = initialY + Mathf.PingPong(Time.time * speed, range);
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }
}