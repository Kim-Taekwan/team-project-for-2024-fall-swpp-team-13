using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class Warp : MonoBehaviour
{
    public Transform targetPosition; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (targetPosition != null)
            {
                other.transform.position = targetPosition.position;
            }
        }
    }
}