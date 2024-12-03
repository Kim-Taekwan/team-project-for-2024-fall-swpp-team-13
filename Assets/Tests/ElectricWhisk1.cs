using UnityEngine;

public class ElectricWhisk1 : MonoBehaviour
{
    public bool isActive = true;
    public Transform whiskCenter;   
    public Transform player;         
    public float vibrationStrength = 0.1f;  
    public float vibrationSpeed = 5f;     
    public float pushRadius = 23f;         
    public float pushForce = 150f;      

    private Vector3 initialPosition;       

    void Start()
    {
        initialPosition = transform.position;
    }

    public virtual void Update()
    {
        if (!isActive) return;
        ApplyVibration();
        ApplyPushBasedOnDistance();
    }

    public virtual void ApplyVibration()
    {
        float offsetX = Mathf.PerlinNoise(Time.time * vibrationSpeed, 0f) * 2f - 1f;
        float offsetZ = Mathf.PerlinNoise(0f, Time.time * vibrationSpeed) * 2f - 1f;

        transform.position = initialPosition + new Vector3(offsetX, 0, offsetZ) * vibrationStrength;
    }

    public virtual void ApplyPushBasedOnDistance()
    {
        float distance = Vector3.Distance(whiskCenter.position, player.position);

        Vector3 direction = (player.position - whiskCenter.position).normalized;
        if (distance < pushRadius)
        {
            player.GetComponent<Rigidbody>().AddForce(direction * pushForce, ForceMode.Impulse);
        }
    }
}
