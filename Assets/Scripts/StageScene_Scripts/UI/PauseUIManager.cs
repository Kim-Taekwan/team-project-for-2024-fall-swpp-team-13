using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseUIManager : MonoBehaviour
{
    public Canvas mainCanvas;
    public Canvas pauseCanvas;
    public List<Button> buttons = new List<Button>();
    public int currentButtonIndex = 0;
    public GameObject settingsPanel;

    private StageManager stageManager;

    // Start is called before the first frame update
    void Start()
    {
        stageManager = GameObject.Find("StageManager").GetComponent<StageManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !stageManager.isGameOver && !stageManager.isGameClear && stageManager.canPause)
        {
            if (!stageManager.isGamePaused)
                ActivatePauseUI();
            else if (settingsPanel.activeInHierarchy == false)
                DeactivatePauseUI();
        }

        // Pause UI action
        if (stageManager.isGamePaused)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                currentButtonIndex = (currentButtonIndex + 1>= buttons.Count) ? 0 : currentButtonIndex + 1;
                HighlightButton(currentButtonIndex);
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                currentButtonIndex = (currentButtonIndex <= 0) ? buttons.Count - 1 : currentButtonIndex - 1;
                HighlightButton(currentButtonIndex);
            }
        }
    }

    private void ActivatePauseUI()
    {
        stageManager.isGamePaused = true;
        Time.timeScale = 0.0f;
        pauseCanvas.gameObject.SetActive(true);

        currentButtonIndex = 0;
        HighlightButton(currentButtonIndex);
    }

    public void DeactivatePauseUI()
    {
        stageManager.isGamePaused = false;
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
