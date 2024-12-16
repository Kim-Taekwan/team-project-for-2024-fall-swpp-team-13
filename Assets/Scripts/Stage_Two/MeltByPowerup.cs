using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeltByPowerup : MonoBehaviour
{
    private PlayerController playerController;

    void Start()
    {
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && playerController.isUsingPowerup)
        {
            //TODO : Add Particle
            //TODO : Add SFX
            Destroy(gameObject);
        }
    }
}
