using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingPin : MonoBehaviour
{
    public float oscillationSpeed = 2f;
    public float rotationSpeed = 100f;
    public float oscillationRange = 2f;
    private float initialX;

    void Start()
    {
        initialX = transform.position.x;
    }

    void Update()
    {
        float newX = initialX + Mathf.Sin(Time.time * oscillationSpeed) * oscillationRange;
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}