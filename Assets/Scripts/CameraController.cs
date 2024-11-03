using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public Vector3 cameraOffset = new Vector3(0, 3, -4);
    public Vector3 playerOffset = new Vector3(0, 0, 2);
    public float smoothSpeed = 1.5f;
    
    void LateUpdate()
    {
        Vector3 desiredPosition = player.position + cameraOffset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
        transform.LookAt(player.transform.position + playerOffset);
    }
}