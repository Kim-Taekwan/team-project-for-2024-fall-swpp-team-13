using UnityEngine;

public class CarrotProjectile : MonoBehaviour
{
    public int damage = 1;
    public float lifetime = 1f;
    //public GameObject hitEffectPrefab;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            IEnemy enemy = collision.gameObject.GetComponent<IEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Destroy(gameObject);
            }

            // if (hitEffectPrefab != null)
            // {
            //     Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            // }
        }
    }
}
