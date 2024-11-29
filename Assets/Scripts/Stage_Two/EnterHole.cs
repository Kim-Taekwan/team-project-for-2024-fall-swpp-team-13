using UnityEngine;

public class EnterHole : MonoBehaviour
{
    public Transform exitHole; 
    private bool playerInHole = false; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInHole = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInHole = false;
        }
    }

    private void Update()
    {
        if (playerInHole && Input.GetKeyDown(KeyCode.Z))
        {
            if (exitHole != null)
            {
                TeleportPlayer();
            }
        }
    }

    private void TeleportPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            player.transform.position = exitHole.position;
        }
    }
}
