using TMPro;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    public int costMelee = 100;
    public int costRange = 100;
    public TMP_Text txtCostMelee;
    public TMP_Text txtCostRange;
    public BattleManager battleManager;
    public GameObject rangeUnitPrefab;
    public GameObject meleeUnitPrefab;
    public static UnitSpawner Instance;
    private void Awake()
    {
        Instance = this;
        LoadCost(costMelee, costRange);
    }
    public void LoadCost(int cM, int cR)
    {
        costMelee = cM;
        costRange = cR;
        txtCostMelee.SetText(costMelee + "$");
        txtCostRange.SetText(costRange + "$");
    }
    public void UpgradeCost(bool isMelee)
    {
        if (isMelee)
        {
            costMelee += 20;
            txtCostMelee.SetText(costMelee + "$");
        } else
        {
            costRange += 20;
            txtCostRange.SetText(costRange + "$");
        }
    }
    public void SpawnRangeUnit(int level)
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
                    MonsterHealth unit = unitObj.GetComponent<MonsterHealth>();
                    unit.LevelUp(level);
                    grid.Place(unit, x, y);
                    battleManager.playerTeam.Add(unitObj);
                    Char.Instance.dataMyTeam.Add(unit);
                    return;
                }
            }
        }
        Debug.Log("Grid full - cannot spawn unit");
    }

    public void SpawnMeleeUnit(int level)
    {
        GridManager grid = GridManager.Instance;

        for (int y = 2; y >= 0; y--)
        {
            for (int x = 0; x <= 4; x++)
            {
                if (grid.IsEmpty(x, y))
                {
                    GameObject unitObj = Instantiate(meleeUnitPrefab);
                    MonsterHealth unit = unitObj.GetComponent<MonsterHealth>();
                    unit.LevelUp(level);
                    grid.Place(unit, x, y);
                    battleManager.playerTeam.Add(unitObj);
                    Char.Instance.dataMyTeam.Add(unit);
                    return;
                }
            }
        }
        Debug.Log("Grid full - cannot spawn unit");
    }
}