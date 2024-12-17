using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Warp : MonoBehaviour
{
    public Transform targetPosition;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && targetPosition != null)
        {
            AudioManager.Instance.PlayWarpSound();
            other.transform.position = targetPosition.position;
            AlignActiveCamera(targetPosition.position);
        }
    }

    private void AlignActiveCamera(Vector3 position)
    {
        CinemachineVirtualCamera activeCamera = FindActiveCinemachineCamera();
        if (activeCamera != null)
        {
            activeCamera.OnTargetObjectWarped(activeCamera.Follow, position - activeCamera.Follow.position);
        }
    }

    private CinemachineVirtualCamera FindActiveCinemachineCamera()
    {
        CinemachineVirtualCamera[] cameras = FindObjectsOfType<CinemachineVirtualCamera>();
        foreach (CinemachineVirtualCamera cam in cameras)
        {
            if (cam.Priority == 20) 
            {
                return cam;
            }
        }
        return null;
    }
}