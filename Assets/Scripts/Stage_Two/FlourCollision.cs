using UnityEngine;
using UnityEngine.UI;

public class FlourCollision : MonoBehaviour
{
    public GameObject flourMarkPrefab;
    private FlourUIManager uiManager;   
    public float markDuration = 5f;   

    private void Start()
    {
        uiManager = FindObjectOfType<FlourUIManager>();
        if (uiManager == null)
        {
            Debug.LogError("FlourUIManager를 찾을 수 없습니다!");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            ContactPoint contact = collision.contacts[0];
            Vector3 spawnPosition = contact.point;
            GameObject flourMark = Instantiate(flourMarkPrefab, spawnPosition, Quaternion.identity);
            Destroy(flourMark, markDuration);
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            if (uiManager != null)
            {
                AudioManager.Instance.PlayFlourOnScreenSound();
                uiManager.ShowFlourUI();
            }
            Destroy(gameObject); 
        }
    }
}
