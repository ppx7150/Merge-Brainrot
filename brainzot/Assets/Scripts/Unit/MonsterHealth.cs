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
        stats.level+= count;
        stats.maxHP *= Mathf.Pow(2,count);
        stats.attackDamage *= Mathf.Pow(2, count);
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