using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakFence : MonoBehaviour, IEnemy
{
    public void GiveDamage()
    {
        return;
    }

    public void TakeDamage(int amount)
    {
        Destroy(gameObject);
    }
}
