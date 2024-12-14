using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    [SerializeField] Vector3 boxScale;
    [SerializeField] Vector3 offset;
    [SerializeField] float maxDistance;
    [SerializeField] bool isHit;
    [SerializeField] LayerMask groundLayer;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position + offset - transform.up * maxDistance, boxScale);
        isHit = Physics.BoxCast(transform.position + offset, boxScale / 2.0f, -transform.up, out RaycastHit hit, transform.rotation, maxDistance, groundLayer);
        if (isHit)
        {
            Gizmos.DrawRay(transform.position + offset, -transform.up * hit.distance);
            Gizmos.DrawWireCube(transform.position + offset - transform.up * hit.distance, boxScale);
        }
        else
        {
            Gizmos.DrawRay(transform.position + offset, -transform.up * maxDistance);
        }
    }

    public bool IsGrounded()
    {
        return Physics.BoxCast(transform.position + offset, boxScale / 2.0f, -transform.up, out RaycastHit hit, transform.rotation, maxDistance, groundLayer);
    }
}
