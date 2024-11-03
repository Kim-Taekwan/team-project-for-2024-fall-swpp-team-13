using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningItems : MonoBehaviour
{
    public float spinSpeed = 90.0f;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime);
    }
}
