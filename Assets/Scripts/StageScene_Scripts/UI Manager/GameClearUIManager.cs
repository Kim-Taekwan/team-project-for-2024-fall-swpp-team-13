using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameClearUIManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI coinText;
    public Image recipeImage;
    public Sprite[] recipeSprites = new Sprite[2];
    public Button goNextButton;
    public Vector3 newPlayerOffset = new Vector3(2.5f, 0.5f, 2);

    private StageManager stageManager;
    private CameraController cameraController;

    private void OnEnable()
    {
        StageManager.OnGameCleared += UpdateGameClearUI;
    }

    private void OnDisable()
    {
        StageManager.OnGameCleared -= UpdateGameClearUI;
    }

    // Start is called before the first frame update
    void Start()
    {
        stageManager = GameObject.Find("StageManager").GetComponent<StageManager>();
        cameraController = GameObject.Find("Main Camera").GetComponent<CameraController>();
    }

    private void UpdateGameClearUI()
    {
        scoreText.text = "½ºÄÚ¾î " + stageManager.stageScore.ToString("D5");
        coinText.text = "X " + stageManager.stageCoins.ToString("D2");
        recipeImage.sprite = stageManager.obtainedRecipe ? recipeSprites[1] : recipeSprites[0];
        goNextButton.Select();
        cameraController.playerOffset = newPlayerOffset;
    }
}
