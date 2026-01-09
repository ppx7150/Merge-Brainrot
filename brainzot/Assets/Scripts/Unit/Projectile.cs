using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage;
    public float lifeTime = 3f;
    public GameObject enemy;
    public void Awake()
    {
        Destroy(gameObject, lifeTime);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        MonsterHealth hp = other.GetComponent<MonsterHealth>();
        if (hp != null && other.gameObject == enemy)
        {
            hp.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}