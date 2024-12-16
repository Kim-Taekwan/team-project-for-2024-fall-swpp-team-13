using UnityEngine;
using UnityEngine.UI;

public class FlourUIManager : MonoBehaviour
{
    public Image flourScreenUI; 
    public float uiDuration = 2.0f;
    private PlayerController playerController;
    private Coroutine currentFlourCoroutine;

    void Start()
    {
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    public void ShowFlourUI()
    {
        if (flourScreenUI != null)
        {
            if (currentFlourCoroutine != null) StopCoroutine(currentFlourCoroutine);
            currentFlourCoroutine = StartCoroutine(ShowAndHideUI());
        }
    }

    private System.Collections.IEnumerator ShowAndHideUI()
    {
        flourScreenUI.gameObject.SetActive(true);
        flourScreenUI.color = Color.white;
        yield return new WaitForSeconds(0.5f);
        //TODO : Add SFX

        Color color = flourScreenUI.color;
        float time = 0.0f;
        while (color.a > 0.0f)
        {
            float timeAddition = Time.deltaTime / uiDuration;
            time += playerController.isUsingPowerup ? 3 * timeAddition : timeAddition;
            color.a = Mathf.Lerp(1, 0, time);
            flourScreenUI.color = color;
            yield return null;
        }
        flourScreenUI.gameObject.SetActive(false);
    }
}
