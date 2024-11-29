using UnityEngine;

public class FlourUIManager : MonoBehaviour
{
    public GameObject flourScreenUI; 
    public float uiDuration = 3f;    

    public void ShowFlourUI()
    {
        if (flourScreenUI != null)
        {
            StartCoroutine(ShowAndHideUI());
        }
    }

    private System.Collections.IEnumerator ShowAndHideUI()
    {
        flourScreenUI.SetActive(true); 

        yield return new WaitForSeconds(uiDuration); 
        if (flourScreenUI != null)
        {
            flourScreenUI.SetActive(false); 
        }
    }
}
