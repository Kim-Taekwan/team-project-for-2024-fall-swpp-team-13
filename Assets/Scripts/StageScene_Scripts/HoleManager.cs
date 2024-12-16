using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleManager : MonoBehaviour
{
    public GameObject hole1;
    public GameObject hole2;
    [SerializeField] float exitDelay = 0.5f;
    [TextArea(minLines: 2, maxLines: 4)]
    public string Attention;
    private GameObject player;
    private StageManager stageManager;
    private PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        stageManager = GameObject.Find("StageManager").GetComponent<StageManager>();
        player = GameObject.Find("Player");
        playerController = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToHole1 = Vector3.Distance(player.transform.position, hole1.transform.position);
        float distanceToHole2 = Vector3.Distance(player.transform.position, hole2.transform.position);

        if (stageManager.CheckGameContinue() && Input.GetKeyDown(KeyCode.X))
        {
            if (distanceToHole1 < 1.5f)
            {
                //TODO: Particle effects on both side
                Vector3 hole2ExitPosition = hole2.transform.GetChild(0).position;
                player.transform.position = hole2ExitPosition;
                StartCoroutine(playerController.MoveFreeze(exitDelay));
            }
            else if (distanceToHole2 < 1.5f)
            {
                //TODO: Particle effects on both side
                Vector3 hole1ExitPosition = hole1.transform.GetChild(0).position;
                player.transform.position = hole1ExitPosition;
                StartCoroutine(playerController.MoveFreeze(exitDelay));
            }
        }        
    }
}
