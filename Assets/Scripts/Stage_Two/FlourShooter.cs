using UnityEngine;

public class FlourShooter : MonoBehaviour
{
    public GameObject flourPrefab;     
    public Transform spawnPoint;    
    public float maxDistance = 5f;    
    public float flightDuration = 1f; 
    public float arcHeight = 2f;      
    public float minDistance = 3f;
    public float repeatRate = 1f;

    void Start()
    {
        InvokeRepeating(nameof(LaunchFlour), repeatRate, repeatRate);
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(LaunchFlour));
    }

    void LaunchFlour()
    {
        GameObject flour = Instantiate(flourPrefab, spawnPoint.position, Quaternion.identity);

        Vector3 randomTarget = GetRandomTargetPosition();

        StartCoroutine(MoveFlourInParabola(flour, randomTarget));
    }

    Vector3 GetRandomTargetPosition()
    {
        Vector3 randomTarget;

        do
        {
            randomTarget = new Vector3(
                Random.Range(-maxDistance, maxDistance), 
                0f,                                      
                Random.Range(-maxDistance, maxDistance)  
            );
        } while (Vector3.Distance(spawnPoint.position, randomTarget) < minDistance);

        return spawnPoint.position + randomTarget;
    }

    System.Collections.IEnumerator MoveFlourInParabola(GameObject flour, Vector3 target)
    {
        Vector3 start = spawnPoint.position;
        float elapsedTime = 0f;

        while (elapsedTime < flightDuration)
        {
            float t = elapsedTime / flightDuration;

            Vector3 horizontalPosition = Vector3.Lerp(start, target, t);

            float height = Mathf.Sin(Mathf.PI * t) * arcHeight; 

            if (flour == null)
            {
                break;
            }

            flour.transform.position = new Vector3(
                horizontalPosition.x,
                start.y + height,
                horizontalPosition.z
            );

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if(flour != null)
            Destroy(flour, 0.5f);
    }
}
