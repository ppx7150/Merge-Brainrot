using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.Mathematics.Geometry;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    public long costMelee = 100;
    public long costRange = 100;
    public TMP_Text txtCostMelee;
    public TMP_Text txtCostRange;
    public BattleManager battleManager;
    public GameObject rangeUnitPrefab;
    public GameObject meleeUnitPrefab;
    public static UnitSpawner Instance;


    private void Awake()
    {
        Instance = this;
    }
    public void LoadCost(long cM, long cR) //Làm mới giá tiền mua Unit
    {
        costMelee = cM;
        costRange = cR;
        OnCost();
    }
    public void OnCost() //Hien giá tiền mua Unit
    {
        if (!txtCostMelee.gameObject.activeSelf) txtCostMelee.gameObject.SetActive(true);
        if (!txtCostRange.gameObject.activeSelf) txtCostRange.gameObject.SetActive(true);
        txtCostMelee.SetText(Char.FormatMoney(costMelee) + "$");
        txtCostRange.SetText(Char.FormatMoney(costRange) + "$");
    }
    public void UpgradeCost(bool isMelee) //Nâng giá tiền mua Unit
    {
        if (isMelee)
        {
            costMelee = (long)System.Math.Round(costMelee * 1.1);
        }
        else
        {
            costRange = (long)System.Math.Round(costRange * 1.1);
        }
        OnCost();
    }
    public void SpawnRangeUnit(int level) //Spawn Unit đánh xa
    {
        GridManager grid = GridManager.Instance;
        // Tìm ô trống đầu tiên (từ dưới lên)
        for (int y = 0; y <= 2; y++)
        {
            for (int x = 4; x >= 0; x--)
            {
                if (grid.IsEmpty(x, y))
                {
                    GameObject unitObj = Instantiate(rangeUnitPrefab);
                    battleManager.playerTeam.Add(unitObj);
                    MonsterHealth unit = unitObj.GetComponent<MonsterHealth>();
                    Char.Instance.dataMyTeam.Add(unit);
                    unit.LevelUp(level);
                    AudioManager.Instance.PlayUnitSound(level, unit.stats.type);
                    grid.Place(unit, x, y);
                    if(Char.Instance.level >= 2) UpgradeCost(false);
                    return;
                }
            }
        }
        Debug.Log("Grid full - cannot spawn unit");
    }

    public void SpawnMeleeUnit(int level) //Spawn unit cận chiến
    {
        GridManager grid = GridManager.Instance;
        for (int y = 2; y >= 0; y--)
        {
            for (int x = 0; x <= 4; x++)
            {
                if (grid.IsEmpty(x, y))
                {
                    GameObject unitObj = Instantiate(meleeUnitPrefab);
                    battleManager.playerTeam.Add(unitObj);
                    MonsterHealth unit = unitObj.GetComponent<MonsterHealth>();
                    Char.Instance.dataMyTeam.Add(unit);
                    unit.LevelUp(level);
                    AudioManager.Instance.PlayUnitSound(level, unit.stats.type);
                    grid.Place(unit, x, y);
                    if (Char.Instance.level >= 2) UpgradeCost(true);
                    return;
                }
            }
        }
        Debug.Log("Grid full - cannot spawn unit");
    }
}