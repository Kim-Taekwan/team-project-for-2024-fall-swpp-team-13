using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float speedZ = 2f;
    public float rangeZ = 3f;
    public float speedY = 1f;
    public float rangeY = 2f;
    public float jitterSpeed = 0.5f;
    public float jitterRange = 0.5f;

    private Vector3 initialPosition;
    private Vector3 jitterOffset;

    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        float z = Mathf.Sin(Time.time * speedZ) * rangeZ;
        float y = Mathf.Sin(Time.time * speedY) * rangeY / 2f;

        jitterOffset.x = Mathf.PerlinNoise(Time.time * jitterSpeed, 0) * 2 - 1;
        jitterOffset.y = Mathf.PerlinNoise(Time.time * jitterSpeed, 1) * 2 - 1;
        jitterOffset.z = Mathf.PerlinNoise(Time.time * jitterSpeed, 2) * 2 - 1;

        jitterOffset *= jitterRange;

        transform.position = initialPosition + new Vector3(jitterOffset.x, y + jitterOffset.y, z + jitterOffset.z);
    }
}