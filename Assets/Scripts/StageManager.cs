using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Animations;
using UnityEngine;
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
    [SerializeField] bool obtainedRecipe = false;
    [SerializeField] int stageCoins = 0;
    [SerializeField] int stageScore = 0;
    [SerializeField] bool isGameOver = false;
    [SerializeField] bool isGameClear = false;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI coinText;
    public Image recipeImage;
    public Image[] powerupImages = new Image[5];
    public GameObject staminaBar;
    [SerializeField] float rechargeRate = 3.0f;
    private Slider staminaSlider;
    private Coroutine recharging;

    // Player status
    [SerializeField] int hp, maxHp = 6;
    [SerializeField] float stamina, maxStamina = 10.0f;
    [SerializeField] Powerup currentPowerup = Powerup.None;
    public GameObject[] mouseForms = new GameObject[5];

    // Start is called before the first frame update
    void Start()
    {
        staminaSlider = staminaBar.GetComponent<Slider>();
        SetMaxStamina();
    }

    private void SetMaxStamina()
    {
        stamina = maxStamina;
        staminaSlider.maxValue = maxStamina;
        staminaSlider.value = maxStamina;
    }

    // Update is called once per frame
    void Update()
    {

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

    public void RunStamina(float staminaCost)
    {
        stamina = (stamina >= staminaCost) ? stamina - staminaCost : 0.0f;
        staminaSlider.value = stamina;

        // Recharging stamina coroutine happens uniquely
        if (recharging != null) StopCoroutine(recharging);
        recharging = StartCoroutine(RechargeStamina());
    }

    IEnumerator RechargeStamina()
    {
        yield return new WaitForSeconds(1);
        while (stamina < maxStamina)
        {
            // Recharge Stamina by 'rechargeRate' every second
            stamina += rechargeRate / 50.0f;
            if (stamina >= maxStamina) stamina = maxStamina;
            staminaSlider.value = stamina;
            yield return new WaitForSeconds(0.02f);
        }
    }

    public void GameClear()
    {
        isGameClear = true;
        GameManager.Instance.UpdateStageClear(stageScore, obtainedRecipe);
    }
}
