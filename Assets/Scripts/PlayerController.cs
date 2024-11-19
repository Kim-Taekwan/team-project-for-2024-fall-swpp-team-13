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

    // Default Ground - General
    [SerializeField] float currentSpeed; // for debugging speed
    public float speed = 8.0f;
    public float jumpForce = 200.0f;
    public bool isGrounded = true;
    private float moveHorizontal = 0.0f;
    private float moveVertical = 0.0f;
    private bool hasJumpInput = false;

    // Ice Ground
    /*public bool isOnIce = false;
    public float iceAcceleration = 1.5f;
    public float maxIceSpeed = 5.0f;
    private Vector3 iceVelocity = Vector3.zero;
    public float iceDeceleration = 0.001f;*/

    // Animator
    private Animator animator;
    private bool isMoving = false;

    // Audio
    public AudioSource audioSource;
    public AudioClip jumpSound;
    public AudioClip groundImpactSound;
    public AudioClip movementSound;

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
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            StartCoroutine(AddReverseForce());
            //audioSource.PlayOneShot(jumpSound);
            //animator.SetTrigger("Jump_trig");
        }
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
            isGrounded = true;
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
            stageManager.GameClear();
        }

        if (other.gameObject.CompareTag("Powerup"))
        {
            Destroy(other.gameObject);
            stageManager.UpdatePowerup(other.gameObject.name);
        }
    }
}