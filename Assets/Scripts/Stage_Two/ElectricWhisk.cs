using UnityEngine;

public class ElectricWhisk : MonoBehaviour
{
    public bool isActive = true;
    public Transform whiskCenter;   
    public Transform player;         
    public float vibrationStrength = 0.1f;  
    public float vibrationSpeed = 5f;     
    public float pushRadius = 23f;         
    public float pushForce = 150f;      

    private Vector3 initialPosition;
    private AudioSource audioSource;

    void Start()
    {
        initialPosition = transform.position;
        audioSource = GetComponent<AudioSource>();
    }

    private void OnDisable()
    {
        audioSource.Stop();
    }

    void Update()
    {
        if (!isActive) return;
        ApplyVibration();
        ApplyPushBasedOnDistance();
        if (audioSource != null)
        {
            audioSource.volume = AudioManager.Instance.sfxVolume;
        }        
    }

    void ApplyVibration()
    {
        float offsetX = Mathf.PerlinNoise(Time.time * vibrationSpeed, 0f) * 2f - 1f;
        float offsetZ = Mathf.PerlinNoise(0f, Time.time * vibrationSpeed) * 2f - 1f;

        transform.position = initialPosition + new Vector3(offsetX, 0, offsetZ) * vibrationStrength;
    }

    void ApplyPushBasedOnDistance()
    {
        float distance = Vector3.Distance(whiskCenter.position, player.position);

        Vector3 direction = (player.position - whiskCenter.position).normalized;
        if (distance < pushRadius)
        {
            player.GetComponent<Rigidbody>().AddForce(direction * pushForce, ForceMode.Impulse);
        }
    }
}
