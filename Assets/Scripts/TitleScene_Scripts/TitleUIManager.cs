using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class TitleUIManager : MonoBehaviour
{
    private Renderer planeRenderer;
    public GameObject newGameButton;
    public GameObject continueButton;
    public GameObject settingsButton;
    public GameObject quitButton;
    public GameObject originalPlane;
    public GameObject quitAskPlane;
    public GameObject quitAskText;
    public GameObject quitYesButton;
    public GameObject quitNoButton;
    public GameObject[] selectTriangleList;
    public int selectedButton = 0;
    public int selectedQuitButton = 0;
    private int maxButtons = 4;
    public bool isQuitAsk;

    void OnEnable()
    {
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    void OnDisable()
    {
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    void OnSceneUnloaded(Scene scene)
    {
        showButtons();
        showSelectTriangle();
    }

    public void Start()
    {
        showButtons();
        showSelectTriangle();
        hideQuitAsk();

        isQuitAsk = false;
    }
    public void Update()
    {
        if(!isQuitAsk){
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
                    LoadContinueScene();
                }
                if (selectedButton == 2)
                {
                    LoadSettingsScene();
                }
                if (selectedButton == 3)
                {
                    QuitAsk();
                }
            }
        }
        else{
            if(Input.GetKeyDown(KeyCode.LeftArrow)){
                if(selectedQuitButton == 1){
                    selectedQuitButton = 0;
                }
                showQuitSelect();
            }
            if(Input.GetKeyDown(KeyCode.RightArrow)){
                if(selectedQuitButton == 0){
                    selectedQuitButton = 1;
                }
                showQuitSelect();
            }
            if(Input.GetKeyDown(KeyCode.Return)){
                if(selectedQuitButton == 0){
                    QuitYes();
                }
                if(selectedQuitButton == 1){
                    QuitNo();
                }
            }
        }
    }
    public void hideButtons()
    {
        newGameButton.SetActive(false);
        continueButton.SetActive(false);
        settingsButton.SetActive(false);
        quitButton.SetActive(false);
    }
    public void showButtons()
    {
        newGameButton.SetActive(true);
        continueButton.SetActive(true);
        settingsButton.SetActive(true);
        quitButton.SetActive(true);
    }
    public void hideQuitAsk()
    {
        quitAskPlane.SetActive(false);
        quitAskText.SetActive(false);
        quitYesButton.SetActive(false);
        quitNoButton.SetActive(false);
    }
    public void showQuitAsk()
    {
        quitAskPlane.SetActive(true);
        quitAskText.SetActive(true);
        quitYesButton.SetActive(true);
        quitNoButton.SetActive(true);
    }
    public void hideSelectTriangle()
    {
        for (int i = 0; i < selectTriangleList.Length; i++)
        {
            selectTriangleList[i].SetActive(false);
        }
    }
    public void showSelectTriangle()
    {
        for (int i = 0; i < selectTriangleList.Length; i++)
        {
            selectTriangleList[i].SetActive(false);
        }
        selectTriangleList[2*selectedButton].SetActive(true);
        selectTriangleList[2*selectedButton+1].SetActive(true);
    }
    public void LoadWorldMapScene()
    {
        if(!isQuitAsk){
            PlayerPrefs.SetString("LastScene", "TitleScene");
            SceneManager.LoadScene("WorldMapScene");
        }
    }
    public void LoadSettingsScene()
    {
        hideButtons();
        hideSelectTriangle();
        if(!isQuitAsk){
            SceneManager.LoadSceneAsync("SettingsScene", LoadSceneMode.Additive);
        }
    }
    public void LoadContinueScene()
    {
        if(!isQuitAsk){
            PlayerPrefs.SetString("LastScene", "TitleScene");
            SceneManager.LoadScene("ContinueScene");
        }
    }
    public void QuitAsk()
    {
        Renderer planeRenderer = originalPlane.GetComponent<Renderer>();
        planeRenderer.material.color = new Color(0.8f, 0.8f, 0.8f, 1.0f);
        isQuitAsk = true;
        quitAskPlane.SetActive(true);
        quitAskText.SetActive(true);
        quitYesButton.SetActive(true);
        quitNoButton.SetActive(true);
        selectedQuitButton = 0;
        showQuitSelect();
    }
    public void QuitYes()
    {
        Application.Quit();
    }
    public void QuitNo()
    {
        Renderer planeRenderer = originalPlane.GetComponent<Renderer>();
        planeRenderer.material.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        isQuitAsk = false;
        quitAskPlane.SetActive(false);
        quitAskText.SetActive(false);
        quitYesButton.SetActive(false);
        quitNoButton.SetActive(false);
        selectedQuitButton = 0;
    }
    public void showQuitSelect()
    {
        Outline quitYesOutline = quitYesButton.gameObject.GetComponent<Outline>();
        Outline quitNoOutline = quitNoButton.gameObject.GetComponent<Outline>();
        if(selectedQuitButton == 0){
            quitYesOutline.effectColor = new Color(1.0f, 0.0f, 0.0f, 1.0f);
            quitNoOutline.effectColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        }
        if(selectedQuitButton == 1){
            quitYesOutline.effectColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
            quitNoOutline.effectColor = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        }
    }
}
