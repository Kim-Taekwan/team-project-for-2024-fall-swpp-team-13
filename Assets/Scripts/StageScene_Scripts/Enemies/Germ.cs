using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Germ : MonoBehaviour, IEnemy
{
    // Status
    [Header("Status")]
    public int hp = 1;
    public int damageAmount = 1;
    public bool isStationary = false;
    public int score = 400;

    // Movement
    [Header("Movement")]
    [SerializeField] Vector3 defaultPosition;
    public Vector3 patrolRange = new Vector3(5.0f, 0, 5.0f);
    public float speed = 4.0f;
    public float detectDistance = 6.0f; // �÷��̾� ���� �Ÿ�
    public float returnDistance = 8.0f; // �÷��̾ �����ϴٰ� ���ư��� �Ÿ�
    public bool isChasing = false;
    private float reachedThreshold = 1.0f;
    [SerializeField] float currentSpeed;
    [SerializeField] bool hasReached = false;
    private Quaternion defaultRotation;
    private Quaternion targetRotation;
    private bool isDead = false;    
    private Vector3 patrolTarget;
    public GameObject deathParticlePrefab;

    // Attack
    [Header("Attack")]
    public LayerMask playerLayer;
    public bool isAttacking = false;
    public float attackThreshold = 1.7f;
    public float attackTime = 0.4f;
    public float attackCooldown = 1.3f;
    public float attackRange = 1.5f;
    public float attackAngle = 45.0f;

    // Components
    private StageManager stageManager;
    private Transform playerTransform;
    private Rigidbody rb;
    private GameObject player;
    private Animator animator;

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
        if (!stageManager.CheckGameContinue() || isAttacking || isDead || playerTransform == null || rb == null)
        {
            rb.velocity = Vector3.zero;
            animator.SetFloat("Speed_f", 0.0f);
            return;
        }

        Vector3 playerPosition = playerTransform.position;
        float distanceToPlayer = Vector3.Distance(playerPosition, transform.position);
        if (isChasing && distanceToPlayer > returnDistance)
        {
            isChasing = false;
            SetNewPatrolTarget();
        }
        else if (!isChasing && distanceToPlayer < detectDistance)
        {
            isChasing = true;
        }

        // Move to chase target or patrol target
        Vector3 direction;
        if (isChasing)
        {
            Vector3 playerXZPosition = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);
            float currentDistanceToPlayer = Vector3.Distance(transform.position, playerXZPosition);
            if (currentDistanceToPlayer >= attackThreshold && !isAttacking)
            {
                direction = (playerXZPosition - transform.position).normalized;
                targetRotation = Quaternion.LookRotation(direction);
                Vector3 targetVelocity = direction * speed;
                rb.velocity = new Vector3(targetVelocity.x, rb.velocity.y, targetVelocity.z);
            }
            else
            {
                //Attack if the player is close enough
                AttackPlayer(playerXZPosition);
            }
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
                    // patrolTarget�� ����� ������ ���ο� ��ǥ ���� ����
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
            // ���� �ȿ��� �������� ��Ʈ�� Ÿ�� ����
            float randomX = Random.Range(-patrolRange.x, patrolRange.x);
            float randomZ = Random.Range(-patrolRange.z, patrolRange.z);
            patrolTarget = defaultPosition + new Vector3(randomX, 0, randomZ);
        }
    }

    public void AttackPlayer(Vector3 playerXZPosition)
    {
        if (isAttacking)
        {
            return;
        }
        isAttacking = true;
        animator.SetTrigger("AttackTrig");
        Vector3 direction = (playerXZPosition - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(direction);
        StartCoroutine(AttackStay());
        StartCoroutine(AttackCooldown());
    }

    private IEnumerator AttackStay()
    {
        yield return new WaitForSeconds(0.5f);
        float startTime = Time.time;
        while (Time.time < startTime + attackTime)
        {
            Vector3 forward = transform.forward;
            Vector3 germPosition = transform.position;
            Collider[] hitColliders = Physics.OverlapSphere(germPosition, attackRange, playerLayer);
            foreach (Collider hitCollider in hitColliders)
            {
                GameObject hitObject = hitCollider.gameObject;
                PlayerController playerController = hitObject.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    Vector3 directionToPlayer = hitCollider.transform.position - germPosition;
                    float angle = Vector3.Angle(forward, directionToPlayer);
                    if (angle < attackAngle)
                    {
                        playerController.TakeDamage(damageAmount, transform);
                    }
                }
            }
            yield return new WaitForSeconds(attackTime / 10.0f);
        }
    }

    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    // Damaged by attack
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

    public bool CanBeSteppedOn() => false;

    public void GiveDamage()
    {
        player.GetComponent<PlayerController>().TakeDamage(damageAmount, transform);
    }
}
