using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenByAttack : MonoBehaviour, IDamageable
{
    public void TakeDamage(int amount)
    {
        AudioManager.Instance.PlayAttackSound();
        Destroy(gameObject);
    }
}