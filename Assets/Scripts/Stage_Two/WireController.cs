using Unity.Burst.CompilerServices;
using UnityEngine;

public class WireController : MonoBehaviour, IDamageable
{
    public LineRenderer lineRenderer;   
    public GameObject sparkEffectPrefab; 
    public ElectricWhisk electricWhisk;
    public FlourShooter flourShooter;

    private CapsuleCollider wireCollider;
    private GameObject player;

    void Start()
    {
        CreateColliderAlongLine();
        player = GameObject.Find("Player");
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

    public void TakeDamage(int amount)
    {        
        Vector3 playerPosition = player.transform.position;
        Vector3 forward = player.transform.forward;
        Vector3 attackPosition;
        if (Physics.Raycast(playerPosition, forward, out RaycastHit hitInfo, player.GetComponent<PlayerController>().attackRange))
        {
            attackPosition = hitInfo.point;
        }
        else
        {
            attackPosition = playerPosition;
        }
        Instantiate(sparkEffectPrefab, attackPosition, Quaternion.identity);

        if (electricWhisk != null)
        {
            electricWhisk.enabled = false; 
            Debug.Log("Electric Whisk turned off!");
        }

        if (flourShooter != null)
        {
            flourShooter.enabled = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && player.GetComponent<PlayerController>().isInvincible)
        {
            TakeDamage(1);
        }
    }
}
