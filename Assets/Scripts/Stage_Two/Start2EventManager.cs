using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Start2EventManager : MonoBehaviour
{
    public CinemachineVirtualCamera startCamera;
    public CinemachineVirtualCamera MovingCamera;
    public Transform lookOnPoint;
    public GameObject moveZone;
    private PlayerController playerController;
    private Vector3 lookOnPointMove = new Vector3(0, -5.0f, 0);

    // Start is called before the first frame update
    void Start()
    {
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        StartCoroutine(TransformCamera());
    }
    void Update()
    {
        if (lookOnPoint != null)
        {
            lookOnPoint.Translate(lookOnPointMove * Time.deltaTime);
        }
    }

    IEnumerator TransformCamera()
    {
        playerController.canMove = false;
        yield return new WaitForSeconds(2.0f);

        startCamera.enabled = false;
        Destroy(lookOnPoint.gameObject);
        startCamera.MoveToTopOfPrioritySubqueue();
        yield return new WaitForSeconds(2.0f);

        playerController.canMove = true;
        moveZone.SetActive(true);
    }
}
