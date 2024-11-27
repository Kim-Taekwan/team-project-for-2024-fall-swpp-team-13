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
        { Powerup.SweetPotato, 10.0f },
        { Powerup.ChiliPepper, 1.0f },
        { Powerup.Carrot, 1.0f },
        { Powerup.Ice, 1.0f }
    };

    private Rigidbody rb;
    private StageManager stageManager;
    private StaminaManager staminaManager;
    private HealthManager healthManager;

    // Default Ground - General
    [SerializeField] float currentSpeed; // for debugging speed
    public float speed = 8.0f;
    public float jumpForce = 200.0f;
    public bool isGrounded = true;
    private float moveHorizontal = 0.0f;
    private float moveVertical = 0.0f;
    private bool hasJumpInput = false;
    public float groundNormalThreshold = 30.0f;
    private float pushBackSpeedThreshold = 2.0f;
    private float pushBackVelocity = 8.0f;

    // Ice Ground
    /*public bool isOnIce = false;
    public float iceAcceleration = 1.5f;
    public float maxIceSpeed = 5.0f;
    private Vector3 iceVelocity = Vector3.zero;
    public float iceDeceleration = 0.001f;*/

    // Animator
    private Animator animator;
    private bool isMoving = false;
    private bool isJumping = false;
    private bool isFalling = false;

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
    public LayerMask enemyLayer;

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

    public Vector3 yOffset = new Vector3(0, 0.5f, 0);

    // Particles
    //public ParticleSystem iceTrail;
    public ParticleSystem dirtTrail;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        stageManager = GameObject.Find("StageManager").GetComponent<StageManager>();
        staminaManager = GameObject.Find("Stamina Bar").GetComponent<StaminaManager>();
        healthManager = GameObject.Find("Health").GetComponent<HealthManager>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (stageManager.CheckGameContinue() && stageManager.CanMove())
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
                animator.SetTrigger("attackTrig");
                PerformAttack();
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                UsePowerup();
            }
        }
    }

    void FixedUpdate()
    {
        if (stageManager.CheckGameContinue())
        {
            if (stageManager.CanMove())
            {
                HandleMovement();
                HandleJump();
            }
            UpdateEffects();
        }
    }

    private void HandleMovement()
    {
        // Movement Input
        moveHorizontal = Input.GetAxis("Horizontal");
        moveVertical = Input.GetAxis("Vertical");

        Vector3 velocity = new Vector3(moveHorizontal * speed, rb.velocity.y, moveVertical * speed);
        Vector3 direction = new Vector3(moveHorizontal, 0.0f, moveVertical).normalized;
        currentSpeed = new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;

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
        canAttack = false;
        Vector3 playerPosition = transform.position;
        Vector3 forward = transform.forward;
        Collider[] hitColliders = Physics.OverlapSphere(playerPosition, attackRange, enemyLayer);
        IEnemy closestEnemy = null;
        float closestDistance = float.MaxValue;
        foreach (Collider hitCollider in hitColliders)
        {
            GameObject hitObject = hitCollider.gameObject;
            IEnemy enemy = hitObject.GetComponent<IEnemy>();
            if (enemy != null)
            {
                Vector3 directionToEnemy = hitCollider.transform.position - playerPosition;
                float distanceToEnemy = Vector3.Distance(playerPosition, hitObject.transform.position);
                float angle = Vector3.Angle(forward, directionToEnemy);
                if (distanceToEnemy < closestDistance && angle < attackAngle)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = enemy;
                }
            }
        }
        if (closestEnemy != null)
        {
            closestEnemy.TakeDamage(attackDamage);
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
        //animator.SetFloat("Speed_f", currentSpeed);
        //animator.SetBool("Moving_b", movement != Vector3.zero);

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
            if (isOnIce && !iceTrail.isPlaying)
            {
                iceTrail.Play();
            }
            else if (!isOnIce && !dirtTrail.isPlaying)
            {
                dirtTrail.Play();
            }
            */
        }
        else
        {
            //iceTrail.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            //dirtTrail.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
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
            Vector3 currentVelocity = rb.velocity;
            if (currentVelocity.magnitude > pushBackSpeedThreshold)
            {
                rb.velocity = -currentVelocity;
            }
            else
            {
                Vector3 pushDirection = (transform.position - collision.transform.position).normalized;
                rb.velocity = -currentVelocity.normalized * pushBackVelocity;
            }
        }
    }

    private void UsePowerup()
    {
        float powerupStaminaCost = staminaCost[stageManager.currentPowerup];
        if (!staminaManager.CanUsePowerup(powerupStaminaCost) || !canPowerup) return;
        switch (stageManager.currentPowerup)
        {
            case Powerup.SweetPotato:
                staminaManager.RunStamina(powerupStaminaCost);
                StartCoroutine(SweetPotatoCoroutine());
                break;
            case Powerup.ChiliPepper:
                staminaManager.RunStamina(powerupStaminaCost);
                StartCoroutine(ChiliPepperCourtine());
                break;
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

    private IEnumerator SweetPotatoCoroutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < powerupDuration)
        {
            UseSweetPotato();
            elapsedTime += sweetPotatoAttackInterval;

            yield return new WaitForSeconds(sweetPotatoAttackInterval);
        }
    }

    private IEnumerator ChiliPepperCourtine()
    {
        isInvincible = true;
        float originalSpeed = speed;
        speed = dashSpeed;
        yield return new WaitForSeconds(powerupDuration);
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

    private void UseIce()
    {

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

    private void DeactivateEnemies()
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
        }

        if (other.gameObject.CompareTag("Recipe"))
        {
            Destroy(other.gameObject);
            stageManager.ObtainRecipe();
        }

        if (other.gameObject.CompareTag("Reward"))
        {
            Destroy(other.gameObject);
            rb.velocity = Vector3.zero;
            DeactivateEnemies();
            stageManager.GameClear();
        }

        if (other.gameObject.CompareTag("Powerup"))
        {
            Destroy(other.gameObject);
            stageManager.UpdatePowerup(other.gameObject.name);
        }
    }
}