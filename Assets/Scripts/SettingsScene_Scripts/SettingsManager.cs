using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    public Slider brightnessSlider;
    public Slider bgmVolumeSlider;
    public Slider sfxVolumeSlider;
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown fullscreenDropdown;

    public GameObject settingsPanel;
    public GameObject quitPanel; 
    public TextMeshProUGUI quitAskText;
    public Button quitYesButton;
    public Button quitNoButton;

    public Image brightnessOverlayPrefab; 
    private Image brightnessOverlay;  

    public bool isSaved;

    public Resolution[] resolutions;
    private Resolution[] predefinedResolutions = new Resolution[]
    {
        new Resolution { width = 1920, height = 1080 },
        new Resolution { width = 1600, height = 900 },
        new Resolution { width = 1280, height = 720 }
    };

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
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

        resolutionDropdown.onValueChanged.AddListener(SetResolution);

        fullscreenDropdown.value = Screen.fullScreen ? 1 : 0;
        fullscreenDropdown.RefreshShownValue();

        fullscreenDropdown.onValueChanged.AddListener(SetFullscreen);

        settingsPanel.SetActive(false);
        quitPanel.SetActive(false);

        isSaved = true;

        LoadSettings();
        AttachOverlayToCanvas();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AttachOverlayToCanvas();
    }

    private void AttachOverlayToCanvas()
    {
        if (brightnessOverlay == null && brightnessOverlayPrefab != null)
        {
            Canvas rootCanvas = GetRootCanvas();

            brightnessOverlay = Instantiate(brightnessOverlayPrefab, rootCanvas.transform, false);
            brightnessOverlay.transform.SetSiblingIndex(rootCanvas.transform.childCount - 1); 

            float brightness = PlayerPrefs.GetFloat("Brightness", 1f);
            float alpha = 1f - brightness;
            brightnessOverlay.color = new Color(0, 0, 0, alpha);
            Debug.Log($"BrightnessOverlay added: Alpha={alpha}");
        }
    }

    public void SetBrightness(float brightness)
    {
        if (brightnessOverlay != null)
        {
            float alpha = 1f - brightness; 
            brightnessOverlay.color = new Color(0, 0, 0, alpha);

            PlayerPrefs.SetFloat("Brightness", brightness);
            PlayerPrefs.Save();

            Debug.Log($"Brightness set to {brightness}, Alpha set to {alpha}");
        }

        isSaved = false;
    }

    public void SetBGMVolume(float volume)
    {
        if (volume < 0 || volume > 100)
        {
            return;
        }
        AudioManager.Instance.SetBGMVolume(volume);
        isSaved = false;
    }

    public void SetSFXVolume(float volume)
    {
        if (volume < 0 || volume > 100)
        {
            return;
        }
        AudioManager.Instance.SetSFXVolume(volume);
        isSaved = false;
    }

    public void SetResolution(int resolutionIndex)
    {
        if (resolutionIndex < 0 || resolutionIndex >= resolutions.Length)
        {
            return;
        }
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        isSaved = false;
    }

    public void SetFullscreen(int fullscreenIndex)
    {
        bool isFullscreen = fullscreenIndex == 1;
        Screen.fullScreen = isFullscreen;
        isSaved = false;
    }

    public void Back()
    {
        if (quitPanel.activeSelf)
        {
            quitPanel.SetActive(false);
        }
        else if (settingsPanel.activeSelf)
        {
            quitPanel.SetActive(true);
        }
    }

    public void QuitYes()
    {
        SaveSettings();
        settingsPanel.SetActive(false);
        quitPanel.SetActive(false);
        Time.timeScale = 1;
    }

    public void QuitNo()
    {
        quitPanel.SetActive(false);
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("Brightness", 1f - brightnessOverlay.color.a);
        PlayerPrefs.SetFloat("BGMVolume", AudioManager.Instance.bgmVolume);
        PlayerPrefs.SetFloat("SFXVolume", AudioManager.Instance.sfxVolume);
        PlayerPrefs.SetInt("Resolution", resolutionDropdown.value);
        PlayerPrefs.SetInt("Fullscreen", fullscreenDropdown.value);
        isSaved = true;
    }

    public void LoadSettings()
    {
        if (!PlayerPrefs.HasKey("Brightness") || !PlayerPrefs.HasKey("BGMVolume") || !PlayerPrefs.HasKey("SFXVolume") || !PlayerPrefs.HasKey("Resolution") || !PlayerPrefs.HasKey("Fullscreen"))
        {
            PlayerPrefs.SetFloat("Brightness", 1f);
            PlayerPrefs.SetFloat("BGMVolume", 0.5f);
            PlayerPrefs.SetFloat("SFXVolume", 0.5f);
            PlayerPrefs.SetInt("Resolution", 0);
            PlayerPrefs.SetInt("Fullscreen", 1);
        }
        float brightness = PlayerPrefs.GetFloat("Brightness", 1f);
        Debug.Log(brightness);
        PlayerPrefs.SetFloat("Brightness", brightness);
        brightnessSlider.value = brightness;
        brightnessSlider.onValueChanged.RemoveAllListeners();
        brightnessSlider.value = brightness;
        brightnessSlider.onValueChanged.AddListener(SetBrightness);

        if (brightnessOverlay != null)
        {
            float alpha = 1f - brightness;
            brightnessOverlay.color = new Color(0, 0, 0, alpha);
        }

        float bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 0.5f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.5f);

        bgmVolumeSlider.onValueChanged.RemoveAllListeners();
        sfxVolumeSlider.onValueChanged.RemoveAllListeners();

        AudioManager.Instance.SetBGMVolume(bgmVolume);
        bgmVolumeSlider.value = bgmVolume;

        bgmVolumeSlider.onValueChanged.AddListener(SetBGMVolume);

        AudioManager.Instance.SetSFXVolume(sfxVolume);
        sfxVolumeSlider.value = sfxVolume;

        sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);

        resolutionDropdown.value = PlayerPrefs.GetInt("Resolution");
        SetResolution(PlayerPrefs.GetInt("Resolution"));

        fullscreenDropdown.value = PlayerPrefs.GetInt("Fullscreen");
        SetFullscreen(fullscreenDropdown.value);
    }
    private Canvas GetRootCanvas()
    {
        Canvas[] allCanvases = FindObjectsOfType<Canvas>();

        foreach (Canvas canvas in allCanvases)
        {
            if (canvas.transform.parent == null) 
            {
                return canvas;
            }
        }

        GameObject newCanvas = new GameObject("RootCanvas");
        Canvas rootCanvas = newCanvas.AddComponent<Canvas>();
        rootCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

        newCanvas.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        newCanvas.AddComponent<GraphicRaycaster>();

        return rootCanvas;
    }


    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Back();
        }
    }
}
