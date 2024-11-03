using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Rigid Body
    // Mass: 200
    // Drag: 0.5, Angular Drag: 0.05
    // Freeze Rotation: X, Y, Z
    private Rigidbody rb;    
    private StageManager stageManager;

    // Default Ground - General
    [SerializeField] float currentSpeed; // for debugging speed
    public float speed = 1.5f;
    public float jumpForce = 1200.0f;
    public bool isGrounded = true;
    public float groundCheckDistance = 1.1f;

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
        //animator = GetComponent<Animator>();
    }

    void Update()
    {
        GroundCheck(); 
        HandleMovementInput();
        HandleJumpInput();
        UpdateEffects();
    }

    private IEnumerator DelayGroundedReset()
    {
        yield return new WaitForSeconds(0.1f);
        isGrounded = false;
        //animator.SetBool("Grounded", false);
        rb.drag = 0f;
        rb.AddForce(Vector3.down * 0.1f, ForceMode.Acceleration);
    }

    private void GroundCheck()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + new Vector3(0, 1, 0), Vector3.down, out hit, groundCheckDistance))
        {
            if (Vector3.Angle(hit.normal, Vector3.up) < 45f && hit.distance <= groundCheckDistance)
            {
                isGrounded = true;
                //isOnIce = hit.collider.CompareTag("IceGround");
                rb.drag = 0.5f;
                //animator.SetBool("Grounded", true);
                if (!isMoving)
                {
                    //audioSource.PlayOneShot(groundImpactSound);
                }
            }
            else if (isGrounded && rb.velocity.y >= -0.1f) 
            {
                isGrounded = true;
            }
            else
            {
                StartCoroutine(DelayGroundedReset());
            }
        }
        else
        {
            StartCoroutine(DelayGroundedReset());
        }
    }

    private void HandleMovementInput()
    {
        float moveHorizontal = 0f;
        float moveVertical = 0f;

        if (Input.GetKey(KeyCode.UpArrow)) moveVertical = 1f;
        else if (Input.GetKey(KeyCode.DownArrow)) moveVertical = -1f;

        if (Input.GetKey(KeyCode.LeftArrow)) moveHorizontal = -1f;
        else if (Input.GetKey(KeyCode.RightArrow)) moveHorizontal = 1f;

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical).normalized;
        ApplyMovement(movement);
        UpdateAnimationAndSound(movement);
    }

    private void ApplyMovement(Vector3 movement)
    {
        if (isGrounded /*&& !isOnIce*/) // On Default Ground
        {
            Vector3 force = movement * speed * 2.5f * rb.mass;
            rb.AddForce(force, ForceMode.Force);
            //iceVelocity = Vector3.zero;
        }
        else if (/*isOnIce &&*/ isGrounded) // On Ice
        {
            if (movement != Vector3.zero)
            {
                //iceVelocity += movement * iceAcceleration;
                //iceVelocity = Vector3.ClampMagnitude(iceVelocity, maxIceSpeed);
                //rb.AddForce(iceVelocity * rb.mass, ForceMode.Force);
            }
            else
            {
                //iceVelocity = Vector3.Lerp(iceVelocity, Vector3.zero, iceDeceleration);
                //rb.AddForce(iceVelocity * rb.mass, ForceMode.Force);
            }
        }
        // On Air
        else 
        {
            Vector3 force = movement * speed * rb.mass * 0.4f;
            rb.AddForce(force, ForceMode.Force);
            rb.AddForce(Physics.gravity, ForceMode.Acceleration);
            //iceVelocity = Vector3.zero;
        }
    }

    private void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
            //audioSource.PlayOneShot(jumpSound);
            //animator.SetBool("Jump_b", true);
            Invoke("ResetJumpTrigger", 0.1f);
        }
    }

    private void UpdateAnimationAndSound(Vector3 movement)
    {
        currentSpeed = new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;
        //animator.SetFloat("Speed_f", currentSpeed);
        //animator.SetBool("Moving_b", movement != Vector3.zero);

        if (movement != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            /*if (!audioSource.isPlaying)
            {
                audioSource.clip = movementSound;
                audioSource.loop = true;
                audioSource.Play();
            }*/
        }
        else
        {
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

    void ResetJumpTrigger()
    {
        //animator.SetBool("Jump_b", false);
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
            stageManager.GameClear();
        }
    }
}