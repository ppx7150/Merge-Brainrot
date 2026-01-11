using UnityEngine;

public class MonsterAI : MonoBehaviour
{
    public float laneTolerance = 0.2f;   // sai lệch Y cho phép (cùng lane)
    public float xTolerance = 0.5f;      // sai lệch X cho phép (đứng ngang)
    public MonsterHealth monsterHealth;
    public LayerMask enemyLayer;

    public Transform attackPoint;
    public GameObject projectilePrefab;
    public float projectileForce = 10f;

    private float attackTimer;
    private Transform currentTarget;
    public GameObject projectile;

    public Transform visualRoot;

    void Update()
    {
        if (currentTarget == null || !currentTarget.gameObject.activeSelf)
        {
            FindNearestTarget();
            return;
        }
        if (currentTarget == null || !currentTarget.gameObject.activeSelf)
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
        if (currentTarget == null || !currentTarget.gameObject.activeSelf) return;

        Vector2 myPos = transform.position;
        Vector2 targetPos = currentTarget.position;

        float attackRange = monsterHealth.stats.attackRange;
        float moveSpeed = monsterHealth.stats.moveSpeed;

        float sideDir = targetPos.x > myPos.x ? -1f : 1f;

        Vector2 desiredPos = new Vector2(
            targetPos.x + sideDir * attackRange,
            targetPos.y
        );

        desiredPos = GridManager.Instance.ClampToGrid(desiredPos);

        float distanceX = Mathf.Abs(myPos.x - targetPos.x);
        bool sameLane = Mathf.Abs(myPos.y - targetPos.y) <= laneTolerance;

        // ===== CASE 1: KHÔNG CÙNG LANE =====
        if (!sameLane)
        {
            MoveTo(desiredPos, moveSpeed);
            return;
        }
        // ===== CASE 2: QUÁ XA =====
        if (distanceX > attackRange + xTolerance)
        {
            MoveTo(desiredPos, moveSpeed);
            return;
        }
        // ===== CASE 3: QUÁ GẦN =====
        if (distanceX < attackRange - xTolerance)
        {
            Vector2 backPos = myPos + (myPos - targetPos).normalized * moveSpeed * Time.deltaTime;
            transform.position = backPos;
            return;
        }
        // ===== CASE 4: ĐÚNG KHOẢNG CÁCH -> ĐÁNH =====
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            AttackMelee();
            attackTimer = monsterHealth.stats.attackSpeed;
        }
    }

    void MoveTo(Vector2 targetPos, float speed)
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
    }

    void HandleRanged()
    {
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
        if (currentTarget == null || !currentTarget.gameObject.activeSelf) return;
        MonsterHealth hp = currentTarget.GetComponent<MonsterHealth>();
        if (hp != null)
        {
            hp.TakeDamage(monsterHealth.stats.attackDamage);
        }
    }

    void Shoot()
    {
        if (currentTarget == null || !currentTarget.gameObject.activeSelf || projectile != null) return;
        GameObject proj = Instantiate(projectilePrefab, attackPoint.position, Quaternion.identity);
        projectile = proj;
        proj.GetComponent<Projectile>().enemy = currentTarget.gameObject;
        Vector2 dir = (currentTarget.position - attackPoint.position).normalized;
        proj.GetComponent<Rigidbody2D>().AddForce(dir * projectileForce, ForceMode2D.Impulse);
    }
}