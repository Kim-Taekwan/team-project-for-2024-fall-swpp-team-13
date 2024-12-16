using System.Collections.Generic;
using UnityEngine;
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

    public List<GameObject> lines = new List<GameObject>();

    void Start()
    {
        FindAllNodes();
        FindAllLines();

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
                currentStageIndex++;
                UpdateStageNodes();
                UpdateLineColors();
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentStageIndex > 0)
            {
                currentStageIndex--;
                UpdateStageNodes();
                UpdateLineColors();
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

    void FindAllLines()
    {
        lines.Add(GameObject.Find("Line12"));
        lines.Add(GameObject.Find("Line23"));
    }

    public void LoadLevel(int index)
    {
        if (stageUnlocked[index])
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.currentStage = index + 1; 
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
        if (lines.Count >= 3)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                bool isConnectedUnlocked = stageUnlocked[i] && stageUnlocked[i + 1];
                lines[i].SetActive(isConnectedUnlocked);

                if (isConnectedUnlocked)
                {
                    lines[i].GetComponent<Renderer>().material.color = unlockedLineColor;
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