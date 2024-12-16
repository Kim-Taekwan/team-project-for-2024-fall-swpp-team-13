using UnityEngine;

public class QuitButtonController : MonoBehaviour
{
    private SettingsManager settingsManager;

    void Start()
    {
        settingsManager = FindObjectOfType<SettingsManager>();

        if (settingsManager == null)
        {
            Debug.LogError("SettingsManager instance not found in the scene!");
        }
    }

    public void QuitYes()
    {
        if (settingsManager != null)
        {
            settingsManager.SaveSettings();
            settingsManager.settingsPanel.SetActive(false);
            settingsManager.quitPanel.SetActive(false);
            Time.timeScale = 1;
            Debug.Log("QuitYes executed.");
        }
        else
        {
            Debug.LogError("SettingsManager is null. Cannot perform QuitYes.");
        }
    }

    public void QuitNo()
    {
        if (settingsManager != null && settingsManager.quitPanel != null)
        {
            settingsManager.quitPanel.SetActive(false);
            Debug.Log("QuitNo executed.");
        }
        else
        {
            Debug.LogError("SettingsManager or quitPanel is null. Cannot perform QuitNo.");
        }
    }
}
