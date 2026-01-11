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
        damageInSecond += dmg;
        stats.currentHP -= dmg;
        stats.currentHP = Mathf.Clamp(stats.currentHP, 0, stats.maxHP);
        if (hpBar != null)
            hpBar.SetHP(stats.currentHP / stats.maxHP);
        if (stats.currentHP <= 0)
            Die();
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
        stats.level+= count;
        stats.maxHP *= Mathf.Pow(2,count);
        stats.attackDamage *= Mathf.Pow(2, count);
        UpdateVisual();
    }

    public void UpdateVisual()
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
    public void ResetStatus()
    {
        stats.currentHP = stats.maxHP;
        hpBar.SetHP(1f);
        GridManager.Instance.Place(this, gridX, gridY);
    }
}