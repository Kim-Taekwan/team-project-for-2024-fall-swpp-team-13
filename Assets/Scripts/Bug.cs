using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bug : MonoBehaviour
{
    public int hp = 1, maxhp = 1;
    public int damageAmount = 1;
    public float speed = 5.0f;
    public float detectDistance = 10.0f;
    public float returnDistance = 20.0f;
    public bool isAttacking = false;
    private StageManager stageManager;
    

    void Awake()
    {
        stageManager = GameObject.Find("StageManager").GetComponent<StageManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    { 
        
    }

    // Damaged by player jump or attack
    public void TakeDamage(int amount)
    {

    }

    public void AttackPlayer()
    {
        stageManager.TakeDamage(damageAmount);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            AttackPlayer();
        }
    }
}
