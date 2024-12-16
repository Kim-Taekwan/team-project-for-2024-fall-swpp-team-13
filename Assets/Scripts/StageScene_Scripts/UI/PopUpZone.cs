using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpZone : MonoBehaviour
{
    public GameObject keyGuidePrefab;
    private GameObject mainCanvas;
    private GameObject keyGuideInstance;

    // Start is called before the first frame update
    void Start()
    {
        mainCanvas = GameObject.Find("Main Canvas");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            keyGuideInstance = Instantiate(keyGuidePrefab, mainCanvas.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (keyGuideInstance != null)
            {
                Destroy(keyGuideInstance);
            }
        }
    }
}
