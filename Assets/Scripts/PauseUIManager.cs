using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseUIManager : MonoBehaviour
{
    public Canvas mainCanvas;
    public Canvas pauseCanvas;
    public Button[] buttons = new Button[4];
    public bool isGamePaused = false;
    public int currentButtonIndex = 0;

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

        // Pause UI action
        if (isGamePaused)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                currentButtonIndex = (currentButtonIndex + 1>= buttons.Length) ? 0 : currentButtonIndex + 1;
                HighlightButton(currentButtonIndex);
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                currentButtonIndex = (currentButtonIndex <= 0) ? buttons.Length - 1 : currentButtonIndex - 1;
                HighlightButton(currentButtonIndex);
            }
        }
    }

    private void ActivatePauseUI()
    {
        isGamePaused = true;
        Time.timeScale = 0.0f;
        pauseCanvas.gameObject.SetActive(true);

        currentButtonIndex = 0;
        HighlightButton(currentButtonIndex);
    }

    public void DeactivatePauseUI()
    {
        isGamePaused = false;
        Time.timeScale = 1.0f;
        pauseCanvas.gameObject.SetActive(false);
    }

    public void HighlightButton(int currentIndex)
    {
        currentButtonIndex = currentIndex;

        foreach (Button button in buttons)
        {
            button.GetComponent<Outline>().enabled = false;
        }
        buttons[currentButtonIndex].GetComponent<Outline>().enabled = true;
        buttons[currentButtonIndex].Select();
    }
}
