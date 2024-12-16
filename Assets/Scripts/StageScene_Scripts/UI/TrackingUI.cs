using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingUI : MonoBehaviour
{
    private GameObject player;
    private Camera mainCamera;
    [SerializeField] Vector3 offset = new Vector3(330, 240, 0);

    private void Start()
    {
        player = GameObject.Find("Player");
        mainCamera = Camera.main;
    }

    void FixedUpdate()
    {
        if (player != null)
        {
            transform.position = mainCamera.WorldToScreenPoint(player.transform.position) + offset;
        }
    }
}
