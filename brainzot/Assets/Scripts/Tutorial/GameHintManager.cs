using UnityEngine;
using DG.Tweening; // Khuyên dùng DOTween cho animation mượt

public class GameHintManager : MonoBehaviour
{
    [Header("Settings")]
    public float idleTimeThreshold = 5.0f;

    [Header("References")]
    public RectTransform handCursor; // Ảnh bàn tay (UI)
    public GameObject battleButton;

    private float lastInputTime;
    private bool isHinting = false;

    void Update()
    {
        // 1. Reset timer nếu người chơi chạm vào màn hình
        if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
        {
            lastInputTime = Time.time;
            StopAllHints();
        }

        // 2. Kiểm tra thời gian Idle
        if (!isHinting && Time.time - lastInputTime > idleTimeThreshold)
        {
            DecideAndShowHint();
        }
    }

    void StopAllHints()
    {
        isHinting = false;
        handCursor.gameObject.SetActive(false);
        handCursor.DOKill(); // Dừng tween bàn tay
        battleButton.transform.DOKill(); // Dừng tween nút battle
        battleButton.transform.localScale = Vector3.one; // Reset scale
    }

    void DecideAndShowHint()
    {
        // 1. Tìm cặp merge
        var mergePair = FindMergeablePair();

        // Lưu ý: mergePair là nullable struct, cần check .HasValue hoặc != null
        if (mergePair != null)
        {
            isHinting = true;
            // Gọi hàm hiển thị bàn tay (đã viết ở câu trả lời trước)
            // mergePair.Value.startObj lấy phần tử thứ nhất của tuple
            ShowMergeHint(mergePair.Value.startObj, mergePair.Value.endObj);
        }
        // 2. Nếu không có gì để merge thì check xem battle được chưa
        else if (CanStartBattle())
        {
            isHinting = true;
            ShowBattleHint();
        }
    }
    void ShowMergeHint(Transform charA, Transform charB)
    {
        handCursor.gameObject.SetActive(true);

        // Chuyển vị trí từ World Space (Game 3D/2D) sang UI Screen Space
        Vector3 posA = Camera.main.WorldToScreenPoint(charA.position);
        Vector3 posB = Camera.main.WorldToScreenPoint(charB.position);

        // Reset vị trí về A
        handCursor.position = posA;

        // Sử dụng DOTween để tạo Sequence
        Sequence seq = DOTween.Sequence();

        // Hiệu ứng: Hiện tay -> Di chuyển đến B -> Ẩn -> Lặp lại
        seq.Append(handCursor.GetComponent<CanvasGroup>().DOFade(1, 0.2f)); // Hiện lên
        seq.Append(handCursor.DOMove(posB, 1.0f).SetEase(Ease.InOutQuad)); // Di chuyển
        seq.Append(handCursor.GetComponent<CanvasGroup>().DOFade(0, 0.2f)); // Mờ đi
        seq.AppendInterval(0.5f); // Nghỉ một chút

        seq.SetLoops(3); // Lặp lại 3 lần
        seq.OnComplete(() => {
            handCursor.gameObject.SetActive(false);
            // Sau khi xong 3 lần thì làm gì? 
            // Có thể reset timer để 5s sau nhắc lại nếu vẫn chưa merge
            lastInputTime = Time.time;
            isHinting = false;
        });
    }

    // Logic tìm cặp (Bạn cần tự viết dựa trên code game của bạn)
    (Transform startObj, Transform endObj)? FindMergeablePair()
    {
        var grid = GridManager.Instance;

        // Vòng lặp tìm Unit A
        for (int x1 = 0; x1 < grid.columns; x1++)
        {
            for (int y1 = 0; y1 < grid.rows; y1++)
            {
                MonsterHealth unitA = grid.GetUnit(x1, y1);

                // Bỏ qua nếu ô trống hoặc Unit đã đạt cấp tối đa (Level 8)
                // (Không gợi ý merge nếu đã max cấp)
                if (unitA == null || unitA.stats.level >= 8) continue;

                // Vòng lặp tìm Unit B để so sánh với A
                for (int x2 = 0; x2 < grid.columns; x2++)
                {
                    for (int y2 = 0; y2 < grid.rows; y2++)
                    {
                        MonsterHealth unitB = grid.GetUnit(x2, y2);

                        // Các điều kiện để bỏ qua:
                        // 1. Ô B trống
                        // 2. Unit A và B là cùng một object
                        if (unitB == null || unitA == unitB) continue;

                        // --- LOGIC KIỂM TRA MERGE ĐƯỢC HAY KHÔNG ---

                        // 1. Kiểm tra cùng loại (Melee vs Ranged)
                        bool sameType = unitA.stats.type == unitB.stats.type;

                        // 2. Kiểm tra cùng cấp độ (Level 1 vs Level 1)
                        bool sameLevel = unitA.stats.level == unitB.stats.level;

                        if (sameType && sameLevel)
                        {
                            // Tìm thấy cặp! Trả về transform để vẽ bàn tay
                            return (unitA.transform, unitB.transform);
                        }
                    }
                }
            }
        }

        // Không tìm thấy cặp nào
        return null;
    }
    void ShowBattleHint()
    {
        // Hiệu ứng Breath: Scale to lên 1.1 rồi nhỏ lại 1.0
        battleButton.transform.DOScale(1.1f, 0.8f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo); // Loop vĩnh viễn cho đến khi người chơi chạm màn hình
    }

    bool CanStartBattle()
    {
        var grid = GridManager.Instance;

        // Duyệt grid, chỉ cần thấy 1 con unit bất kỳ là return true ngay
        for (int x = 0; x < grid.columns; x++)
        {
            for (int y = 0; y < grid.rows; y++)
            {
                if (!grid.IsEmpty(x, y))
                {
                    return true;
                }
            }
        }
        return false;
    }
}