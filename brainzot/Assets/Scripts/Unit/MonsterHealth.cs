using UnityEngine;

public class MonsterHealth : MonoBehaviour
{
    public int gridX;
    public int gridY;
    [Header("Visual")]
    public SpriteRenderer spriteRenderer;
    public Sprite[] visuals;
    public MonsterStats stats;
    public HPBar hpBar;
    public GameObject damageTextPrefab;
    public Transform textSpawnPoint;
    public float damageInSecond=0;
    public float timeShowDameTxt = 1f;
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
    public void TakeDamage(float dmg)
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
    void ShowDamage(float damage)
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

    public void LevelUp(int count)
    {
        stats.level += count;
        if (stats.type == MonsterType.Melee)
        {
            if (stats.level == 1)
            {
                stats.attackDamage = 5;
                stats.maxHP = 18;
            }
            if (stats.level == 2)
            {
                stats.attackDamage = 12;
                stats.maxHP = 43;
            }
            if (stats.level == 3)
            {
                stats.attackDamage = 27;
                stats.maxHP = 105;
            }
            if (stats.level == 4)
            {
                stats.attackDamage = 64;
                stats.maxHP = 225;
            }
            if (stats.level == 5)
            {
                stats.attackDamage = 152;
                stats.maxHP = 605;
            }
            if (stats.level == 6)
            {
                stats.attackDamage = 315;
                stats.maxHP = 1320;
            }
            if (stats.level == 7)
            {
                stats.attackDamage = 645;
                stats.maxHP = 2765;
            }
        }
        else
        {
            if (stats.level == 1)
            {
                stats.attackDamage = 3;
                stats.maxHP = 45;
            }
            if (stats.level == 2)
            {
                stats.attackDamage = 7;
                stats.maxHP = 115;
            }
            if (stats.level == 3)
            {
                stats.attackDamage = 15;
                stats.maxHP = 270;
            }
            if (stats.level == 4)
            {
                stats.attackDamage = 33;
                stats.maxHP = 625;
            }
            if (stats.level == 5)
            {
                stats.attackDamage = 70;
                stats.maxHP = 1320;
            }
            if (stats.level == 6)
            {
                stats.attackDamage = 150;
                stats.maxHP = 2675;
            }
            if (stats.level == 7)
            {
                stats.attackDamage = 315;
                stats.maxHP = 5550;
            }
        }

        UpdateVisual();
    }

    public void UpdateVisual()
    {
        spriteRenderer.sprite = visuals[stats.level - 1];
    }
    public void SetGridPos(int x, int y)
    {
        gridX = x;
        gridY = y;
    }
    public void ResetStatus()
    {
        stats.currentHP = stats.maxHP;
        hpBar.SetHP(1f);
        GridManager.Instance.Place(this, gridX, gridY);
    }
}