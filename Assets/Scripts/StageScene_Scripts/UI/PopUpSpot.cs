using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpSpot : MonoBehaviour
{
    public GameObject keyGuidePrefab;
    public float guideDistance = 2.0f;
    public float guideCancelDistance = 2.5f;
    [SerializeField] bool isGuiding = false;
    private GameObject player;
    private GameObject mainCanvas;
    private GameObject keyGuideInstance;

    void OnDestroy()
    {
        if (keyGuideInstance != null)
        {
            Destroy(keyGuideInstance);
        }
    }

    void Start()
    {
        player = GameObject.Find("Player");
        mainCanvas = GameObject.Find("Main Canvas");
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
        if (!isGuiding && distanceToPlayer < guideDistance)
        {
            isGuiding = true;
            keyGuideInstance = Instantiate(keyGuidePrefab, mainCanvas.transform);
        }
        else if (isGuiding && distanceToPlayer >= guideCancelDistance)
        {
            isGuiding = false;
            if (keyGuideInstance != null)
            {
                Destroy(keyGuideInstance);
            }
        }
    }
}
