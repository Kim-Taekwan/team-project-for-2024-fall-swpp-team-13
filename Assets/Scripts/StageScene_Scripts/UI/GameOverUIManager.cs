using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUIManager : MonoBehaviour
{
    public List<Button> buttons = new List<Button>();
    public int currentButtonIndex = 0;

    private StageManager stageManager;

    // Start is called before the first frame update
    void Start()
    {
        stageManager = GameObject.Find("StageManager").GetComponent<StageManager>();
    }

    void OnEnable()
    {
        currentButtonIndex = 0;
        HighlightButton(currentButtonIndex);
    }

    // Update is called once per frame
    void Update()
    {
        // Pause UI action
        if (stageManager.isGameOver)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                currentButtonIndex = (currentButtonIndex + 1 >= buttons.Count) ? 0 : currentButtonIndex + 1;
                HighlightButton(currentButtonIndex);
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                currentButtonIndex = (currentButtonIndex <= 0) ? buttons.Count - 1 : currentButtonIndex - 1;
                HighlightButton(currentButtonIndex);
            }
        }
    }

    public void HighlightButton(int currentIndex)
    {
        AudioManager.Instance.PlayMoveButtonSound();
        currentButtonIndex = currentIndex;

        foreach (Button button in buttons)
        {
            button.GetComponent<Outline>().enabled = false;
        }
        buttons[currentButtonIndex].GetComponent<Outline>().enabled = true;
        buttons[currentButtonIndex].Select();
    }
}
