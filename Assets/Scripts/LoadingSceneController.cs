using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneController : MonoBehaviour
{
    static string nextScene;

    [SerializeField]
    private Image progressBar;
    
    private static bool isLoading = false;

    public static void LoadScene(string sceneName)
    {
        if (isLoading) return; 
        isLoading = true;
        nextScene = sceneName; 
        SceneManager.LoadSceneAsync("LoadingScene"); 
    }

    void Start()
    {
        progressBar.fillAmount = 0f;
        StartCoroutine(LoadSceneProcess()); 
    }

    IEnumerator LoadSceneProcess()
    {
        Debug.Log($"Next Scene to Load: {nextScene}");
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false; 

        if (op == null)
        {
            Debug.LogError("AsyncOperation is null! Check the scene name or loading setup.");
            yield break;
        }   
        Debug.Log($"Loading: {nextScene}");
        float timer = 0f; 

        while (!op.isDone)
        {
            yield return null; 

            float progress = Mathf.Clamp01(op.progress / 0.9f);

            if (progress < 0.9f)
            {
                progressBar.fillAmount = progress; 
            }
            else
            {
                timer += Time.unscaledDeltaTime;
                progressBar.fillAmount = Mathf.Lerp(0.9f, 1f, timer);

                if (progressBar.fillAmount >= 1f)
                {
                    op.allowSceneActivation = true; 
                    yield break; 
                }
            }
        }
        isLoading = false;
    }
}
