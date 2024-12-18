using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIWorldMap : MonoBehaviour
{
    public Color completedStageColor = new Color(1f, 0.5f, 0f); // RGB for orange
    public Color currentStageColor = Color.red;
    public Color lockedStageColor = Color.black;
    public Color unlockedLineColor = new Color(1f, 0.5f, 0f); 
    public Color lockedLineColor = Color.black;

    public int currentStageIndex = 0;
    public List<GameObject> stageNodes = new List<GameObject>();
    public List<bool> stageUnlocked = new List<bool> { true, false, false};

    public List<Image> lines = new List<Image>();
    public Sprite[] recipeSprites = new Sprite[2];
    public Image[] recipeImages = new Image[3];
    public TextMeshProUGUI[] bestScoresTexts = new TextMeshProUGUI[3];
    public Image clearPanel;
    public TextMeshProUGUI clearText;

    void Start()
    {
        FindAllNodes();

        // Manually activating levels here, this will actually have to be handled at each Stage scene's scripts
        //PlayerProgress.MarkLevelCompleted(2); // Activate level 2
        //PlayerProgress.MarkLevelCompleted(3);

        UpdateStageUnlockedFromGameManager();
        //stageUnlocked[1] = PlayerProgress.IsLevelCompleted(2); // check if level 2 is completed
        //stageUnlocked[2] = PlayerProgress.IsLevelCompleted(3); 

        // The level with the greatest index is initially selected when starting to load the scene
        for (int i = stageUnlocked.Count - 1; i >= 0; i--)
        {
            if (stageUnlocked[i])
            {
                currentStageIndex = i;
                break;
            }
        }

        UpdateStageNodes();
        UpdateLineColors();
        UpdateStageStatus();
    }

    private void UpdateStageStatus()
    {
        for (int i = 0; i < recipeImages.Length; i++)
        {
            recipeImages[i].sprite = GameManager.Instance.obtainedRecipes[i] ? recipeSprites[1] : recipeSprites[0];
            bestScoresTexts[i].text = GameManager.Instance.bestScores[i].ToString("D5");
        }

        if (GameManager.Instance.stageProgress == 4 && clearPanel != null && clearText != null)
        {
            AudioManager.Instance.PlayClapSound();
            clearPanel.gameObject.SetActive(true);
            clearText.gameObject.SetActive(true);
        }
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        // Left and Right allow to change the selected Level
        // Stage X is executed when it is selected and Enter key is pressed 
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentStageIndex < stageNodes.Count - 1 && stageUnlocked[currentStageIndex + 1])
            {
                AudioManager.Instance.PlayMoveButtonSound();
                currentStageIndex++;
                UpdateStageNodes();
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentStageIndex > 0)
            {
                AudioManager.Instance.PlayMoveButtonSound();
                currentStageIndex--;
                UpdateStageNodes();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            LoadLevel(currentStageIndex);
        }
    }

    void FindAllNodes()
    {
        stageNodes.Add(GameObject.Find("NodeStage1"));
        stageNodes.Add(GameObject.Find("NodeStage2"));
        stageNodes.Add(GameObject.Find("NodeStage3"));

        UIWorldMap uiWorldMap = FindObjectOfType<UIWorldMap>(); 

        for (int i = 0; i < stageNodes.Count; i++)
        {
            int index = i;
            stageNodes[i].AddComponent<ClickableNode>().Setup(index, uiWorldMap); 
        }
    }

    public void LoadLevel(int index)
    {
        if (stageUnlocked[index])
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.currentStage = index + 1;
                AudioManager.Instance.PlayMoveSceneSound();
                Debug.Log($"GameManager currentStage updated to: {GameManager.Instance.currentStage}");
            }
            else
            {
                Debug.LogWarning("GameManager Instance not found!");
            }
            switch (index)
            {
                case 0: LoadingSceneController.LoadScene("Stage1"); break;
                case 1: LoadingSceneController.LoadScene("Stage2"); break;
                case 2: LoadingSceneController.LoadScene("Stage3"); break;
            }
        }
    }

    void UpdateStageNodes()
    {
        for (int i = 0; i < stageNodes.Count; i++)
        {
            if (i < stageUnlocked.Count && stageNodes[i] != null) // Add this bounds check
            {
                GameObject thickCircle = stageNodes[i].transform.Find("thickcircle")?.gameObject;
                if (thickCircle != null)
                {
                    MeshRenderer meshRenderer = thickCircle.GetComponent<MeshRenderer>();
                    if (meshRenderer != null)
                    {
                        if (stageUnlocked[i])
                        {
                            meshRenderer.material.color = (i == currentStageIndex) ? currentStageColor : completedStageColor;
                        }
                        else
                        {
                            meshRenderer.material.color = lockedStageColor;
                        }
                    }
                }
            }
        }
    }

   void UpdateLineColors()
    {
        if (stageNodes.Count >= 2)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                bool isConnectedUnlocked = stageUnlocked[i] && stageUnlocked[i + 1];
                lines[i].gameObject.SetActive(isConnectedUnlocked);

                if (isConnectedUnlocked)
                {
                    lines[i].color = unlockedLineColor;
                }
            }
        }
    }

    public void SelectLevel(int index)
    {
        if (stageUnlocked[index])
        {
            currentStageIndex = index;
            UpdateStageNodes();
            UpdateLineColors();
            LoadLevel(index);
        }
    }
    public void ReturnToMenu()
    {
        AudioManager.Instance.PlayDecisionButtonSound();
        SceneManager.LoadScene("TitleScene"); 
    }

    private void UpdateStageUnlockedFromGameManager()
    {
        if (GameManager.Instance != null)
        {
            int stageProgress = GameManager.Instance.stageProgress;
            Debug.Log($"Stage Progress from GameManager: {stageProgress}");

            for (int i = 0; i < stageProgress && i < stageUnlocked.Count; i++)
            {
                stageUnlocked[i] = true;
            }
        }
        else
        {
            Debug.LogWarning("GameManager Instance not found!");
        }
    }
}