using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    public bool isMelee;
    public Image img;
    public int cost;
    public int level;
    public void BuyUnit()
    {
        if (!Char.Instance.SubGems(cost)) return;
        if (isMelee)
        {
            UnitSpawner.Instance.SpawnMeleeUnit(level);
        }
        else
        {
            UnitSpawner.Instance.SpawnRangeUnit(level);
        }
    }
}
