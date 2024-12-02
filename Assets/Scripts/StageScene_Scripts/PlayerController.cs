using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    // Rigid Body
    // Mass: 20
    // Drag: 0.5, Angular Drag: 0.05
    // Freeze Rotation: X, Y, Z
    private Rigidbody rb;
    private StageManager stageManager;
    private StaminaManager staminaManager;
    private HealthManager healthManager;
    private Camera mainCamera;

    // Default Ground - General
    [SerializeField] float currentSpeed; // for debugging speed
    public float speed = 8.0f;
    public float jumpForce = 200.0f;
    public bool isGrounded = true;
    private float horizontalInput = 0.0f;
    private float verticalInput = 0.0f;
    private bool hasJumpInput = false;
    public float groundNormalThreshold = 30.0f;
    private float pushBackSpeedThreshold = 2.0f;
    private float pushBackVelocity = 6.0f;

    // Animaton
    public Animator animator;
    private bool isMoving = false;
    private bool isJumping = false;
    private bool isFalling = false;
    [SerializeField] float powerupPauseDelay = 1.2f;
    [SerializeField] float showRewardDelay = 1.5f;
    [SerializeField] Vector3 rewardOffset = new Vector3(0.0f, 0.5f, 0.2f);

    // Audio
    public AudioSource audioSource;
    public AudioClip jumpSound;
    public AudioClip groundImpactSound;
    public AudioClip movementSound;

    // Attack
    public float attackRange = 2.0f;
    public float attackAngle = 90.0f;
    public int attackDamage = 1;
    public float attackCooldown = 1.0f;
    private bool canAttack = true;
    public LayerMask damageableLayer;

    // Particles
    public ParticleSystem dirtTrail;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        stageManager = GameObject.Find("StageManager").GetComponent<StageManager>();
        staminaManager = GameObject.Find("Stamina Bar").GetComponent<StaminaManager>();
        healthManager = GameObject.Find("Health").GetComponent<HealthManager>();
        animator = transform.GetChild(0).GetComponent<Animator>();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (stageManager.CheckGameContinue())
        {
            // Jump Input
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                hasJumpInput = true;
            }

            // For debugging stamina behavior
            if (Input.GetKeyDown(KeyCode.X))
            {
                staminaManager.RunStamina(2.0f);
            }

            // Attack Input
            if (Input.GetKeyDown(KeyCode.Z))
            {                
                PerformAttack();
            }
        }
    }

    void FixedUpdate()
    {
        if (stageManager.CheckGameContinue())
        {
            HandleMovement();
            HandleJump();
            UpdateEffects();
        }
    }

    private void HandleMovement()
    {
        // Movement Input
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        currentSpeed = Mathf.Clamp(Math.Abs(horizontalInput) + Math.Abs(verticalInput), 0f, 1f) * speed;

        Vector3 forwardFromCamera = mainCamera.transform.TransformDirection(Vector3.forward);
        forwardFromCamera.y = 0.0f;
        Vector3 rightFromCamera = mainCamera.transform.TransformDirection(Vector3.right);
        rightFromCamera.y = 0.0f;

        Vector3 direction = (verticalInput * forwardFromCamera + horizontalInput * rightFromCamera).normalized;
        Vector3 velocity = new Vector3(0.0f, rb.velocity.y, 0.0f) + direction * currentSpeed;

        rb.velocity = velocity;
        UpdateAnimationAndSound(direction);
    }

    private void HandleJump()
    {
        if (hasJumpInput)
        {
            hasJumpInput = false;
            isGrounded = false;
            isJumping = true;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            StartCoroutine(AddReverseForce());
            //audioSource.PlayOneShot(jumpSound);
            animator.SetTrigger("jumpTrig");
            animator.SetBool("isGrounded", false);
        }
    }

    private void PerformAttack()
    {
        if (!canAttack)
        {
            return;
        }
        animator.SetTrigger("attackTrig");
        canAttack = false;
        Vector3 playerPosition = transform.position;
        Vector3 forward = transform.forward;
        Collider[] hitColliders = Physics.OverlapSphere(playerPosition, attackRange, damageableLayer);
        IDamageable closestDamageableObject = null;
        float closestDistance = float.MaxValue;
        foreach (Collider hitCollider in hitColliders)
        {
            GameObject hitObject = hitCollider.gameObject;
            IDamageable damageableObject = hitObject.GetComponent<IDamageable>();
            if (damageableObject != null)
            {
                float distanceToEnemy = Vector3.Distance(playerPosition, hitObject.transform.position);
                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestDamageableObject = damageableObject;
                }
            }
        }
        if (closestDamageableObject != null)
        {
            Debug.Log(closestDamageableObject);
            closestDamageableObject.TakeDamage(attackDamage);
        }
        StartCoroutine(AttackCooldown());
    }   

    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    // Add gravity direction force to reduce air time
    IEnumerator AddReverseForce()
    {
        yield return new WaitForSeconds(0.2f);
        rb.AddForce(Physics.gravity * 10.0f, ForceMode.Impulse);
    }

    private void UpdateAnimationAndSound(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            animator.SetBool("isMoving", true);
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f);
            /*if (!audioSource.isPlaying)
            {
                audioSource.clip = movementSound;
                audioSource.loop = true;
                audioSource.Play();
            }*/
        }
        else
        {
            animator.SetBool("isMoving", false);
            //audioSource.Stop();
            //audioSource.loop = false;
        }

        // Check falling state
        if (!isJumping && !isFalling && rb.velocity.y < -2.0f)
        {
            isFalling = true;
            isGrounded = false;
            animator.SetBool("isGrounded", false);
            animator.SetTrigger("fallTrig");
        }
    }

    // Particle Effects
    private void UpdateEffects()
    {
        if (isMoving)
        {
            /*
            if (!dirtTrail.isPlaying)
            {
                dirtTrail.Play();
            }
            */
        }
        else
        {
            //dirtTrail.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }

    private void LookAtCamera()
    {
        transform.LookAt(mainCamera.transform);
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            int validContacts = 0;
            foreach (ContactPoint contact in collision.contacts)
            {
                float angle = Vector3.Angle(contact.normal, Vector3.up);
                if (angle < groundNormalThreshold)
                {
                    validContacts++;
                }
            }
            if (validContacts >= collision.contacts.Length / 2)
            {
                isGrounded = true;
                isFalling = false;
                isJumping = false;
                animator.SetBool("isGrounded", true);
            }
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            int validContacts = 0;
            foreach (ContactPoint contact in collision.contacts)
            {
                float angle = Vector3.Angle(contact.normal, Vector3.up);
                if (angle < groundNormalThreshold)
                {
                    validContacts++;
                }
            }
            Debug.Log(validContacts + " / " + collision.contacts.Length);
            IEnemy enemy = collision.gameObject.GetComponent<IEnemy>();
            if (validContacts >= collision.contacts.Length / 2)
            {
                if (enemy != null)
                {
                    enemy.TakeDamage(1);
                }
            }
            else{
                if (enemy != null)
                {
                    enemy.GiveDamage();
                }
            }
            Vector3 currentVelocity = rb.velocity;
            if (currentVelocity.magnitude > pushBackSpeedThreshold)
            {
                rb.velocity = -currentVelocity;
            }
            else
            {
                Vector3 pushDirection = (transform.position - collision.transform.position).normalized;
                rb.velocity = -pushDirection * pushBackVelocity;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            Destroy(other.gameObject);
            stageManager.AddCoins(1);
            stageManager.HealHp(1);
        }

        if (other.gameObject.CompareTag("Recipe"))
        {
            Destroy(other.gameObject);
            stageManager.ObtainRecipe();
        }

        if (other.gameObject.CompareTag("Reward"))
        {
            other.gameObject.SetActive(false);
            rb.velocity = Vector3.zero;

            LookAtCamera();
            animator.SetBool("isGameClear", true);
            stageManager.GameClear();

            StartCoroutine(ShowRewardLater(other.gameObject));
        }

        if (other.gameObject.CompareTag("Powerup"))
        {
            Destroy(other.gameObject);
            stageManager.HealHp(stageManager.maxHp);
            stageManager.UpdatePowerup(other.gameObject.name);

            LookAtCamera();
            animator.SetTrigger("powerupTrig");
            StartCoroutine(PowerupPause());
        }
    }

    private IEnumerator ShowRewardLater(GameObject reward)
    {
        yield return new WaitForSeconds(showRewardDelay);
        reward.transform.localScale *= 0.5f;
        reward.transform.position = transform.position + rewardOffset;
        reward.GetComponent<SpinningItems>().startY = reward.gameObject.transform.position.y;
        reward.SetActive(true);
    }

    private IEnumerator PowerupPause()
    {
        rb.velocity = Vector3.zero;
        stageManager.canMove = false;
        yield return new WaitForSeconds(powerupPauseDelay);
        stageManager.canMove = true;
    }
}