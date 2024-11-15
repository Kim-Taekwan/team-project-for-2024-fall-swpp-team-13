using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    public Slider brightnessSlider;
    public Slider bgmVolumeSlider;
    public Slider sfxVolumeSlider;
    public Dropdown resolutionDropdown;
    public Dropdown fullscreenDropdown;

    public GameObject brightnessValueText;
    public GameObject bgmVolumeValueText;
    public GameObject sfxVolumeValueText;
    public GameObject resolutionValueText;
    public GameObject fullscreenValueText;
    
    public GameObject originalPlane;
    public GameObject quitAskPlane;
    public GameObject quitAskText;
    public GameObject quitYesButton;
    public GameObject quitNoButton;

    public bool isSaved;
    public bool isQuitAsk;
    public int selectedQuitButton = 0;

    public Resolution[] resolutions;
    private Resolution[] predefinedResolutions = new Resolution[]
    {
        new Resolution { width = 1920, height = 1080 },
        new Resolution { width = 1600, height = 900},
        new Resolution { width = 1280, height = 720 }
    };

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        Resolution[] supportedResolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<Resolution> availiableResolutions = predefinedResolutions
        .Where(predef => supportedResolutions.Any(supported => 
            predef.width == supported.width && predef.height == supported.height))
        .ToList();

        resolutions = availiableResolutions.ToArray();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        quitAskPlane.SetActive(false);
        quitAskText.SetActive(false);
        quitYesButton.SetActive(false);
        quitNoButton.SetActive(false);

        isQuitAsk = false;
        isSaved = true;

        LoadSettings();
    }

    public void SetBrightness(int brightness)
    {
        RenderSettings.ambientLight = new Color(brightness, brightness, brightness / 100, 1);
        isSaved = false;
    }

    public void SetBGMVolume(int volume)
    {
        AudioManager.Instance.SetBGMVolume(volume / 100);
        isSaved = false;
    }

    public void SetSFXVolume(int volume)
    {
        AudioManager.Instance.SetSFXVolume(volume / 100);
        isSaved = false;
    }

    public void SetResolution(int resolutionIndex)
    {
        if (resolutionIndex < 0 || resolutionIndex >= resolutions.Length)
        {
            Debug.LogWarning("Invalid resolution index");
            return;
        }
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        isSaved = false;
    }

    public void SetFullscreen(int isFullscreen)
    {
        Screen.fullScreen = isFullscreen == 1;
        isSaved = false;
    }

    public void Back()
    {
        if(isSaved){
            // last scene 없을 때 오류 출력
            SceneManager.UnloadSceneAsync("SettingsScene");
            //SceneManager.LoadScene(PlayerPrefs.GetString("LastScene"));
        }
        else{
            QuitAsk();
        }
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("Brightness", RenderSettings.ambientLight.r);
        PlayerPrefs.SetFloat("BGMVolume", AudioManager.Instance.bgmVolume);
        PlayerPrefs.SetFloat("SFXVolume", AudioManager.Instance.sfxVolume);
        PlayerPrefs.SetInt("Resolution", resolutionDropdown.value);
        PlayerPrefs.SetInt("Fullscreen", fullscreenDropdown.value);
        isSaved = true;
    }

    public void LoadSettings()
    {
        if(!PlayerPrefs.HasKey("Brightness") || !PlayerPrefs.HasKey("BGMVolume") || !PlayerPrefs.HasKey("SFXVolume") || !PlayerPrefs.HasKey("Resolution") || !PlayerPrefs.HasKey("Fullscreen")){
            PlayerPrefs.SetFloat("Brightness", 1f);
            PlayerPrefs.SetFloat("BGMVolume", 0.5f);
            PlayerPrefs.SetFloat("SFXVolume", 0.5f);
            PlayerPrefs.SetInt("Resolution", 0);
            PlayerPrefs.SetInt("Fullscreen", 1);
        }

        RenderSettings.ambientLight = new Color(PlayerPrefs.GetFloat("Brightness"), PlayerPrefs.GetFloat("Brightness"), PlayerPrefs.GetFloat("Brightness"), 1);
        brightnessSlider.value = PlayerPrefs.GetFloat("Brightness");

        AudioManager.Instance.SetBGMVolume(PlayerPrefs.GetFloat("BGMVolume"));
        bgmVolumeSlider.value = PlayerPrefs.GetFloat("BGMVolume");

        AudioManager.Instance.SetSFXVolume(PlayerPrefs.GetFloat("SFXVolume"));
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume");

        resolutionDropdown.value = PlayerPrefs.GetInt("Resolution");
        SetResolution(PlayerPrefs.GetInt("Resolution"));

        fullscreenDropdown.value = PlayerPrefs.GetInt("Fullscreen");
        SetFullscreen(fullscreenDropdown.value);
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
        LoadSettings();
        SceneManager.LoadScene(PlayerPrefs.GetString("LastScene"));
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

    public void Update()
    {
        if (isQuitAsk)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                selectedQuitButton = 1 - selectedQuitButton;
                showQuitSelect();
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                selectedQuitButton = 1 - selectedQuitButton;
                showQuitSelect();
            }
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (selectedQuitButton == 0)
                {
                    QuitYes();
                }
                if (selectedQuitButton == 1)
                {
                    QuitNo();
                }
            }
        }
    }
}