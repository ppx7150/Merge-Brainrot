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
    private Rigidbody2D rb;
    private Transform currentTarget;
    public GameObject projectile;

    public float overlapCheckRadius = 0.3f;
    public float ySpacing = 0.25f;
    public LayerMask unitLayer;
    public Transform visualRoot; // kéo child Visual vào đây

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
        HandleVerticalOffset();
    }
    void HandleMelee()
    {
        if (currentTarget == null) return;

        Vector2 myPos = transform.position;
        Vector2 targetPos = currentTarget.position;

        float attackRange = monsterHealth.stats.attackRange;
        float moveSpeed = monsterHealth.stats.moveSpeed;

        // Xác định hướng: enemy bên phải thì mình đứng bên trái và ngược lại
        float sideDir = targetPos.x > myPos.x ? -1f : 1f;

        // Vị trí chuẩn để đứng đánh
        Vector2 desiredPos = new Vector2(
            targetPos.x + sideDir * attackRange,
            targetPos.y
        );

        desiredPos = GridManager.Instance.ClampToGrid(desiredPos);

        float distanceX = Mathf.Abs(myPos.x - targetPos.x);
        bool sameLane = Mathf.Abs(myPos.y - targetPos.y) <= laneTolerance;

        // ====== CASE 1: KHÔNG CÙNG LANE -> CHẠY CHÉO TỚI ======
        if (!sameLane)
        {
            MoveTo(desiredPos, moveSpeed);
            return;
        }

        // ====== CASE 2: QUÁ XA -> TIẾN LẠI ======
        if (distanceX > attackRange + xTolerance)
        {
            MoveTo(desiredPos, moveSpeed);
            return;
        }

        // ====== CASE 3: QUÁ GẦN -> LÙI RA ======
        if (distanceX < attackRange - xTolerance)
        {
            Vector2 backDir = (myPos - targetPos).normalized;
            rb.linearVelocity = backDir * moveSpeed;
            return;
        }

        // ====== CASE 4: ĐÚNG KHOẢNG CÁCH -> ĐÁNH ======
        rb.linearVelocity = Vector2.zero;

        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            AttackMelee();
            attackTimer = monsterHealth.stats.attackSpeed;
        }
    }

    void MoveTo(Vector2 targetPos, float speed)
    {
        Vector2 dir = (targetPos - (Vector2)transform.position).normalized;
        rb.linearVelocity = dir * speed;
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
    void HandleVerticalOffset()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, overlapCheckRadius, unitLayer);

        float targetOffset = 0f;

        if (hits.Length > 1)
        {
            System.Array.Sort(hits, (a, b) => a.GetInstanceID().CompareTo(b.GetInstanceID()));

            int index = 0;
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].gameObject == gameObject)
                {
                    index = i;
                    break;
                }
            }

            float center = (hits.Length - 1) / 2f;
            targetOffset = (index - center) * ySpacing;
        }

        // Lệch Y CHỈ ở visual, không đụng Rigidbody
        Vector3 localPos = visualRoot.localPosition;
        localPos.y = Mathf.Lerp(localPos.y, targetOffset, Time.deltaTime * 10f);
        visualRoot.localPosition = localPos;
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
        if (currentTarget == null || projectile != null) return;
        GameObject proj = Instantiate(projectilePrefab, attackPoint.position, Quaternion.identity);
        projectile = proj;
        proj.GetComponent<Projectile>().enemy = currentTarget.gameObject;
        Vector2 dir = (currentTarget.position - attackPoint.position).normalized;
        proj.GetComponent<Rigidbody2D>().AddForce(dir * projectileForce, ForceMode2D.Impulse);
    }
}