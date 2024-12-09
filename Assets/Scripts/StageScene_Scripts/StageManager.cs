using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum Powerup
{
    None,        // 0
    SweetPotato, // 1
    ChiliPepper, // 2
    Carrot,      // 3
}

// Manage stage & player status and UI on playing stage screen
public class StageManager : MonoBehaviour
{
    // Stage status
    public bool obtainedRecipe = false;
    public int stageCoins = 0;
    public int stageScore = 0;
    public bool isGameOver = false;
    public bool isGameClear = false;
    public bool isGamePaused = false;
    public bool isFreezed = false; // for movement of all dynamic objects like player, enemies, obstacles

    // Stage UI
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI coinText;
    public Image recipeImage;
    public Image[] powerupImages = new Image[4];
    public Canvas mainCanvas;
    public Canvas gameOverCanvas;
    public Canvas gameClearCanvas;
    private PauseUIManager pauseUIManager;

    // Player status
    public int hp = 6, maxHp = 6;
    public float stamina = 10.0f, maxStamina = 10.0f;
    public Powerup currentPowerup = Powerup.None;
    private GameObject player;
    private PlayerController playerController;

    // Events
    public static event Action OnPlayerDamaged;
    public static event Action OnPlayerHealed;
    public static event Action OnGameCleared;

    public GameObject clearParticlePrefab;

    // Start is called before the first frame update
    void Start()
    {
        pauseUIManager = GameObject.Find("PauseUIManager").GetComponent<PauseUIManager>();
        player = GameObject.Find("Player");
        playerController = player.GetComponent<PlayerController>();
    }

    // Checker method to determine the game continues
    public bool CheckGameContinue()
    {
        return !isGamePaused && !isGameClear && !isGameOver && !isFreezed;
    }

    public void UpdateScore(int score)
    {
        stageScore += score;
        scoreText.text = stageScore.ToString("D5");
    }

    public void AddCoins(int coins)
    {
        stageCoins += coins;
        //AudioManager.Instance.PlayCoinSound();
        UpdateScore(coins * 100);
        coinText.text = "�� " + stageCoins.ToString("D2");
    }

    public void ObtainRecipe()
    {
        obtainedRecipe = true;
        AudioManager.Instance.PlayRecipeSound();
        UpdateScore(5000);

        // Make UI image color opaque
        Color recipeColor = recipeImage.color;
        recipeColor.a = 1.0f;
        recipeImage.color = recipeColor;
    }

    public void UpdatePowerup(string powerupName)
    {
        UpdateScore(1000);
        switch (powerupName)
        {
            case "Sweet Potato":
                currentPowerup = Powerup.SweetPotato;
                break;
            case "Chili Pepper":
                currentPowerup = Powerup.ChiliPepper;
                break;
            case "Carrot":
                currentPowerup = Powerup.Carrot;
                break;
            default:
                Debug.Log("Error: Powerup not recognized");
                break;
        }

        // Only activate current powerup form and UI image
        for (int i = 0; i < 4; i++)
        {
            Transform powerupForm = player.transform.GetChild(i);
            powerupForm.gameObject.SetActive(i == (int)currentPowerup);
            powerupImages[i].gameObject.SetActive(i == (int)currentPowerup);
        }
        playerController.animator = player.transform.GetChild((int)currentPowerup).GetComponent<Animator>();
        playerController.ResetPowerupSettings();
    }

    public void HealHp(int amount)
    {
        if (!CheckGameContinue())
        {
            return;
        }
        hp = (maxHp <= hp + amount) ? maxHp : hp + amount;
        OnPlayerHealed?.Invoke();
    }

    public void LoseHp(int amount)
    {
        if (!CheckGameContinue())
        {
            return;
        }
        hp = (hp <= amount) ? 0 : hp - amount;
        OnPlayerDamaged?.Invoke();
    }

    public void GameOver()
    {
        isGameOver = true;
        AudioManager.Instance.PlayGameOverSound();
        mainCanvas.gameObject.SetActive(false);
        gameOverCanvas.gameObject.SetActive(true);
    }

    public void GameClear()
    {
        isGameClear = true;
        AudioManager.Instance.StopAllSounds();
        AudioManager.Instance.PlayGameClearSound();
        mainCanvas.gameObject.SetActive(false);
        gameClearCanvas.gameObject.SetActive(true);
        OnGameCleared?.Invoke();
        StartCoroutine(GameClearDelay(0.3f));
        //Instantiate(clearParticlePrefab, player.transform.position, Quaternion.identity);

        // Save log right after stage clear
        GameManager.Instance.UpdateStageClear(stageScore, obtainedRecipe);
    }

    private IEnumerator GameClearDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Instantiate(clearParticlePrefab, player.transform.position, Quaternion.identity);
    }

    public void RestartGame()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public IEnumerator StageFreeze(float freezeDelay)
    {
        isFreezed = true;
        yield return new WaitForSeconds(freezeDelay);
        isFreezed = false;
    }
}
