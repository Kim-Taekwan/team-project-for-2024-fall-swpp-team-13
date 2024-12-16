using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlourZone : MonoBehaviour
{
    [SerializeField] private float speedMultiplier = 0.5f;

    private HashSet<PlayerController> playersInZone = new HashSet<PlayerController>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                // 속도 감소 및 플레이어를 목록에 추가
                if (!playersInZone.Contains(player))
                {
                    player.speed *= speedMultiplier;
                    playersInZone.Add(player);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null && playersInZone.Contains(player))
            {
                // 속도 복구 및 플레이어를 목록에서 제거
                player.speed = player.isInvincible ? player.dashSpeed : player.originalSpeed;
                playersInZone.Remove(player);
            }
        }
    }

    private void OnDestroy()
    {
        // FlourZone이 파괴될 때 해당 Zone 내의 모든 플레이어의 속도를 복구
        foreach (var player in playersInZone)
        {
            if (player != null)
            {
                player.speed = player.isInvincible ? player.dashSpeed : player.originalSpeed;
            }
        }

        // 목록 초기화
        playersInZone.Clear();
    }
}
