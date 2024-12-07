using UnityEngine;

public class SubStageTrigger : MonoBehaviour
{
    public int subStageIndex; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Stage2Manager에 Sub-Stage 방문 기록 요청
            Stage2Manager stage2Manager = FindObjectOfType<Stage2Manager>();
            if (stage2Manager != null)
            {
                stage2Manager.MarkSubStageVisited(subStageIndex);
            }
        }
    }
}
