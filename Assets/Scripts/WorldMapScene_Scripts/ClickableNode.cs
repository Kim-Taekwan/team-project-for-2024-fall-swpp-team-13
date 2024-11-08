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
        uiWorldMap.SelectLevel(nodeIndex); 
    }
}