using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenByAttack : MonoBehaviour, IDamageable
{
    public GameObject dustPrefab;
    public void TakeDamage(int amount)
    {
        AudioManager.Instance.PlayAttackSound();
        StartCoroutine(PlayDustEffectAndDestroy());
    }

    IEnumerator PlayDustEffectAndDestroy()
    {
        yield return new WaitForSeconds(0.5f);
        GameObject dust = Instantiate(dustPrefab, transform.position + new Vector3(0.0f, 0.5f, 0.0f), Quaternion.identity);
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }

}