using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    public bool isMelee; //Kiểm tra xem có phải Unit cận chiến không
    public int cost; //Giá tiền của Unit
    public int level; //level của Unit
    public int dame; //atk của Unit
    public int hp; //hp của Unit
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
        if (level < 1 || level > 8) return;
        int index = level - 1;
        if (isMelee)
        {
            dame = Char.Instance.damagesMelee[index];
            hp = Char.Instance.hpsMelee[index];
        }
        else
        {
            dame = Char.Instance.damagesRange[index];
            hp = Char.Instance.hpsRange[index];
        }
    }
    public void BuyUnit() //Mua unit
    {
        if (!Char.Instance.SubGems(cost)) return;
        PanelManager.Instance.hideSummonPanel();
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
