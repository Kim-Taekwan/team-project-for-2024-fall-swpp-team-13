using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraSwitcher : MonoBehaviour
{
    public CinemachineVirtualCamera VC1;
    public CinemachineVirtualCamera VC2;
    public MonoBehaviour playerMovementScript;
    public float transitionTime = 1.5f;

    private void Start()
    {
        VC1.Priority = 20;
        VC2.Priority = 10;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (gameObject.name == "ToVC1" && VC1.Priority < VC2.Priority)
            {
                StartCoroutine(SwitchCamera(VC1, VC2));
            }
            else if (gameObject.name == "ToVC2" && VC2.Priority < VC1.Priority)
            {
                StartCoroutine(SwitchCamera(VC2, VC1));
            }
        }
    }

    private IEnumerator SwitchCamera(CinemachineVirtualCamera activeCamera, CinemachineVirtualCamera inactiveCamera)
    {
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = false;
        }

        activeCamera.Priority = 20;
        inactiveCamera.Priority = 10;

        yield return new WaitForSeconds(transitionTime);

        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = true;
        }
    }
}