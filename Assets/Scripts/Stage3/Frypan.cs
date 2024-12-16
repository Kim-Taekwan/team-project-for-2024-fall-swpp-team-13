using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class Frypan : MonoBehaviour
{
    public float rotationSpeed = 50f;
    public float tiltAmount = 10f;
    public float tiltSpeed = 2f;
    public float moveSpeed = 1f;
    public float moveRange = 2f;

    private Vector3 initialPosition;
    private Vector3 initialRotation;

    void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.eulerAngles;
    }

    void Update()
    {
        float tiltX = Mathf.Sin(Time.time * tiltSpeed) * tiltAmount;
        float tiltZ = Mathf.Cos(Time.time * tiltSpeed) * tiltAmount;
        float moveZ = Mathf.PingPong(Time.time * moveSpeed, moveRange);

        transform.eulerAngles = initialRotation + new Vector3(tiltX, 0, tiltZ);
        transform.position = initialPosition + new Vector3(0, 0, moveZ);
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
