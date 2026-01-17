using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    public float costMelee = 100;
    public float costRange = 100;
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
    public void LoadCost(float cM, float cR) //Làm mới giá tiền mua Unit
    {
        costMelee = cM;
        costRange = cR;
        txtCostMelee.SetText((int)costMelee + "$");
        txtCostRange.SetText((int)costRange + "$");
    }
    public void UpgradeCost(bool isMelee) //Nâng giá tiền mua Unit
    {
        if (!IsGridFull())
        {
            if (isMelee)
            {
                costMelee *= Char.Instance.level < 15 ? 1.175f:1.195f;
                txtCostMelee.SetText((int)costMelee + "$");
            }
            else
            {
                costRange *= Char.Instance.level < 15 ? 1.175f : 1.195f;
                txtCostRange.SetText((int)costRange + "$");
            }
        }
        
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
                    MonsterHealth unit = unitObj.GetComponent<MonsterHealth>();
                    unit.LevelUp(level);
                    AudioManager.Instance.PlayUnitSound(level, unit.stats.type);
                    grid.Place(unit, x, y);
                    battleManager.playerTeam.Add(unitObj);
                    Char.Instance.dataMyTeam.Add(unit);
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
                    MonsterHealth unit = unitObj.GetComponent<MonsterHealth>();
                    unit.LevelUp(level);
                    AudioManager.Instance.PlayUnitSound(level, unit.stats.type);
                    grid.Place(unit, x, y);
                    battleManager.playerTeam.Add(unitObj);
                    Char.Instance.dataMyTeam.Add(unit);
                    return;
                }
            }
        }
        Debug.Log("Grid full - cannot spawn unit");
    }

    public bool IsGridFull()
    {
        GridManager grid = GridManager.Instance;
        for (int y = 0; y <= 2; y++)
        {
            for (int x = 4; x >= 0; x--)
            {
                if (grid.IsEmpty(x, y))
                {

                    return false;
                }
            }
        }
        return true;
    }

}