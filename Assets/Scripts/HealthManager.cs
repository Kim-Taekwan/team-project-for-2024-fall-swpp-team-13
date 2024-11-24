using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public GameObject heartPrefab;
    private StageManager stageManager;
    List<Heart> hearts = new List<Heart>();

    private void OnEnable()
    {
        StageManager.OnPlayerDamaged += UpdateHearts;
    }

    private void OnDisable()
    {
        StageManager.OnPlayerDamaged -= UpdateHearts;
    }

    // Start is called before the first frame update
    void Start()
    {
        stageManager = GameObject.Find("StageManager").GetComponent<StageManager>();
        UpdateHearts();
    }

    public void UpdateHearts()
    {
        ClearHearts();

        // Assume stageManager.maxHp >= stageManager.hp
        int numOfHearts = stageManager.maxHp / 2;
        for (int i = 0; i < numOfHearts; i++)
        {
            CreateEmptyHeart();
        }

        // Update sprites of full & half hearts
        int numOfHalfHearts = stageManager.hp % 2;
        int numOfFullHearts = stageManager.hp / 2;
        for (int i = 0; i < numOfFullHearts + numOfHalfHearts; i++)
        {
            if (i < numOfFullHearts)
            {
                hearts[i].SetHeartSprite(HeartStatus.Full);
            }
            else
            {
                hearts[i].SetHeartSprite(HeartStatus.Half);
            }
        }

    }

    public void CreateEmptyHeart()
    {
        GameObject newHeart = Instantiate(heartPrefab, transform);
        Heart newHeartScript = newHeart.GetComponent<Heart>();
        newHeartScript.SetHeartSprite(HeartStatus.Empty);
        hearts.Add(newHeartScript);
    }

    public void ClearHearts()
    {
        // Destroy all children (hearts)
        foreach (Transform t in transform)
        {
            Destroy(t.gameObject);
        }
        hearts = new List<Heart>();
    }
}
