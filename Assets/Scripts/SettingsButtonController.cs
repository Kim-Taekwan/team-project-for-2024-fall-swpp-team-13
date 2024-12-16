using UnityEngine;

public class SettingsButtonController : MonoBehaviour
{
    public GameObject settingsPanel; // Settings Panel 오브젝트

    public void OpenSettingsPanel()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true); // Settings Panel 활성화
        }
        else
        {
            Debug.LogError("Settings Panel is not assigned or not found.");
        }
    }
}
