using UnityEngine;

public class WireController : MonoBehaviour
{
    public LineRenderer lineRenderer;   
    public GameObject sparkEffectPrefab; 
    public ElectricWhisk electricWhisk;   

    private CapsuleCollider wireCollider; 

    void Start()
    {
        CreateColliderAlongLine();
    }

    void CreateColliderAlongLine()
    {
        Vector3 start = lineRenderer.GetPosition(0);
        Vector3 end = lineRenderer.GetPosition(lineRenderer.positionCount - 1);

        wireCollider = gameObject.AddComponent<CapsuleCollider>();
        wireCollider.isTrigger = true;

        Vector3 midPoint = (start + end) / 2; 
        float length = Vector3.Distance(start, end);
        transform.position = midPoint;
        transform.LookAt(end);
        wireCollider.center = Vector3.zero;
        wireCollider.height = length;
        wireCollider.radius = 0.1f; 
        wireCollider.direction = 2; 
    }

    public void HandleWireAttack(Vector3 attackPosition)
    {
        Instantiate(sparkEffectPrefab, attackPosition, Quaternion.identity);
        if (electricWhisk != null)
        {
            electricWhisk.enabled = false; 
            Debug.Log("Electric Whisk turned off!");
        }
    }
}
