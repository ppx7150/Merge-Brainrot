using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;
using UnityEngine.UIElements;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    [Header("Grid Size")]   //khai báo 6 hàng 5 cột.
    public int columns = 5;
    public int rows = 6;

    [Header("Layout")]  //khai báo kích thước và vị trí của grid
    public float cellSize = 1.6f;       //khoảng cách giữa tâm các ô
    public Vector2 origin = new Vector2(-3.2f, -4.0f);      //tọa độ ô (0,0)

    private MonsterHealth[,] grid;   //mảng 2 chiều grid

    void Awake()
    {
        Instance = this;
        grid = new MonsterHealth[columns, rows];     //khởi tạo grid
    }

    public Vector3 GetWorldPos(int x, int y)        
    {
        return new Vector3(origin.x + x * cellSize, origin.y + y * cellSize, 0);
    }

    public bool IsValid(int x, int y)   //kiểm tra xem ô có nằm trong grid không
    {
        return (x >= 0) && (x < columns) && (y >= 0) && (y < rows);
    }

    public bool IsEmpty(int x, int y)   //kiểm tra ô có trống hay không
    {
        return IsValid(x, y) && (grid[x, y] == null);
    }

    public MonsterHealth GetUnit(int x, int y)   //lấy unit tại vị trí (x,y)
    {
        //return IsValid(x, y) ? grid[x, y] : null;
        if (IsValid(x, y))
            return grid[x, y];
        else
            return null;
    }

    public void Place(MonsterHealth unit, int x, int y)  //đặt unit vào vị trí (x,y)
    {
        if (!IsEmpty(x, y)) return;

        grid[x, y] = unit;
        unit.SetGridPos(x, y);
        unit.transform.position = GetWorldPos(x, y);
    }

    public void Remove(int x, int y)    //đánh dấu vị trí cũ của unit là null
    {
        if (!IsValid(x, y)) return;
        grid[x, y] = null;
    }

    void OnDrawGizmos()     //vẽ grid trên Scene để xem
    {
        Gizmos.color = Color.gray;

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Gizmos.DrawWireCube(
                    GetWorldPos(x, y),
                    Vector3.one * cellSize
                );
            }
        }
    }
    public Vector2 ClampToGrid(Vector2 worldPos)
    {
        float minX = origin.x;
        float maxX = origin.x + (columns - 1) * cellSize;

        float minY = origin.y;
        float maxY = origin.y + (rows - 1) * cellSize;

        float clampedX = Mathf.Clamp(worldPos.x, minX, maxX);
        float clampedY = Mathf.Clamp(worldPos.y, minY, maxY);

        return new Vector2(clampedX, clampedY);
    }

}
