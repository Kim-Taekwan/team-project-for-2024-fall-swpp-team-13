using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupEventManager : MonoBehaviour
{
    public CinemachineVirtualCamera eventCamera;
    public CinemachineDollyCart eventCart;
    public CinemachineVirtualCamera defaultCamera;
    public GameObject sweetPotato;
    public Transform[] bugSpawnPoints = new Transform[6];
    public float playerMoveDelay = 5.5f;
    public float returnDelay = 0.5f;
    public GameObject bugPrefab;
    public GameObject keyGuidePrefab;
    private GameObject keyGuideInstance;
    private GameObject mainCanvas;
    [SerializeField] bool hasStarted = false;
    [SerializeField] bool isSkillReady = false;
    [SerializeField] bool hasEnded = false;
    private PlayerController playerController;
    private StageManager stageManager;

    private void Start()
    {
        stageManager = GameObject.Find("StageManager").GetComponent<StageManager>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        mainCanvas = GameObject.Find("Main Canvas");
    }

    // Update is called once per frame
    void Update()
    {
        if (sweetPotato == null && !hasStarted)
        {
            hasStarted = true;
            stageManager.canPause = false;
            EnemiesAttackEvent();
        }

        if (isSkillReady && !hasEnded && Input.GetKeyDown(KeyCode.C))
        {
            hasEnded = true;
            Time.timeScale = 1.0f;
            stageManager.canPause = true;
            StartCoroutine(EndEvent());
        }
    }

    private void EnemiesAttackEvent()
    {
        eventCamera.Priority = 20;
        StartCoroutine(playerController.MovePause(playerMoveDelay));
        StartCoroutine(SlowMotion());
        StartCoroutine(PopUpGuide());

        // Generates enemies from 6 spawn points
        foreach (Transform spawnPoint in bugSpawnPoints)
        {
            Instantiate(bugPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }

    IEnumerator SlowMotion()
    {
        yield return new WaitForSeconds(3.0f);
        eventCart.m_Speed = 6.5f;
        yield return new WaitForSeconds(1.5f);
        while (!hasEnded)
        {
            yield return new WaitForSeconds(0.1f);
            Time.timeScale = Time.timeScale - 0.1f <= 0.0f ? 0.0f : Time.timeScale - 0.1f;
            Debug.Log(Time.time);
        }
    }

    IEnumerator PopUpGuide()
    {
        yield return new WaitForSeconds(playerMoveDelay);
        keyGuideInstance = Instantiate(keyGuidePrefab, mainCanvas.transform);
        isSkillReady = true;
    }

    IEnumerator EndEvent()
    {
        if (keyGuideInstance != null)
        {
            Destroy(keyGuideInstance);
        }
        yield return new WaitForSeconds(returnDelay);
        eventCamera.Priority = 9;
        defaultCamera.MoveToTopOfPrioritySubqueue();
    }
}
