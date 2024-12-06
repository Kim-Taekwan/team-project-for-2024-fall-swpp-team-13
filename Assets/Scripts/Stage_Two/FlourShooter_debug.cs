using UnityEngine;

public class FlourShooter_debug : MonoBehaviour
{
    public GameObject flourPrefab;
    public Transform spawnPoint;
    public float flightDuration = 1f; // 비행 시간
    public float arcHeight = 2f;      // 포물선 최고점 높이
    public Vector3 fixedDirection = new Vector3(5f, 0f, 0f); // 일정한 방향 (X:5, Z:0)

    void Start()
    {
        // 기존 랜덤 발사
        InvokeRepeating(nameof(LaunchFlour), 1f, 1f);

        // 일정한 방향으로 발사
        InvokeRepeating(nameof(LaunchFlourInFixedDirection), 1f, 2f); // 2초마다 발사
    }

    void LaunchFlour()
    {
        GameObject flour = Instantiate(flourPrefab, spawnPoint.position, Quaternion.identity);
        Vector3 randomTarget = GetRandomTargetPosition();
        StartCoroutine(MoveFlourInParabola(flour, randomTarget));
    }

    void LaunchFlourInFixedDirection()
    {
        GameObject flour = Instantiate(flourPrefab, spawnPoint.position, Quaternion.identity);
        Vector3 fixedTarget = spawnPoint.position + fixedDirection; // 고정된 방향으로 타겟 설정
        StartCoroutine(MoveFlourInParabola(flour, fixedTarget));
    }

    Vector3 GetRandomTargetPosition()
    {
        Vector3 randomTarget;

        do
        {
            randomTarget = new Vector3(
                Random.Range(-5f, 5f), // X축 랜덤
                0f,                  // Y축 고정
                Random.Range(-5f, 5f) // Z축 랜덤
            );
        } while (Vector3.Distance(spawnPoint.position, randomTarget) < 3f);

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

            if (flour == null) break;

            flour.transform.position = new Vector3(
                horizontalPosition.x,
                start.y + height,
                horizontalPosition.z
            );

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (flour != null)
            Destroy(flour, 0.5f);
    }
}
