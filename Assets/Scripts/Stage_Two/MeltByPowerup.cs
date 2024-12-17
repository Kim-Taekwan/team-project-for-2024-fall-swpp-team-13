using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeltByPowerup : MonoBehaviour
{
    private PlayerController playerController;
    public GameObject dustPrefab;

    void Start()
    {
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && playerController.isUsingPowerup)
        {
            GameObject dust = Instantiate(dustPrefab, transform.position + new Vector3(0.0f, 0.5f, 0.0f), Quaternion.identity);
            //TODO : Add SFX
            Destroy(gameObject);
        }
    }
}
