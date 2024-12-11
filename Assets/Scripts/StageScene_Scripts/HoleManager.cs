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
    public GameObject dustPrefab;

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
                StartCoroutine(PlayDustEffect(hole1.transform.position));
                StartCoroutine(PlayDustEffect(hole2.transform.position));
                Vector3 hole2ExitPosition = hole2.transform.GetChild(0).position;
                player.transform.position = hole2ExitPosition;
                StartCoroutine(playerController.MovePause(exitDelay));
            }
            else if (distanceToHole2 < 1.5f)
            {
                //TODO: Particle effects on both side
                StartCoroutine(PlayDustEffect(hole1.transform.position));
                StartCoroutine(PlayDustEffect(hole2.transform.position));
                Vector3 hole1ExitPosition = hole1.transform.GetChild(0).position;
                player.transform.position = hole1ExitPosition;
                StartCoroutine(playerController.MovePause(exitDelay));
            }
        }        
    }

    IEnumerator PlayDustEffect(Vector3 pos)
    {
        GameObject dust = Instantiate(dustPrefab, pos + new Vector3(0.0f, 0.2f, 0.0f), Quaternion.identity);
        yield return new WaitForSeconds(0.5f);
        Destroy(dust);
    }
}
