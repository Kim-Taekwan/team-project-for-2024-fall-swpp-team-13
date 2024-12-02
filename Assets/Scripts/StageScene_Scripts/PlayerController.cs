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

    public Dictionary<Powerup, float> staminaCost = new Dictionary<Powerup, float>
    {
        { Powerup.SweetPotato, 1.0f },
        { Powerup.ChiliPepper, 1.0f },
        { Powerup.Carrot, 1.0f },
        { Powerup.Ice, 1.0f }
    };

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
    private float pushBackVelocity = 8.0f;

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
    public float attackAngle = 45.0f;
    public int attackDamage = 1;
    public float attackCooldown = 1.0f;
    private bool canAttack = true;
    public LayerMask damageableLayer;

    // Powerup
    private bool canPowerup = true;
    public float powerupCooldown = 1.0f;
    public float powerupDuration = 10.0f;
    public float sweetPotatoAttackRange = 5.0f;
    public int sweetPotatoAttackDamage = 1;
    public float sweetPotatoAttackInterval = 1.0f;
    public bool isInvincible = false;
    public float dashStrength = 2000.0f;
    public float dashSpeed = 12.0f;
    public GameObject carrotPrefab;
    public float carrotSpeed = 10.0f;
    public float[] carrotAngles = { -30f, 0f, 30f };
    private Coroutine currentPowerupCoroutine;
    private float originalSpeed;
    public float holdPowerupStaminaCooldown = 1.0f;

    public Vector3 yOffset = new Vector3(0, 0.5f, 0);

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
        originalSpeed = speed;
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
            // if (Input.GetKeyDown(KeyCode.X))
            // {
            //     staminaManager.RunStamina();
            // }

            // Attack Input
            if (Input.GetKeyDown(KeyCode.Z))
            {                
                PerformAttack();
            }

            // Powerup Input
            if (Input.GetKeyDown(KeyCode.C))
            {
                Powerup current = stageManager.currentPowerup;
                if (current == Powerup.Carrot || current == Powerup.Ice)
                {
                    UsePowerup();
                }
                else if (current == Powerup.SweetPotato || current == Powerup.ChiliPepper)
                {
                    // Check if can use powerup
                    float cost = staminaCost[current];
                    if (staminaManager.CanUsePowerup(cost) && canPowerup)
                    {
                        staminaManager.RunStamina(cost);
                        if (current == Powerup.SweetPotato)
                        {
                            currentPowerupCoroutine = StartCoroutine(SweetPotatoHoldCoroutine());
                        }
                        else if (current == Powerup.ChiliPepper)
                        {
                            currentPowerupCoroutine = StartCoroutine(ChiliPepperHoldCoroutine());
                        }
                    }
                }
            }

            if (Input.GetKeyUp(KeyCode.C))
            {
                Powerup current = stageManager.currentPowerup;
                if (current == Powerup.SweetPotato || current == Powerup.ChiliPepper)
                {
                    if (currentPowerupCoroutine != null)
                    {
                        StopCoroutine(currentPowerupCoroutine);
                        currentPowerupCoroutine = null;
                    }

                    if (current == Powerup.ChiliPepper)
                    {
                        speed = originalSpeed;
                        isInvincible = false;
                    }
                }
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
                Vector3 directionToEnemy = hitCollider.transform.position - playerPosition;
                float distanceToEnemy = Vector3.Distance(playerPosition, hitObject.transform.position);
                float angle = Vector3.Angle(forward, directionToEnemy);
                if (distanceToEnemy < closestDistance && angle < attackAngle)
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

    private IEnumerator PowerupCooldown()
    {
        canPowerup = false;
        yield return new WaitForSeconds(powerupCooldown);
        canPowerup = true;
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
            if (validContacts >= collision.contacts.Length / 2 || isInvincible)
            {
                if (enemy != null)
                {
                    enemy.TakeDamage(1);
                }
            }
            else
            {
                if (enemy != null)
                {
                    enemy.GiveDamage();
                }
            }
            Vector3 pushDirection = collision.transform.position - transform.position;
            pushDirection.y = 0;
            pushDirection.Normalize();
            rb.velocity = rb.velocity - pushDirection * pushBackVelocity;
        }
    }

    private void UsePowerup()
    {
        float powerupStaminaCost = staminaCost[stageManager.currentPowerup];
        if (!staminaManager.CanUsePowerup(powerupStaminaCost) || !canPowerup) return;
        switch (stageManager.currentPowerup)
        {
            case Powerup.Carrot:
                staminaManager.RunStamina(powerupStaminaCost);
                UseCarrot();
                break;
            case Powerup.Ice:
                staminaManager.RunStamina(powerupStaminaCost);
                UseIce();
                break;
            default:
                break;
        }
        StartCoroutine(PowerupCooldown());
    }

    private IEnumerator SweetPotatoHoldCoroutine()
    {
        while (Input.GetKey(KeyCode.C) && canPowerup)
        {
            UseSweetPotato();
            staminaManager.RunStamina(staminaCost[Powerup.SweetPotato]);
            yield return new WaitForSeconds(holdPowerupStaminaCooldown);
        }
    }

    private IEnumerator ChiliPepperHoldCoroutine()
    {
        isInvincible = true;
        speed = dashSpeed;
        while (Input.GetKey(KeyCode.C) && canPowerup)
        {
            staminaManager.RunStamina(staminaCost[Powerup.ChiliPepper]);
            yield return new WaitForSeconds(holdPowerupStaminaCooldown);
        }
        speed = originalSpeed;
        isInvincible = false;
    }

    private void UseSweetPotato()
    {
        Vector3 playerPosition = transform.position;
        Collider[] hitColliders = Physics.OverlapSphere(playerPosition, sweetPotatoAttackRange, enemyLayer);
        foreach (Collider hitCollider in hitColliders)
        {
            GameObject hitObject = hitCollider.gameObject;
            IEnemy enemy = hitObject.GetComponent<IEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(sweetPotatoAttackDamage);
            }
        }
    }


    private void UseCarrot()
    {
        Vector3 playerPosition = transform.position;
        Quaternion playerRotation = transform.rotation;
        foreach (float angle in carrotAngles)
        {
            Quaternion rotation = playerRotation * Quaternion.Euler(0, angle, 0);
            Vector3 direction = rotation * Vector3.forward;

            GameObject carrot = Instantiate(carrotPrefab, playerPosition + direction * 1.0f + yOffset, rotation);

            Rigidbody rb = carrot.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = direction * carrotSpeed;
            }
        }
    }

    private void ActivateEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            IEnemy enemyScript = enemy.GetComponent<IEnemy>();
            if (enemyScript != null)
            {
                enemyScript.ActivateEnemy();
            }
        }
    }

    public void DeactivateEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            IEnemy enemyScript = enemy.GetComponent<IEnemy>();
            if (enemyScript != null)
            {
                enemyScript.DeactivateEnemy();
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
            DeactivateEnemies();
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