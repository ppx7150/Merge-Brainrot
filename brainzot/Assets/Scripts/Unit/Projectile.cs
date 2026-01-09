using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage = 10f;
    public float lifeTime = 3f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        MonsterHealth hp = other.GetComponent<MonsterHealth>();
        if (hp != null)
        {
            hp.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}