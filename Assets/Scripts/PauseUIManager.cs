using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseUIManager : MonoBehaviour
{
    public Canvas mainCanvas;
    public Canvas pauseCanvas;
    public bool isGamePaused = false;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isGamePaused)
                ActivatePauseUI();
            else
                DeactivatePauseUI();            
        }
    }

    private void ActivatePauseUI()
    {
        isGamePaused = true;
        Time.timeScale = 0.0f;
        pauseCanvas.gameObject.SetActive(true);
        
    }

    private void DeactivatePauseUI()
    {
        isGamePaused = false;
        Time.timeScale = 1.0f;
        pauseCanvas.gameObject.SetActive(false);
    }
}
