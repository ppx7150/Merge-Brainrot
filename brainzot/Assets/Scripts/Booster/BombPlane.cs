using UnityEngine;
using System;
using System.Collections;

public class BombPlane : MonoBehaviour
{
    public float speed;

    private Action onDropBomb;
    private bool dropped;
    private float targetDropX = 0f;

    private Vector3 endPos;
    private Vector3 dir;

    public void Init(Action dropCallback)
    {
        onDropBomb = dropCallback;
        dropped = false;
        StartFly(UnityEngine.Random.value < 0.5);
    }

    void StartFly(bool isLeftToRight)
    {
        float y = Camera.main.ViewportToWorldPoint(new Vector3(0, UnityEngine.Random.Range(0.75f, 0.85f), 10)).y;
        Vector3 start, end;

        if (isLeftToRight)
        {
            start = Camera.main.ViewportToWorldPoint(new Vector3(-0.2f, 0.8f, 10));
            end = Camera.main.ViewportToWorldPoint(new Vector3(1.2f, 0.8f, 10));
            transform.localScale = Vector3.one;
        }
        else
        {
            start = Camera.main.ViewportToWorldPoint(new Vector3(1.2f, 0.8f, 10));
            end = Camera.main.ViewportToWorldPoint(new Vector3(-0.2f, 0.8f, 10));
            transform.localScale = new Vector3(-1, 1, 1); // lật sprite
        }
        start.y = end.y = y;
        transform.position = start;
        endPos = end;
        dir = (end - start).normalized;
    }

    void Update()
    {
        transform.position += dir * speed * Time.deltaTime;

        // ===== THẢ BOM TẠI X = 0 =====
        if (!dropped && HasReachedDropX())
        {
            dropped = true;
            onDropBomb?.Invoke();
        }

        // Kết thúc đường bay
        if (Vector3.Dot(endPos - transform.position, dir) <= 0 || !BattleManager.Instance.startPvP)
        {
            BombPlanePool.Instance.Release(this);
        }
    }
    bool HasReachedDropX()
    {
        // Bay từ trái sang phải
        if (dir.x > 0)
            return transform.position.x >= targetDropX;

        // Bay từ phải sang trái
        return transform.position.x <= targetDropX;
    }
}
