using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bug : MonoBehaviour, IEnemy
{
    public int hp = 1;
    public int damageAmount = 1;
    public bool isStationary = false;

    [SerializeField] Vector3 defaultPosition;
    public Vector3 patrolRange = new Vector3(5.0f, 0, 5.0f);
    public float speed = 3.0f;
    public float detectDistance = 3.0f; // 플레이어 감지 거리
    public float returnDistance = 5.0f; // 플레이어를 추적하다가 돌아가는 거리
    public bool isAttacking = false;
    private float reachedThreshold = 1.0f;
    [SerializeField] float currentSpeed;
    private Quaternion defaultRotation;
    private Quaternion targetRotation;

    private StageManager stageManager;
    private Vector3 patrolTarget;
    private Transform playerTransform;
    private Rigidbody rb;
    private GameObject player;
    private Animator animator;
    private bool isDead = false;
    [SerializeField] bool hasReached = false;

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
        defaultRotation = transform.rotation;
        SetNewPatrolTarget();
    }

    void FixedUpdate()
    {
        if(!stageManager.CheckGameContinue() || isDead || playerTransform == null || rb == null)
        {
            rb.velocity = Vector3.zero;
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
            Vector3 playerXZPosition = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);
            direction = (playerXZPosition - transform.position).normalized;
            Vector3 targetVelocity = direction * speed;
            rb.velocity = new Vector3(targetVelocity.x, rb.velocity.y, targetVelocity.z);
            targetRotation = Quaternion.LookRotation(direction);
        }
        else
        {
            float currentDistanceToPatrolTarget = Vector3.Distance(transform.position, patrolTarget);
            if (currentDistanceToPatrolTarget < reachedThreshold)
            {
                if (isStationary)
                {
                    hasReached = true;
                }
                else
                {
                    // patrolTarget과 충분히 가까우면 새로운 목표 지점 생성
                    SetNewPatrolTarget();
                }                
            }
            else
            {
                hasReached = false;
            }

            if (hasReached)
            {
                targetRotation = defaultRotation;
            }
            else
            {
                direction = patrolTarget - transform.position;
                direction.y = 0.0f;
                direction.Normalize();
                Vector3 targetVelocity = direction * speed;
                rb.velocity = new Vector3(targetVelocity.x, rb.velocity.y, targetVelocity.z);
                targetRotation = Quaternion.LookRotation(direction);
            }            
        }

        // Turn Animation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f);
        currentSpeed = new Vector2(rb.velocity.x, rb.velocity.z).magnitude;
        animator.SetFloat("Speed_f", currentSpeed);
    }

    void SetNewPatrolTarget()
    {
        if (isStationary)
        {
            patrolTarget = defaultPosition;
        }
        else
        {
            // 범위 안에서 랜덤으로 패트롤 타겟 지정
            float randomX = Random.Range(-patrolRange.x, patrolRange.x);
            float randomZ = Random.Range(-patrolRange.z, patrolRange.z);
            patrolTarget = defaultPosition + new Vector3(randomX, 0, randomZ);
        }
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
            StartCoroutine(DelayDeath());
        }
    }

    private IEnumerator DelayDeath()
    {
        yield return new WaitForSeconds(0.3f);
        //TODO: Add Particle Effect
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
