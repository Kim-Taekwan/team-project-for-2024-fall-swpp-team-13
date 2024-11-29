using UnityEngine;

public class WhiskMotion : MonoBehaviour
{
    public Transform bowlCenter;    
    public float rotationRadius = 0.7f; 
    public float rotationSpeed = 200f;  
    public float heightOffset = 7.5f;  

    private Quaternion initialRotation; 
    private float angle = 0f;          

    void Start()
    {
        initialRotation = transform.rotation;

        transform.position = new Vector3(
            bowlCenter.position.x,
            bowlCenter.position.y + heightOffset,
            bowlCenter.position.z
        );

        transform.position += Vector3.forward * rotationRadius;
    }

    void Update()
    {
        MoveInCircle();
    }

    void MoveInCircle()
    {
        angle += rotationSpeed * Time.deltaTime;

        float x = bowlCenter.position.x + Mathf.Cos(angle * Mathf.Deg2Rad) * rotationRadius;
        float z = bowlCenter.position.z + Mathf.Sin(angle * Mathf.Deg2Rad) * rotationRadius;

        transform.position = new Vector3(
            x,
            bowlCenter.position.y + heightOffset,
            z
        );

        transform.rotation = initialRotation;
    }
}
