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

    // Click on stage works as Enter key down
    void OnMouseDown()
    {
        UIWorldMap uiWorldMap = FindObjectOfType<UIWorldMap>();
        if (uiWorldMap == null)
        {
            Debug.LogError("UIWorldMap instance not found");
            return;
        }

        if (uiWorldMap.stageUnlocked[nodeIndex])
        {
            string nextSceneName = "";

            switch (nodeIndex)
            {
                case 0: nextSceneName = "Stage1"; break;
                case 1: nextSceneName = "Stage2"; break;
                case 2: nextSceneName = "Stage3"; break;
                default:
                    Debug.LogError("Invalid node index: " + nodeIndex);
                    return;
            }

            LoadingSceneController.LoadScene(nextSceneName);
        }
        else
        {
            Debug.Log("This stage is locked.");
        }
    }

}