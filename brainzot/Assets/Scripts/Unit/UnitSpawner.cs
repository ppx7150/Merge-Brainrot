using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    public BattleManager battleManager;
    public GameObject rangeUnitPrefab;
    public GameObject meleeUnitPrefab;

    public void SpawnRangeUnit()
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

                    grid.Place(unit, x, y);
                    battleManager.playerTeam.Add(unitObj);
                    return;
                }
            }
        }

        Debug.Log("Grid full - cannot spawn unit");
    }

    public void SpawnMeleeUnit()
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

                    grid.Place(unit, x, y);
                    battleManager.playerTeam.Add(unitObj);
                    return;
                }
            }
        }

        Debug.Log("Grid full - cannot spawn unit");
    }
}