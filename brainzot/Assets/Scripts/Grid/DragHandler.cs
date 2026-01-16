using UnityEngine;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private MonsterHealth unit;
    private Vector3 offset;

    void Awake()
    {
        unit = GetComponent<MonsterHealth>();
    }
    bool IsPointerOverUI()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        return EventSystem.current.IsPointerOverGameObject();
#else
    if (Input.touchCount > 0)
        return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
    return false;
#endif
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (BattleManager.Instance.startPvP || BattleManager.Instance.winPanel.activeSelf || BattleManager.Instance.losePanel.activeSelf) return;
        offset = transform.position - GetMouseWorldPos();   
        GridManager.Instance.Remove(unit.gridX, unit.gridY);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (BattleManager.Instance.startPvP || BattleManager.Instance.winPanel.activeSelf || BattleManager.Instance.losePanel.activeSelf) return;
        transform.position = GetMouseWorldPos() + offset;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (BattleManager.Instance.startPvP || BattleManager.Instance.winPanel.activeSelf || BattleManager.Instance.losePanel.activeSelf) return;
        TrySnap();
    }
    void TrySnap()  //thay đổi vị trí unit
    {
        GridManager grid = GridManager.Instance;

        int x = Mathf.RoundToInt((transform.position.x - grid.origin.x) / grid.cellSize);   //tính tọa độ (x,y) của vị trí mới
        int y = Mathf.RoundToInt((transform.position.y - grid.origin.y) / grid.cellSize);

        if (!grid.IsValid(x, y) || y > 2 || (TutorialController.Instance.currentState == TutorialController.TutorialState.Phase2_DragMerge && !TutorialController.Instance.isSucessPos(new Vector2(unit.gridX, unit.gridY), new Vector2(x, y))))  //nếu vị trí nằm ngoài grid thì trả về chỗ cũ
        {
            SnapBack();
            if(TutorialController.Instance.currentState == TutorialController.TutorialState.Phase2_DragMerge) unit.GetComponent<SpriteRenderer>().sortingOrder = 95;
            return;
        }

        MonsterHealth other = grid.GetUnit(x, y);

        if (other != null)
        {
            if (other.stats.level == unit.stats.level && other.stats.type == unit.stats.type)
            {
                TryMerge(other);
            }
            else
            {
                SwapWith(other);
            }
            return;
        }

        grid.Place(unit, x, y);     //nếu vị trí mới đang không có unit thì đặt unit vào vị trí mới
    }

    void SnapBack()     //trả unit về vị trí cũ trong trường hợp không di chuyển được
    {
        GridManager.Instance.Place(unit, unit.gridX, unit.gridY);
    }

    void TryMerge(MonsterHealth other)   //hàm merge unit, cần sửa
    {
        if (other.stats.level != unit.stats.level)
        {
            SnapBack();
            return;
        }

        // MERGE
        GridManager.Instance.Remove(other.gridX, other.gridY);
        Destroy(other.gameObject);
        unit.GetComponent<SpriteRenderer>().sortingOrder = -other.gridY;
        BattleManager.Instance.playerTeam.Remove(other.gameObject);
        unit.LevelUp(1);
        if (TutorialController.Instance != null)
        {
            TutorialController.Instance.OnMergeCompleted();
        }
        GridManager.Instance.Place(unit, other.gridX, other.gridY);
        Char.Instance.dataMyTeam.RemoveAll(m => m == null);
    }

    void SwapWith(MonsterHealth other)
    {
        GridManager grid = GridManager.Instance;

        // Lưu vị trí cũ của unit đang kéo
        int oldX = unit.gridX;
        int oldY = unit.gridY;

        // Vị trí của unit còn lại
        int otherX = other.gridX;
        int otherY = other.gridY;

        // Gỡ unit còn lại khỏi grid
        grid.Remove(otherX, otherY);

        // Đặt unit đang kéo vào vị trí mới
        grid.Place(unit, otherX, otherY);

        // Đặt unit còn lại về vị trí cũ
        grid.Place(other, oldX, oldY);
    }


    Vector3 GetMouseWorldPos() //lấy vị trí chuột
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition); //chuyển từ tọa độ screen(màn hình điện thoại) sang tọa độ world(Unity)
        pos.z = 0;
        return pos;
    }
}
