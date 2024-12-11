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
        { Powerup.SweetPotato, 3.0f },
        { Powerup.ChiliPepper, 2.5f },
        { Powerup.Carrot, 2.0f },
    };

    private Rigidbody rb;
    private StageManager stageManager;
    private StaminaManager staminaManager;
    private HealthManager healthManager;
    private Camera mainCamera;

    // Default Ground - General
    [SerializeField] float currentSpeed; // for debugging speed
    public float speed = 800.0f;
    public float jumpForce = 200.0f;
    public bool isGrounded = true;
    public bool canMove = true; // for player movement only
    private float horizontalInput = 0.0f;
    private float verticalInput = 0.0f;
    private bool hasJumpInput = false;

    // Animaton
    public Animator animator;
    private bool isMoving = false;
    private bool isJumping = false;
    private bool isFalling = false;
    [SerializeField] float powerupPauseDelay = 1.2f;
    [SerializeField] float showRewardDelay = 1.5f;
    [SerializeField] Vector3 rewardOffset = new Vector3(0.0f, 0.5f, 0.2f);

    // Audio
    //public AudioSource audioSource;
    //public AudioClip groundImpactSound;
    //public AudioClip movementSound;

    // Attack & Damage
    public float attackRange = 2.0f;
    public float attackAngle = 45.0f;
    public float attackTime = 0.5f;
    public int attackDamage = 1;
    public float attackCooldown = 1.0f;
    private bool canAttack = true;
    public LayerMask damageableLayer;
    private bool canTakeDamage = true;
    public float getDamageCooldown = 1.0f;
    public float stunCooldown = 0.5f;
    public float groundNormalThreshold = 30.0f;
    private float pushBackSpeed = 8.0f;
    public float bounceSpeed = 8.0f;

    // Powerup
    private bool canPowerup = true;
    public float powerupCooldown = 0.3f;
    public float powerupDuration = 10.0f;
    public float sweetPotatoAttackRange = 5.0f;
    public int sweetPotatoAttackDamage = 1;
    public float sweetPotatoAttackInterval = 1.0f;
    public bool isInvincible = false;
    public float dashStrength = 2000.0f;
    public float dashSpeed = 12.0f;
    public GameObject carrotPrefab;
    public float carrotSpeed = 10.0f;
    public float[] carrotAngles = { -20f, 0f, 20f };
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
        if (stageManager.CheckGameContinue() && canMove)
        {
            // Jump Input
            if (Input.GetKeyDown(KeyCode.Z) && isGrounded)
            {
                hasJumpInput = true;
            }

            // Attack Input
            if (Input.GetKeyDown(KeyCode.X))
            {                
                PerformAttack();
            }

            // Powerup Input
            if (Input.GetKeyDown(KeyCode.C))
            {
                Powerup currentPowerup = stageManager.currentPowerup;
                if (currentPowerup == Powerup.Carrot)
                {
                    UseCarrotPowerup();
                }
                else if (currentPowerup == Powerup.SweetPotato || currentPowerup == Powerup.ChiliPepper)
                {
                    // Check if can use powerup
                    float cost = staminaCost[currentPowerup];
                    if (staminaManager.CanUsePowerup(cost) && canPowerup)
                    {
                        staminaManager.RunStamina(cost);
                        if (currentPowerup == Powerup.SweetPotato)
                        {
                            currentPowerupCoroutine = StartCoroutine(SweetPotatoHoldCoroutine());
                        }
                        else if (currentPowerup == Powerup.ChiliPepper)
                        {
                            currentPowerupCoroutine = StartCoroutine(ChiliPepperHoldCoroutine());
                        }
                    }
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (stageManager.CheckGameContinue() && canMove)
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
            AudioManager.Instance.PlayJumpSound();
            animator.SetTrigger("jumpTrig");
            animator.SetBool("isGrounded", false);
        }
    }

    // Add gravity direction force to reduce air time
    IEnumerator AddReverseForce()
    {
        yield return new WaitForSeconds(0.2f);
        rb.AddForce(Physics.gravity * 10.0f, ForceMode.Impulse);
    }

    private void PerformAttack()
    {
        if (!canAttack)
        {
            return;
        }
        animator.SetTrigger("attackTrig");
        canAttack = false;
        StartCoroutine(AttackStay());

        Vector3 playerPosition = transform.position;
        Vector3 forward = transform.forward;
        Collider[] wireColliders = Physics.OverlapSphere(playerPosition, attackRange);
        foreach (Collider hit in wireColliders)
        {
            if (hit.CompareTag("Wire")) 
            {
                WireController wire = hit.GetComponent<WireController>();
                if (wire != null)
                {
                    if (Physics.Raycast(playerPosition, forward, out RaycastHit hitInfo, attackRange))
                    {
                        wire.HandleWireAttack(hitInfo.point); 
                    }
                    else
                    {
                        wire.HandleWireAttack(hit.ClosestPoint(playerPosition));
                    }
                }
            }
        }
        StartCoroutine(AttackCooldown());
        StartCoroutine(InputPause(attackTime));
    }

    // Continue hit detection during attackTime
    private IEnumerator AttackStay()
    {
        float startTime = Time.time;
        while (Time.time < startTime + attackTime)
        {
            Vector3 playerPosition = transform.position;
            Vector3 forward = transform.forward;
            Collider[] hitColliders = Physics.OverlapSphere(playerPosition, attackRange, damageableLayer);
            foreach (Collider hitCollider in hitColliders)
            {
                GameObject hitObject = hitCollider.gameObject;
                IDamageable damageableObject = hitObject.GetComponent<IDamageable>();
                if (damageableObject != null)
                {
                    Vector3 directionToEnemy = hitCollider.transform.position - playerPosition;
                    float distanceToEnemy = Vector3.Distance(playerPosition, hitObject.transform.position);
                    float angle = Vector3.Angle(forward, directionToEnemy);
                    if (angle < attackAngle)
                    {
                        damageableObject.TakeDamage(attackDamage);
                    }
                }
            }
            yield return new WaitForSeconds(attackTime / 10.0f);
        }
        
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

    private void UpdateAnimationAndSound(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            animator.SetBool("isMoving", true);
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f);
            /*if (!audioSource.isPlaying && isGrounded)
            {
                audioSource.clip = movementSound;
                audioSource.Play();
            }*/
        }
        else
        {
            animator.SetBool("isMoving", false);
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

    public void ResetPowerupSettings()
    {
        if (currentPowerupCoroutine != null)
        {
            StopCoroutine(currentPowerupCoroutine);
            currentPowerupCoroutine = null;
        }
        speed = originalSpeed;
        isInvincible = false;
    }

    private void UseCarrotPowerup()
    {
        float powerupStaminaCost = staminaCost[stageManager.currentPowerup];
        if (!staminaManager.CanUsePowerup(powerupStaminaCost) || !canPowerup) return;
        staminaManager.RunStamina(powerupStaminaCost);
        UseCarrot();
        StartCoroutine(PowerupCooldown());
    }

    private IEnumerator SweetPotatoHoldCoroutine()
    {
        while (Input.GetKey(KeyCode.C) && !staminaManager.isEmpty())
        {
            UseSweetPotato();
            staminaManager.RunStamina(staminaCost[Powerup.SweetPotato] / 50.0f);
            yield return new WaitForSeconds(holdPowerupStaminaCooldown / 50.0f);
        }
        ResetPowerupSettings();
    }

    private IEnumerator ChiliPepperHoldCoroutine()
    {
        isInvincible = true;
        speed = dashSpeed;
        while (Input.GetKey(KeyCode.C) && !staminaManager.isEmpty())
        {
            staminaManager.RunStamina(staminaCost[Powerup.ChiliPepper] / 50.0f);
            yield return new WaitForSeconds(holdPowerupStaminaCooldown / 50.0f);
        }
        ResetPowerupSettings();
    }

    private void UseSweetPotato()
    {
        Vector3 playerPosition = transform.position;
        Collider[] hitColliders = Physics.OverlapSphere(playerPosition, sweetPotatoAttackRange, damageableLayer);
        foreach (Collider hitCollider in hitColliders)
        {
            GameObject hitObject = hitCollider.gameObject;
            IDamageable damageable = hitObject.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(sweetPotatoAttackDamage);
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

            Rigidbody carrotRb = carrot.GetComponent<Rigidbody>();
            if (carrotRb != null)
            {
                carrotRb.velocity = direction * carrotSpeed;
            }
        }
    }

    public void TakeDamage(int amount)
    {
        if (!canTakeDamage || !stageManager.CheckGameContinue())
        {
            return;
        }

        canTakeDamage = false;
        stageManager.LoseHp(amount);

        if (stageManager.hp == 0)
        {
            stageManager.GameOver();
            animator.SetBool("isDead", true);
        }
        else
        {
            AudioManager.Instance.PlayDamagedSound();
            animator.SetTrigger("damagedTrig");
            StartCoroutine(DamageCooldown());
            StartCoroutine(MovePause(stunCooldown));
        }
    }

    private IEnumerator DamageCooldown()
    {
        yield return new WaitForSeconds(getDamageCooldown);
        canTakeDamage = true;
    }

    public void LookAtCamera(GameObject optionCam)
    {
        if (optionCam != null)
        {
            transform.LookAt(optionCam.transform);
        }
        else
        {
            transform.LookAt(mainCamera.transform);
        }
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
                //if(audioSource != null && audioSource.isActiveAndEnabled)
                //    audioSource.PlayOneShot(groundImpactSound);
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
            if (isInvincible)
            {
                if (enemy != null)
                {
                    enemy.TakeDamage(1);
                }
            }
            else if (validContacts >= collision.contacts.Length / 2)
            {
                if (enemy != null)
                {
                    enemy.TakeDamage(1);
                    rb.velocity = new Vector3(rb.velocity.x, bounceSpeed, rb.velocity.z);
                }
            }
            else
            {
                if (enemy != null)
                {
                    enemy.GiveDamage();

                    Vector3 pushDirection = collision.transform.position - transform.position;
                    pushDirection.y = 0;
                    pushDirection.Normalize();
                    rb.velocity = rb.velocity - pushDirection * pushBackSpeed;
                }
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

            animator.SetBool("isGameClear", true);
            stageManager.GameClear();

            StartCoroutine(ShowRewardLater(other.gameObject));
        }

        if (other.gameObject.CompareTag("Powerup"))
        {
            Destroy(other.gameObject);
            stageManager.HealHp(stageManager.maxHp);
            stageManager.UpdatePowerup(other.gameObject.name);

            LookAtCamera(null);
            animator.SetTrigger("powerupTrig");
            rb.velocity = Vector3.zero;
            StartCoroutine(stageManager.StageFreeze(powerupPauseDelay));
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

    public IEnumerator InputPause(float moveDelay)
    {
        canMove = false;
        yield return new WaitForSeconds(moveDelay);
        canMove = true;
    }

    public IEnumerator MovePause(float moveDelay)
    {
        rb.velocity = Vector3.zero;
        canMove = false;
        yield return new WaitForSeconds(moveDelay);
        canMove = true;
    }
}