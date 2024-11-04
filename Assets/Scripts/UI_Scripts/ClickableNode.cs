using UnityEngine;

public class ClickableNode : MonoBehaviour
{
    private int nodeIndex;
    private UIManager uiManager;
    private MeshRenderer meshRenderer;
    private Color originalColor = new Color(1f, 0.5f, 0f); 

    public void Setup(int index, UIManager manager)
    {
        nodeIndex = index;
        uiManager = manager;
        meshRenderer = transform.Find("thickcircle")?.GetComponent<MeshRenderer>();

        if (meshRenderer != null)
        {
            meshRenderer.material.color = (nodeIndex == uiManager.currentStageIndex) ? uiManager.currentStageColor : originalColor;
        }
    }

    // Change to red color if hovering over the circle
    void OnMouseEnter()
    {
        if (meshRenderer != null && uiManager.stageUnlocked[nodeIndex])
        {
            meshRenderer.material.color = uiManager.currentStageColor;
        }
    }

    // If not hovering the mouse over the circle, return to the original color
    void OnMouseExit()
    {
        if (meshRenderer != null && uiManager.stageUnlocked[nodeIndex])
        {
            if (nodeIndex != uiManager.currentStageIndex)
            {
                meshRenderer.material.color = originalColor;
            }
        }
    }

    // Click on stage works as Enter key down
    void OnMouseDown()
    {
        uiManager.SelectLevel(nodeIndex); 
    }
}