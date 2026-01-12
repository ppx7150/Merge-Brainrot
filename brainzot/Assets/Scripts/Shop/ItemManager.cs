using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    public bool isMelee;
    public int cost;
    public int level;
    public int dame;
    public int hp;
    public TMP_Text txtCost;
    public TMP_Text txtDame;
    public TMP_Text txtHp;
    private void Start()
    {
        SetStats(level);
        if (txtCost != null) txtCost.SetText(cost.ToString());
        if (txtDame != null) txtDame.SetText(dame.ToString());
        if (txtHp != null) txtHp.SetText(hp.ToString());
    }
    public void SetStats(int level)
    {
        if (level < 1 || level > 7) return;
        int index = level - 1;
        if (isMelee)
        {
            int[] damages = { 5, 12, 27, 64, 152, 315, 645 };
            int[] hps = { 18, 43, 105, 225, 605, 1320, 2765 };
            dame = damages[index];
            hp = hps[index];
        }
        else
        {
            int[] damages = { 3, 7, 15, 33, 70, 150, 315 };
            int[] hps = { 45, 115, 270, 625, 1320, 2675, 5550 };
            dame = damages[index];
            hp = hps[index];
        }
    }
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
