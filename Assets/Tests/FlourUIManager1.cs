using UnityEngine;

public class FlourUIManager1 : MonoBehaviour
{
    public GameObject flourScreenUI; 
    public float uiDuration = 3f;    

    public virtual void ShowFlourUI()
    {
        if (flourScreenUI != null)
        {
            StartCoroutine(ShowAndHideUI());
        }
    }

    public virtual System.Collections.IEnumerator ShowAndHideUI()
    {
        flourScreenUI.SetActive(true); 

        yield return new WaitForSeconds(uiDuration); 
        if (flourScreenUI != null)
        {
            flourScreenUI.SetActive(false); 
        }
    }
}
