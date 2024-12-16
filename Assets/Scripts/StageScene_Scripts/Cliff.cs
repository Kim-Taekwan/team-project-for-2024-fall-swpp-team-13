using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cliff : MonoBehaviour
{
    private GameObject player;
    private PlayerController playerController;
    private StageManager stageManager;
    Vector3 respawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        playerController = player.GetComponent<PlayerController>();
        stageManager = GameObject.Find("StageManager").GetComponent<StageManager>();
        respawnPoint = transform.GetChild(0).position;
    }

    // Update is called once per frame
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerController.TakeDamage(1);
            if (!stageManager.isGameOver)
            {
                player.transform.position = respawnPoint;
            }
            else
            {
                player.SetActive(false);
            }
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject);
        }
    }
}
