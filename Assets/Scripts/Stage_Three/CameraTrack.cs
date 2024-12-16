using UnityEngine;
using Cinemachine;

public class CameraTrack : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public Transform Player;
    public float dollyMoveSpeed = 0.1f; 
    public float followMoveSpeed = 5f; 
    public Vector3 followOffset = new Vector3(0, 2.5f, -5.5f); 
    public float rotationSmoothing = 5f; 
    private CinemachineTrackedDolly dollyComponent;
    private bool dollyCompleted = false;

    void Start()
    {
        dollyComponent = virtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
        dollyComponent.m_PathPosition = 0f;
        virtualCamera.transform.rotation = Quaternion.Euler(80, 0, 0); 
    }

    void Update()
    {
        if (!dollyCompleted)
        {
            dollyComponent.m_PathPosition += dollyMoveSpeed * Time.deltaTime;

            if (dollyComponent.m_PathPosition >= 1f)
            {
                dollyCompleted = true;
                dollyComponent.enabled = false;
            }
        }
        else
        {
            Vector3 targetPosition = Player.position + followOffset;
            virtualCamera.transform.position = Vector3.Lerp(virtualCamera.transform.position, targetPosition, Time.deltaTime * followMoveSpeed);

            Vector3 lookDirection = Player.position - virtualCamera.transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            virtualCamera.transform.rotation = Quaternion.Slerp(virtualCamera.transform.rotation, targetRotation, Time.deltaTime * rotationSmoothing);
        }
    }
}