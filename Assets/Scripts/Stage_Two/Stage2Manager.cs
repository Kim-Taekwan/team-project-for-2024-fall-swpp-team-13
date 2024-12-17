using UnityEngine;

public class Stage2Manager : MonoBehaviour
{
    public Transform[] respawnPoints; // 각 Sub-Stage의 Respawn 지점
    public int currentSubStage = 0;   // 현재 플레이어가 도달한 Sub-Stage 번호
    public float deathYThreshold = 15f; // 죽음 판정 Y좌표

    private PlayerController player;
    private StageManager stageManager;
    private bool[] visitedSubStages; // 각 Sub-Stage의 방문 여부

    void Start()
    {
        // 플레이어 찾기
        player = FindObjectOfType<PlayerController>();
        if (player == null)
        {
            Debug.LogError("PlayerController를 찾을 수 없습니다!");
        }
        stageManager = GameObject.Find("StageManager").GetComponent<StageManager>();

        // 방문 여부 배열 초기화
        visitedSubStages = new bool[respawnPoints.Length];
    }

    void Update()
    {
        // 플레이어가 죽음 판정 기준을 충족했을 경우 Respawn 처리
        if (player != null && player.transform.position.y < deathYThreshold)
        {
            player.TakeDamage(2);
            if (!stageManager.isGameOver)
            {
                AudioManager.Instance.PlayCliffSound();
                RespawnPlayer();
            }            
        }
    }

    public void MarkSubStageVisited(int subStageIndex)
    {
        if (subStageIndex >= 0 && subStageIndex < visitedSubStages.Length)
        {
            visitedSubStages[subStageIndex] = true;
            if (subStageIndex > currentSubStage)
            {
                currentSubStage = subStageIndex; // 가장 나중에 방문한 Sub-Stage로 업데이트
            }
            Debug.Log($"Sub-Stage {subStageIndex + 1} 방문 기록!");
        }
        else
        {
            Debug.LogError("잘못된 Sub-Stage 번호입니다!");
        }
    }

    private void RespawnPlayer()
    {
        // 가장 최근에 방문한 Sub-Stage의 Respawn 지점으로 이동
        Transform respawnPoint = respawnPoints[currentSubStage];
        if (respawnPoint != null)
        {
            Debug.Log($"플레이어 Respawn: Sub-Stage {currentSubStage + 1} - {respawnPoint.position}");

            // 플레이어 위치 초기화
            player.transform.position = respawnPoint.position;

            // 플레이어 속도 초기화
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
            }
        }
        else
        {
            Debug.LogError("Respawn 지점이 설정되지 않았습니다!");
        }
    }
}
