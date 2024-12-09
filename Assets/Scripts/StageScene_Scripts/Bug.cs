using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bug : MonoBehaviour, IEnemy
{
    public int hp = 1;
    public int damageAmount = 1;

    public Vector3 defaultPosition;
    public Vector3 patrolRange = new Vector3(5.0f, 0, 5.0f);
    public float speed = 3.0f;
    public float detectDistance = 3.0f; // 플레이어 감지 거리
    public float returnDistance = 5.0f; // 플레이어를 추적하다가 돌아가는 거리
    public bool isAttacking = false;
    public float reachedThreshold = 0.5f;
    [SerializeField] float currentSpeed;
    public GameObject deathParticlePrefab;

    //public bool isActivated = true;

    private StageManager stageManager;
    private Vector3 patrolTarget;
    private Transform playerTransform;
    private Rigidbody rb;
    private GameObject player;
    private Animator animator;
    private bool isDead = false;

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
        defaultPosition = transform.position;
        SetNewPatrolTarget();
    }

    void FixedUpdate()
    {
        if(!stageManager.CheckGameContinue() || isDead || playerTransform == null || rb == null)
        {
            return;
        }

        Vector3 playerPosition = playerTransform.position;
        float distanceToPlayer = Vector3.Distance(playerPosition, transform.position);

        if (isAttacking && distanceToPlayer > returnDistance)
        {
            isAttacking = false;
            SetNewPatrolTarget();
        }
        else if (!isAttacking && distanceToPlayer < detectDistance)
        {
            isAttacking = true;
        }

        Vector3 direction;
        if (isAttacking)
        {
            direction = playerPosition - transform.position;
            direction.y = 0;
            direction.Normalize();
            Vector3 targetVelocity = direction * speed;
            rb.velocity = new Vector3(targetVelocity.x, 0, targetVelocity.z);
        }
        else
        {
            // 패트롤 타겟과 충분히 가까우면 새로운 목표 지점 생성
            float currentDistanceToPatrol = Vector3.Distance(transform.position, patrolTarget);
            if (currentDistanceToPatrol < reachedThreshold)
            {
                SetNewPatrolTarget();
            }
            direction = (patrolTarget - transform.position).normalized;
            Vector3 targetVelocity = direction * speed;
            rb.velocity = new Vector3(targetVelocity.x, rb.velocity.y, targetVelocity.z);
        }

        // Turn Animation
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f);
        currentSpeed = new Vector2(rb.velocity.x, rb.velocity.z).magnitude;
        animator.SetFloat("Speed_f", currentSpeed);
    }

    void SetNewPatrolTarget()
    {
        // 범위 안에서 랜덤으로 패트롤 타겟 지정
        float randomX = Random.Range(-patrolRange.x, patrolRange.x);
        float randomZ = Random.Range(-patrolRange.z, patrolRange.z);
        patrolTarget = defaultPosition + new Vector3(randomX, 0, randomZ);
    }

    // Damaged by player jump or attack
    public void TakeDamage(int amount)
    {
        hp -= amount;
        if (hp <= 0)
        {
            isDead = true;
            rb.velocity = Vector3.zero;
            GetComponent<BoxCollider>().enabled = false;
            animator.SetTrigger("DeathTrig");
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

    public void GiveDamage()
    {
        player.GetComponent<PlayerController>().TakeDamage(damageAmount);
    }
    
    /*
    public void ActivateEnemy()
    {
        isActivated = true;
    }

    public void DeactivateEnemy()
    {
        isActivated = false;
    }
    */

    // private void OnCollisionEnter(Collision collision)
    // {
    //     if (collision.gameObject.CompareTag("Player"))
    //     {
    //         AttackPlayer();
    //     }
    // }
}
