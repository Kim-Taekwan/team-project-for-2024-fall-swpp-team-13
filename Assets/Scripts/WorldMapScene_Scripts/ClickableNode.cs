using UnityEngine;

public class ClickableNode : MonoBehaviour
{
    private int nodeIndex;
    private UIWorldMap uiWorldMap;
    private MeshRenderer meshRenderer;
    private Color originalColor = new Color(1f, 0.5f, 0f); 

    public void Setup(int index, UIWorldMap worldMap) 
    {
        nodeIndex = index;
        uiWorldMap = worldMap;
        meshRenderer = transform.Find("thickcircle")?.GetComponent<MeshRenderer>();

        if (meshRenderer != null)
        {
            meshRenderer.material.color = (nodeIndex == uiWorldMap.currentStageIndex) ? uiWorldMap.currentStageColor : originalColor;
        }
    }

    // Change to red color if hovering over the circle
    void OnMouseEnter()
    {
        if (meshRenderer != null && uiWorldMap.stageUnlocked[nodeIndex])
        {
            AudioManager.Instance.PlayMoveButtonSound();
            meshRenderer.material.color = uiWorldMap.currentStageColor;
        }
    }

    // If not hovering the mouse over the circle, return to the original color
    void OnMouseExit()
    {
        if (meshRenderer != null && uiWorldMap.stageUnlocked[nodeIndex])
        {
            if (nodeIndex != uiWorldMap.currentStageIndex)
            {
                meshRenderer.material.color = originalColor;
            }
        }
    }

    void OnMouseDown()
    {
        UIWorldMap uiWorldMap = FindObjectOfType<UIWorldMap>();
        if (uiWorldMap == null)
        {
            Debug.LogError("UIWorldMap instance not found");
            return;
        }

        // Check if the thickcircle child exists and its color is black
        MeshRenderer thickCircleRenderer = transform.Find("thickcircle")?.GetComponent<MeshRenderer>();
        if (thickCircleRenderer != null && thickCircleRenderer.material.color == uiWorldMap.lockedStageColor)
        {
            Debug.Log("Cannot click this node because thickcircle is black (locked).");
            return;
        }

        if (uiWorldMap.stageUnlocked[nodeIndex])
        {
            AudioManager.Instance.PlayMoveSceneSound();
            string nextSceneName = "";
            GameManager.Instance.currentStage = nodeIndex + 1;
            Debug.Log($"GameManager currentStage updated to: {GameManager.Instance.currentStage}");

            switch (gameObject.name)
            {
                case "NodeStage1":
                    nextSceneName = "Stage1";
                    break;
                case "NodeStage2":
                    nextSceneName = "Stage2";
                    break;
                case "NodeStage3":
                    nextSceneName = "Stage3";
                    break;
                default:
                    Debug.LogError("Invalid object name: " + gameObject.name);
                    return;
            }

            Debug.Log($"Loading scene: {nextSceneName}");
            LoadingSceneController.LoadScene(nextSceneName);
        }
        else
        {
            Debug.Log("This stage is locked.");
        }
    }
}