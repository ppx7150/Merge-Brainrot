using UnityEngine;

public class MonsterHealth : MonoBehaviour
{
    public int gridX;
    public int gridY;
    [Header("Visual")]
    public SpriteRenderer spriteRenderer;
    public UnitVisualData[] visuals;
    public MonsterStats stats;
    public HPBar hpBar;
    void Awake()
    {
        stats.currentHP = stats.maxHP;
        hpBar.SetHP(1f);
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        UpdateVisual();
    }
    public void TakeDamage(float dmg)
    {
        stats.currentHP -= dmg;
        stats.currentHP = Mathf.Clamp(stats.currentHP, 0, stats.maxHP);
        if (hpBar != null)
            hpBar.SetHP(stats.currentHP / stats.maxHP);
        if (stats.currentHP <= 0)
            Die();
    }

    void Die()
    {
        Destroy(gameObject);
    }

    public void LevelUp()
    {
        stats.level++;
        stats.maxHP *= 2;
        stats.attackDamage *= 2;
        UpdateVisual();
    }

    void UpdateVisual()
    {
        foreach (var v in visuals)
        {
            if (v.level == stats.level)
            {
                spriteRenderer.sprite = v.sprite;
                transform.localScale = v.scale;
                return;
            }
        }
    }
    public void SetGridPos(int x, int y)
    {
        gridX = x;
        gridY = y;
    }
}