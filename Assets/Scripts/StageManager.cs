using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    Ice          // 4
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

    // Stage UI
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI coinText;
    public Image recipeImage;
    public Image[] powerupImages = new Image[5];
    public Canvas mainCanvas;
    public Canvas gameOverCanvas;
    public Canvas gameClearCanvas;
    private PauseUIManager pauseUIManager;
    private PlayerController playerController;

    // Player status
    public int hp = 6, maxHp = 6;
    public float stamina = 10.0f, maxStamina = 10.0f;
    public Powerup currentPowerup = Powerup.None;
    public GameObject[] mouseForms = new GameObject[5];
    public float getDamageCooldown = 0.3f;
    private bool canTakeDamage = true;
    public float stunCooldown = 0.3f;
    private bool canMove = true;

    // Events
    public static event Action OnPlayerDamaged;
    public static event Action OnGameCleared;

    // Start is called before the first frame update
    void Start()
    {
        pauseUIManager = GameObject.Find("PauseUIManager").GetComponent<PauseUIManager>();
    }

    // Checker method to determine the game continues
    public bool CheckGameContinue()
    {
        return !isGamePaused && !isGameClear && !isGameOver;
    }

    public void UpdateScore(int score)
    {
        stageScore += score;
        scoreText.text = stageScore.ToString("D5");
    }

    public void AddCoins(int coins)
    {
        stageCoins += coins;
        UpdateScore(coins * 100);
        coinText.text = "X " + stageCoins.ToString("D2");
    }

    public void ObtainRecipe()
    {
        obtainedRecipe = true;
        UpdateScore(5000);

        // Make UI image color opaque
        Color recipeColor = recipeImage.color;
        recipeColor.a = 1.0f;
        recipeImage.color = recipeColor;
    }

    public void UpdatePowerup(string powerupName)
    {
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
            case "Ice":
                currentPowerup = Powerup.Ice;
                break;
            default:
                Debug.Log("Powerup not recognized");
                break;
        }

        // Only activate current powerup form and UI image
        for (int i = 0; i < 5; i++)
        {
            mouseForms[i].SetActive(i == (int)currentPowerup);
            powerupImages[i].gameObject.SetActive(i == (int)currentPowerup);
        }
    }

    public void TakeDamage(int amount)
    {
        if (!canTakeDamage)
        {
            return;
        }

        canTakeDamage = false;
        canMove = false;
        hp = (hp <= amount) ? 0 : hp - amount;
        OnPlayerDamaged?.Invoke();

        if (hp == 0)
        {
            GameOver();
        }

        StartCoroutine(DamageCooldown());
        StartCoroutine(StunCooldown());
    }

    private IEnumerator DamageCooldown()
    {
        yield return new WaitForSeconds(getDamageCooldown);
        canTakeDamage = true;
    }

    private IEnumerator StunCooldown()
    {
        yield return new WaitForSeconds(stunCooldown);
        canMove = true;
    }

    public bool CanMove()
    {
        return canMove;
    }

    public void GameOver()
    {
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        playerController.DeactivateEnemies();
        isGameOver = true;
        mainCanvas.gameObject.SetActive(false);
        gameOverCanvas.gameObject.SetActive(true);
    }

    public void GameClear()
    {
        isGameClear = true;
        mainCanvas.gameObject.SetActive(false);
        gameClearCanvas.gameObject.SetActive(true);
        OnGameCleared?.Invoke();

        // Save log right after stage clear
        GameManager.Instance.UpdateStageClear(stageScore, obtainedRecipe);
    }

    public void RestartGame()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
