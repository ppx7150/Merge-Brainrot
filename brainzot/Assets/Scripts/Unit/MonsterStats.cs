public enum MonsterType
{
    Melee,
    Ranged
}

[System.Serializable]
public class MonsterStats
{
    public MonsterType type;
    public int level;

    public float maxHP;
    public float currentHP;

    public float attackDamage;
    public float attackSpeed;   // thời gian giữa các đòn
    public float attackRange;   // range đánh / bắn

    public float moveSpeed;     // chỉ dùng cho melee
}