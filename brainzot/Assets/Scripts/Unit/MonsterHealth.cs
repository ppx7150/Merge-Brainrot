using UnityEngine;

public class MonsterHealth : MonoBehaviour
{
    public int gridX; //Tọa độ x của Unit trong grid
    public int gridY; //Tọa độ y của Unit trong grid
    [Header("Visual")]
    public SpriteRenderer spriteRenderer;
    public Sprite[] visuals;
    public MonsterStats stats;
    public HPBar hpBar;
    public GameObject damageTextPrefab;
    public Transform textSpawnPoint;
    public float damageInSecond=0; //Giá dame nhận được trong 1 giây
    public float timeShowDameTxt = 1f; //Delay hiển thị dame
    void Awake()
    {
        stats.currentHP = stats.maxHP;
        hpBar.SetHP(1f);
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        UpdateVisual();
    }
    private void Update()
    {
        if(timeShowDameTxt > 0) timeShowDameTxt -= Time.deltaTime;
    }
    public void TakeDamage(float dmg) //Hàm tính lượng hp còn lại sau khi bị attack với sát thương dmg
    {
        if (stats.currentHP > 0) dmg = Mathf.Min(dmg, stats.currentHP);
        damageInSecond += dmg;
        stats.currentHP -= dmg;
        if (hpBar != null) hpBar.SetHP(stats.currentHP / stats.maxHP);
        if (stats.currentHP <= 0) Die();
        if (CompareTag("Enemy") && timeShowDameTxt <= 0)
        {
            ShowDamage(damageInSecond);
        }
    }
    void ShowDamage(float damage) //Hàm hiển thị dame
    {
        GameObject textObj = Instantiate(damageTextPrefab, textSpawnPoint.position, Quaternion.identity, textSpawnPoint);
        DamageText dmgText = textObj.GetComponent<DamageText>();
        dmgText.SetText(damage.ToString());
        timeShowDameTxt = 1f;
        damageInSecond = 0f;
    }
    void Die()
    {
        gameObject.SetActive(false);
    }

    public void LevelUp(int count) //Nâng cấp level của Unit
    {
        stats.level += count;
        SetStats(stats.level);
        UpdateVisual();
    }
    public void SetStats(int level)
    {
        if (level < 1 || level > 7) return;
        int index = level - 1;
        if (stats.type == MonsterType.Melee)
        {
            stats.attackDamage = Char.Instance.damagesMelee[index];
            stats.maxHP = Char.Instance.hpsMelee[index];
            stats.currentHP = stats.maxHP;
        }
        else
        {
            stats.attackDamage = Char.Instance.damagesRange[index];
            stats.maxHP = Char.Instance.hpsRange[index];
            stats.currentHP = stats.maxHP;
        }
    }

    public void UpdateVisual() //Cập nhật visual cho phù hợp với level Unit
    {
        spriteRenderer.sprite = visuals[stats.level - 1];
    }
    public void SetGridPos(int x, int y) //Lưu vị trí của Unit
    {
        gridX = x;
        gridY = y;
    }
    public void ResetStatus() //Trả về trạng thái chuẩn bị
    {
        stats.currentHP = stats.maxHP;
        hpBar.SetHP(1f);
        GridManager.Instance.Place(this, gridX, gridY);
    }
}