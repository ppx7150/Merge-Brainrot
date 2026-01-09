using UnityEngine;

public class MonsterAI : MonoBehaviour
{
    public float stopThreshold = 0.05f;
    public MonsterHealth monsterHealth;
    public LayerMask enemyLayer;

    public Transform attackPoint;
    public GameObject projectilePrefab;
    public float projectileForce = 10f;

    private float attackTimer;
    private Rigidbody2D rb;
    private Transform currentTarget;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (currentTarget == null)
        {
            FindNearestTarget();
            return;
        }
        if (currentTarget == null)
            return;
        attackTimer -= Time.deltaTime;
        if (monsterHealth.stats.type == MonsterType.Melee)
        {
            HandleMelee();
        }
        else
        {
            HandleRanged();
        }
    }
    void HandleMelee()
    {
        if (currentTarget == null) return;
        Vector2 myPos = transform.position;
        Vector2 targetPos = currentTarget.position;
        float laneTolerance = 0.2f;   // sai lệch Y cho phép (cùng lane)
        float xTolerance = 0.5f;      // sai lệch X cho phép (đứng ngang)
        // Xác định hướng (giả sử team bạn đứng bên trái, enemy bên phải)
        float direction = targetPos.x > myPos.x ? -1f : 1f;
        // Vị trí chuẩn cần đứng để đánh
        Vector2 desiredPos = new Vector2(
            targetPos.x + direction * 1f,
            targetPos.y
        );
        // Clamp để đảm bảo không ra ngoài grid
        desiredPos = GridManager.Instance.ClampToGrid(desiredPos);
        bool sameLane = Mathf.Abs(myPos.y - targetPos.y) <= laneTolerance;
        bool correctX = Mathf.Abs(myPos.x - desiredPos.x) <= xTolerance;
        // Nếu chưa đúng lane hoặc chưa đúng X -> chạy chéo tới vị trí chuẩn
        if (!sameLane || !correctX)
        {
            Vector2 moveDir = (desiredPos - myPos).normalized;
            rb.linearVelocity = moveDir * monsterHealth.stats.moveSpeed;
            return;
        }
        // Đã đúng lane + đúng X -> đứng yên và đánh
        rb.linearVelocity = Vector2.zero;
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            AttackMelee();
            attackTimer = monsterHealth.stats.attackSpeed;
        }
    }

    void HandleRanged()
    {
        rb.linearVelocity = Vector2.zero;
        float dist = Vector2.Distance(transform.position, currentTarget.position);
        if (dist <= monsterHealth.stats.attackRange && attackTimer <= 0)
        {
            Shoot();
            attackTimer = monsterHealth.stats.attackSpeed;
        }
    }
    void FindNearestTarget()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 10f, enemyLayer);
        float minDist = Mathf.Infinity;
        Transform nearest = null;
        foreach (var col in cols)
        {
            float d = Vector2.Distance(transform.position, col.transform.position);
            if (d < minDist)
            {
                minDist = d;
                nearest = col.transform;
            }
        }
        currentTarget = nearest;
    }
    void AttackMelee()
    {
        if (currentTarget == null) return;
        MonsterHealth hp = currentTarget.GetComponent<MonsterHealth>();
        if (hp != null)
        {
            hp.TakeDamage(monsterHealth.stats.attackDamage);
            // knockback meme
            Rigidbody2D enemyRb = currentTarget.GetComponent<Rigidbody2D>();
            if (enemyRb != null)
                enemyRb.AddForce(new Vector2(3f, 2f), ForceMode2D.Impulse);
        }
    }

    void Shoot()
    {
        if (currentTarget == null) return;
        GameObject proj = Instantiate(projectilePrefab, attackPoint.position, Quaternion.identity);
        Vector2 dir = (currentTarget.position - attackPoint.position).normalized;
        proj.GetComponent<Rigidbody2D>().AddForce(dir * projectileForce, ForceMode2D.Impulse);
    }
}