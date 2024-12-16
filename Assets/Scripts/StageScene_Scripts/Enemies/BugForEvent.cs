using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

// Bugs that always chase and attack the player
public class BugForEvent : MonoBehaviour, IEnemy
{
    public int hp = 1;
    public int damageAmount = 1;
    public float speed = 3.0f;
    [SerializeField] float currentSpeed;
    public int score = 200;

    private StageManager stageManager;
    private Transform playerTransform;
    private Rigidbody rb;
    private GameObject player;
    private Animator animator;
    private bool isDead = false;
    public GameObject deathParticlePrefab;

    void Awake()
    {
        player = GameObject.Find("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        rb = GetComponent<Rigidbody>();
        animator = transform.GetChild(0).GetComponent<Animator>();
    }

    void Start()
    {
        stageManager = GameObject.Find("StageManager").GetComponent<StageManager>();
    }

    void FixedUpdate()
    {
        if(!stageManager.CheckGameContinue() || isDead || playerTransform == null || rb == null)
        {
            rb.velocity = Vector3.zero;
            return;
        }

        Vector3 playerXZPosition = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);
        Vector3 direction = (playerXZPosition - transform.position).normalized;
        Vector3 targetVelocity = direction * speed;
        rb.velocity = new Vector3(targetVelocity.x, rb.velocity.y, targetVelocity.z);
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        
        // Turn Animation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f);
        currentSpeed = new Vector2(rb.velocity.x, rb.velocity.z).magnitude;
        animator.SetFloat("Speed_f", currentSpeed);
    }

    // Damaged by player jump or attack
    public void TakeDamage(int amount)
    {
        AudioManager.Instance.PlayAttackSound();
        hp -= amount;
        if (hp <= 0)
        {
            isDead = true;
            rb.velocity = Vector3.zero;
            GetComponent<BoxCollider>().enabled = false;
            animator.SetTrigger("DeathTrig");
            stageManager.UpdateScore(score);
            StartCoroutine(DelayDeath());
        }
    }

    private IEnumerator DelayDeath()
    {
        yield return new WaitForSeconds(0.3f);
        if (deathParticlePrefab != null)
        {
            Instantiate(deathParticlePrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    public bool CanBeSteppedOn() => true;

    public void GiveDamage()
    {
        player.GetComponent<PlayerController>().TakeDamage(damageAmount, transform);
    }
}
