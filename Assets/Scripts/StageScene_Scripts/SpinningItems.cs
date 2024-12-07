using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningItems : MonoBehaviour
{
    public float spinSpeed = 90.0f;
    public float floatSpeed = 3.0f;
    public float floatLimit = 0.1f;
    public float startY;

    void Start()
    {
        startY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        // Spin spinSpeed degree every second
        transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.World);

        // Float slightly up and down
        float floatedY = startY + Mathf.Sin(Time.timeSinceLevelLoad * floatSpeed) * floatLimit;
        transform.position = new Vector3(transform.position.x, floatedY, transform.position.z);
    }
}
