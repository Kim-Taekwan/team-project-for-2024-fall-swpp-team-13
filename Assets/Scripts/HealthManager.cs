using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public GameObject heartPrefab;
    private StageManager stageManager;
    List<Heart> hearts = new List<Heart>();
    

    // Start is called before the first frame update
    void Start()
    {
        stageManager = GameObject.Find("StageManager").GetComponent<StageManager>();
        DrawHearts();
    }

    public void DrawHearts()
    {
        ClearHearts();

        int numOfHearts = stageManager.maxHp / 2;
        for (int i = 0; i < numOfHearts; i++)
        {
            CreateEmptyHeart();
        }

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
        GameObject newHeart = Instantiate(heartPrefab);
        newHeart.transform.SetParent(transform);

        Heart newHeartScript = newHeart.GetComponent<Heart>();
        newHeartScript.SetHeartSprite(HeartStatus.Empty);
        hearts.Add(newHeartScript);
    }

    public void ClearHearts()
    {
        foreach (Transform t in transform)
        {
            Destroy(t.gameObject);
        }
        hearts = new List<Heart>();
    }
}
