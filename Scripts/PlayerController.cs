using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Default - General
    public float speed = 10f;          
    public float jumpForce = 1000f;      
    public bool isGrounded = true;

    // Ice
    public bool isOnIce = false;
    public float iceAcceleration = 0.1f;
    public float maxIceSpeed = 20f;
    private Vector3 iceVelocity = Vector3.zero;
    public float iceDeceleration = 0.001f;

    // Physics
    private Rigidbody rb;

    // Animation
    private Animator animator;

    // Audio
    public AudioSource audioSource;
    public AudioClip jumpSound;
    public AudioClip groundImpactSound;
    public AudioClip movementSound;
    private bool isMoving = false;

    // Particle Effects
    public ParticleSystem iceTrail;
    public ParticleSystem dirtTrail;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();  
    }

    void Update()
    {
        float moveHorizontal = 0f;
        float moveVertical = 0f;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            moveVertical = 1f;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            moveVertical = -1f;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveHorizontal = -1f;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            moveHorizontal = 1f;
        }

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical).normalized;

        if (isOnIce)
        {
            // Accelerates, speed can not be more than maxIceSpeed
            if (movement != Vector3.zero)
            {
                iceVelocity += movement * iceAcceleration;
                iceVelocity = Vector3.ClampMagnitude(iceVelocity, maxIceSpeed);
            }
            // Decelerates if no movement input
            else
            {
                iceVelocity = Vector3.Lerp(iceVelocity, Vector3.zero, iceDeceleration);
            }
            rb.velocity = new Vector3(iceVelocity.x, rb.velocity.y, iceVelocity.z);
        }
        else
        {
            rb.velocity = new Vector3(movement.x * speed, rb.velocity.y, movement.z * speed);
            iceVelocity = Vector3.zero;
        }

        // Walking - Running
        if (movement != Vector3.zero)
        {
            // Make the Player look always at the direction of its movement
            transform.rotation = Quaternion.LookRotation(movement);
            animator.SetFloat("Speed_f", 1f);  
            
            if (!isMoving)
            {
                audioSource.PlayOneShot(movementSound);
                isMoving = true;
            }

            if (isOnIce)
            {
                if (!iceTrail.isPlaying)
                {
                    iceTrail.Play();
                }
            }
            else
            {
                if (!dirtTrail.isPlaying)
                {
                    dirtTrail.Play();
                }
            }
        }
        else
        // Idle
        {
            animator.SetFloat("Speed_f", 0f);  
            isMoving = false;

            iceTrail.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            dirtTrail.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
            audioSource.PlayOneShot(jumpSound);
            animator.SetBool("Jump_b", true);
            Invoke("ResetJumpTrigger", 0.1f); 
        }
    }


    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("DefaultGround") || collision.gameObject.CompareTag("IceGround"))
        {
            isGrounded = true;
            isOnIce = collision.gameObject.CompareTag("IceGround");
            audioSource.PlayOneShot(groundImpactSound);
        }
    }

    void ResetJumpTrigger()
    {
        animator.SetBool("Jump_b", false);
    }
}