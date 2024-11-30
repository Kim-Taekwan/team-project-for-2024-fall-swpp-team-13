using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleManager : MonoBehaviour
{
    public GameObject hole1;
    public GameObject hole2;
    private GameObject player;
    private StageManager stageManager;

    // Start is called before the first frame update
    void Start()
    {
        stageManager = GameObject.Find("StageManager").GetComponent<StageManager>();
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToHole1 = Vector3.Distance(player.transform.position, hole1.transform.position);
        float distanceToHole2 = Vector3.Distance(player.transform.position, hole2.transform.position);

        if (stageManager.CheckGameContinue() && Input.GetKeyDown(KeyCode.Z))
        {
            if (distanceToHole1 < 1.2f)
            {                
                player.transform.position = hole2.transform.position;
            }
            else if (distanceToHole2 < 1.2f)
            {
                player.transform.position = hole1.transform.position;
            }
        }        
    }
}
