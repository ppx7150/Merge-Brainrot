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
        SetStats(stats.level);
        UpdateVisual();
    }
    public void SetStats(int level)
    {
        if (level < 1 || level > 7) return;
        int index = level - 1;
        if (stats.type == MonsterType.Melee)
        {
            int[] damages = { 5, 12, 27, 64, 152, 315, 645 };
            int[] hps = { 18, 43, 105, 225, 605, 1320, 2765 };
            stats.attackDamage = damages[index];
            stats.maxHP = hps[index];
        }
        else
        {
            int[] damages = { 3, 7, 15, 33, 70, 150, 315 };
            int[] hps = { 45, 115, 270, 625, 1320, 2675, 5550 };
            stats.attackDamage = damages[index];
            stats.maxHP = hps[index];
        }
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