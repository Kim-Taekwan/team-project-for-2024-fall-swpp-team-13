using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Cinemachine;

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
    private HPScript hpScript;
    private Camera mainCamera;

    // Ground Interaction
    [Header("Ground Interaction")]
    [SerializeField] float currentSpeed; // for debugging speed
    public float speed = 8.0f;
    public float jumpForce = 250.0f;
    public bool isGrounded = true;
    public bool canMove = true; // for player movement only
    private float horizontalInput = 0.0f;
    private float verticalInput = 0.0f;
    private bool hasJumpInput = false;
    private GroundChecker groundChecker;
    private BoxCollider boxCollider;
    public PhysicMaterial slip;

    // Animation
    [Header("Animation")]
    public Animator animator;
    private bool isMoving = false;
    private bool isJumping = false;
    private bool isFalling = false;
    [SerializeField] float powerupPauseDelay = 1.2f;
    [SerializeField] float showRewardDelay = 1.5f;
    [SerializeField] Vector3 rewardOffset = new Vector3(0.0f, 0.5f, 0.2f);

    // Attack & Damage
    [Header("Attack & Damage")]
    public float attackRange = 2.0f;
    public float attackAngle = 45.0f;
    public float attackTime = 0.5f;
    public int attackDamage = 1;
    public float attackCooldown = 1.0f;
    private bool canAttack = true;
    public LayerMask damageableLayer;
    private bool canTakeDamage = true;
    private float damageCoolTime = 2.0f;
    public float stunCoolTime = 0.5f;
    public float groundNormalThreshold = 30.0f;
    private float pushBackSpeed = 8.0f;
    public float bounceSpeed = 8.0f;

    // Power Up
    [Header("Power Up")]
    [SerializeField] bool canPowerup = true;
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
    public float originalSpeed;
    public float holdPowerupStaminaCooldown = 1.0f;
    public GameObject sweetPotatoEffect;
    public GameObject chiliPepperEffect;

    public GameObject dustPrefab;
    public Vector3 carrotYOffset = new Vector3(0, 0.5f, 0);
    public bool isUsingPowerup = false;

    // Particles
    //[Header("Particles")]
    // public GameObject dirtTrail;

    // Cinemachine
    public CinemachineVirtualCamera VC1; 
    public float requiredPosition = 1.0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        stageManager = GameObject.Find("StageManager").GetComponent<StageManager>();
        staminaManager = GameObject.Find("Stamina Bar").GetComponent<StaminaManager>();
        healthManager = GameObject.Find("Health").GetComponent<HealthManager>();
        animator = transform.GetChild(0).GetComponent<Animator>();
        hpScript = GetComponent<HPScript>();
        groundChecker = GetComponent<GroundChecker>();
        boxCollider = GetComponent<BoxCollider>();
        mainCamera = Camera.main;
        originalSpeed = speed;
    }

    private void Update()
    {
        if (stageManager.CheckGameContinue() && canMove)
        {
            // Jump Input
            if (Input.GetKeyDown(KeyCode.Z) && groundChecker.IsGrounded())
            {
                hasJumpInput = true;
            }

            // Attack Input
            if (Input.GetKeyDown(KeyCode.X))
            {                
                Attack();
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
                    float cost = staminaCost[currentPowerup] / 50.0f;
                    if (staminaManager.CanUsePowerup(cost) && canPowerup)
                    {
                        isUsingPowerup = true;
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

    private bool DollyTrackReached()
    {
        if (VC1 != null)
        {
            var dolly = VC1.GetCinemachineComponent<CinemachineTrackedDolly>();
            return dolly != null && dolly.m_PathPosition >= requiredPosition;
        }
        return true;
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

        if(isGrounded && (velocity.magnitude > 0.1f)) StartCoroutine(PlayDustAndTrailEffect());
        //Instantiate(dustPrefab, transform.position, Quaternion.identity);
        UpdateAnimation(direction);
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
            boxCollider.material = slip;
        }
        // Check falling state
        else if (!isJumping && !isFalling && rb.velocity.y < -2.0f)
        {
            isGrounded = false;
            isFalling = true;
            animator.SetBool("isGrounded", false);
            animator.SetTrigger("fallTrig");
            boxCollider.material = null;
        }
        // Check grounded state
        else if ((isJumping || isFalling) && groundChecker.IsGrounded() && rb.velocity.y < 0.0f)
        {
            isGrounded = true;
            isFalling = false;
            isJumping = false;
            animator.SetBool("isGrounded", true);
            boxCollider.material = null;
        }
    }

    IEnumerator PlayDustAndTrailEffect()
    {
        if(UnityEngine.Random.Range(0, 2) == 0){
            GameObject dust = Instantiate(dustPrefab, transform.position - transform.forward * 0.2f + new Vector3(0.0f, 0.2f, 0.0f), Quaternion.identity);
            //GameObject trail = Instantiate(dirtTrail, transform.position - transform.forward * 0.2f + new Vector3(0.0f, 0.2f, 0.0f), Quaternion.identity);
            yield return new WaitForSeconds(0.1f);
            Destroy(dust);
            //Destroy(trail);
        }
    }

    // Add gravity direction force to reduce air time
    IEnumerator AddReverseForce()
    {
        yield return new WaitForSeconds(0.2f);
        rb.AddForce(Physics.gravity * 10.0f, ForceMode.Impulse);
    }

    private void Attack()
    {
        if (!canAttack)
        {
            return;
        }
        canAttack = false;
        animator.SetTrigger("attackTrig");
        StartCoroutine(AttackStay());
        StartCoroutine(AttackCooldown());
    }

    // Continue hit detection during attackTime
    private IEnumerator AttackStay()
    {
        float startTime = Time.time;
        canMove = false;
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
                    float angle = Vector3.Angle(forward, directionToEnemy);
                    if (angle < attackAngle)
                    {
                        damageableObject.TakeDamage(attackDamage);
                    }
                }
            }
            yield return new WaitForSeconds(attackTime / 10.0f);
        }
        canMove = true;
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

    private void UpdateAnimation(Vector3 direction)
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
    }

    public void ResetPowerupSettings()
    {
        if (currentPowerupCoroutine != null)
        {
            StopCoroutine(currentPowerupCoroutine);
            currentPowerupCoroutine = null;
        }
        animator.SetBool("isSweetPotato", false);
        speed = originalSpeed;
        isInvincible = false;
        isUsingPowerup = false;
        canMove = true;
    }

    private void UseCarrotPowerup()
    {
        float powerupStaminaCost = staminaCost[stageManager.currentPowerup];
        if (!staminaManager.CanUsePowerup(powerupStaminaCost) || !canPowerup) return;
        staminaManager.RunStamina(powerupStaminaCost);
        UseCarrot();
        animator.SetTrigger("shootCarrot");
        StartCoroutine(PowerupCooldown());
    }

    private IEnumerator SweetPotatoHoldCoroutine()
    {
        canMove = false;
        animator.SetBool("isSweetPotato", true);
        GameObject effect = Instantiate(sweetPotatoEffect, transform.position, Quaternion.identity);
        effect.transform.SetParent(transform);
        while (Input.GetKey(KeyCode.C) && !staminaManager.isEmpty())
        {
            UseSweetPotato();
            staminaManager.RunStamina(staminaCost[Powerup.SweetPotato] / 50.0f);
            yield return new WaitForSeconds(holdPowerupStaminaCooldown / 50.0f);
        }
        Destroy(effect);
        ResetPowerupSettings();
    }

    private IEnumerator ChiliPepperHoldCoroutine()
    {
        isInvincible = true;
        speed = dashSpeed;
        GameObject effect = Instantiate(chiliPepperEffect, transform.position, Quaternion.identity);
        effect.transform.SetParent(transform);
        while (Input.GetKey(KeyCode.C) && !staminaManager.isEmpty())
        {
            staminaManager.RunStamina(staminaCost[Powerup.ChiliPepper] / 50.0f);
            yield return new WaitForSeconds(holdPowerupStaminaCooldown / 50.0f);
        }
        Destroy(effect);
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

            GameObject carrot = Instantiate(carrotPrefab, playerPosition + direction * 1.0f + carrotYOffset, rotation);

            Rigidbody carrotRb = carrot.GetComponent<Rigidbody>();
            if (carrotRb != null)
            {
                carrotRb.velocity = direction * carrotSpeed;
            }
        }
    }

    public void TakeDamage(int amount, Transform attackTransform = null)
    {
        if (!canTakeDamage)
        {
            return;
        }
        canTakeDamage = false;
        hpScript.ChangeHP(transform.position);
        stageManager.LoseHp(amount);

        if (attackTransform != null)
        {
            Vector3 pushDirection =  transform.position - attackTransform.position;
            pushDirection.y = 0;
            pushDirection.Normalize();
            rb.velocity = rb.velocity + pushDirection * pushBackSpeed;
        }        

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
            StartCoroutine(MovePause(stunCoolTime));
        }
    }

    private IEnumerator DamageCooldown()
    {
        float startTime = Time.time;
        yield return new WaitForSeconds(0.3f);
        
        bool isOrigin = true;
        List<Material> playerMaterials = new List<Material>();
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            playerMaterials.AddRange(renderer.materials);
        }

        // Blink player's color during damage cool time
        while (Time.time < startTime + damageCoolTime)
        {
            foreach(Material material in playerMaterials)
            {
                material.color *= (isOrigin ? 0.8f : 1.25f);
            }
            isOrigin = !isOrigin;
            yield return new WaitForSeconds(0.1f);
        }

        // Make sure to return original color when exits
        if (!isOrigin)
        {
            foreach (Material material in playerMaterials)
            {
                material.color *= 1.25f;
            }
        }        
        canTakeDamage = true;
    }

    public void LookAtCamera(GameObject optionCam = null)
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

    private void OnCollisionStay(Collision collision)
    {
        if (!stageManager.CheckGameContinue())
        {
            return;
        }

        if (collision.gameObject.CompareTag("Enemy"))
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
            if (enemy == null)
            {
                return;
            }

            if (isInvincible)
            {
                enemy.TakeDamage(1);
            }
            else if (validContacts >= collision.contacts.Length / 2)
            {
                if (enemy.CanBeSteppedOn())
                {
                    enemy.TakeDamage(1);
                    rb.velocity = new Vector3(rb.velocity.x, bounceSpeed, rb.velocity.z);
                }
                else
                {
                    enemy.GiveDamage();
                }
            }
            else
            {
                enemy.GiveDamage();
            }
        }

        if (collision.gameObject.CompareTag("Obstacle"))
        {
            TakeDamage(1, collision.transform);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!stageManager.CheckGameContinue())
        {
            return;
        }

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
            ResetPowerupSettings();
            canMove = false;

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
        reward.transform.Find("RewardObject").GetComponent<SpinningItems>().startY = reward.gameObject.transform.position.y;
        reward.SetActive(true);
    }

    public IEnumerator MovePause(float moveDelay)
    {
        canMove = false;
        yield return new WaitForSeconds(moveDelay);
        canMove = true;
    }

    public IEnumerator MoveFreeze(float moveDelay)
    {
        rb.velocity = Vector3.zero;
        canMove = false;
        yield return new WaitForSeconds(moveDelay);
        canMove = true;
    }
}