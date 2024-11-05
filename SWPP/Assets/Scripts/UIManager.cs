using UnityEngine;
using UnityEngine.SceneManagement;
public class UIManager : MonoBehaviour
{
    public GameObject newGameButton;
    public GameObject continueButton;
    public GameObject settingsButton;
    public GameObject quitButton;
    public GameObject[] SelectTriangleList;
    public int selectedButton = 0;
    private int maxButtons = 4;

    public void Start()
    {
        newGameButton.SetActive(true);
        continueButton.SetActive(true);
        settingsButton.SetActive(true);
        quitButton.SetActive(true);
        showSelectTriangle();
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (selectedButton < maxButtons - 1)
            {
                selectedButton++;
            }
            showSelectTriangle();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (selectedButton > 0)
            {
                selectedButton--;
            }
            showSelectTriangle();
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (selectedButton == 0)
            {
                LoadWorldMapScene();
            }
            if (selectedButton == 1)
            {
                LoadSettingsScene();
            }
            if (selectedButton == 2)
            {
                LoadContinueScene();
            }
            if (selectedButton == 3)
            {
                QuitGame();
            }
        }
    }
    public void showSelectTriangle()
    {
        for (int i = 0; i < SelectTriangleList.Length; i++)
        {
            SelectTriangleList[i].SetActive(false);
        }
        SelectTriangleList[2*selectedButton].SetActive(true);
        SelectTriangleList[2*selectedButton+1].SetActive(true);
    }
    public void LoadWorldMapScene()
    {
        SceneManager.LoadScene("WorldMapScene");
    }
    public void LoadSettingsScene()
    {
        SceneManager.LoadScene("SettingsScene");
    }
    public void LoadContinueScene()
    {
        SceneManager.LoadScene("ContinueScene");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
