using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public bool startPvP;
    public List<GameObject> playerTeam = new List<GameObject>();
    public List<GameObject> enemyTeam = new List<GameObject>();
    public static BattleManager Instance;
    public GameObject meleeEnemyPrefabs;
    public GameObject rangeEnemyPrefabs;
    public GameObject winPanel;
    public GameObject losePanel;
    private void Awake()
    {
        Instance = this;
    }
    void Update()
    {
        if(startPvP) CheckBattleEnd();
    }
    void CheckBattleEnd()
    {
        if (!playerTeam.Exists(m => m.activeSelf))
        {
            Debug.Log("Enemy Win");
            losePanel.SetActive(true);
            startPvP = false;
        } else if (!enemyTeam.Exists(m => m.activeSelf))
        {
            Debug.Log("Player Win");
            winPanel.SetActive(true);
            startPvP = false;
        }
    }
    public void resetlevel()
    {
        GridManager.Instance.CLear();
        foreach (var m in enemyTeam)
        {
            m.SetActive(true);
            m.GetComponent<MonsterAI>().enabled = false;
            MonsterHealth mh = m.GetComponent<MonsterHealth>();
            m.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            mh.ResetStatus();
            GridManager.Instance.Place(mh, mh.gridX, mh.gridY);
        }
        foreach (var m in playerTeam)
        {
            m.SetActive(true);
            m.GetComponent<MonsterAI>().enabled = false;
            MonsterHealth mh = m.GetComponent<MonsterHealth>();
            m.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            mh.ResetStatus();
            GridManager.Instance.Place(mh, mh.gridX, mh.gridY);
        }
        losePanel.SetActive(false);
    }
    public void StartBattle()
    {
        playerTeam.RemoveAll(m => m == null);
        if (!playerTeam.Exists(m => m.activeSelf)) return;
        foreach (var m in playerTeam)
        {
            if (m == null) continue;
            m.GetComponent<MonsterAI>().enabled = true;
            m.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        }
        foreach (var m in enemyTeam)
        {
            if (m == null) continue;
            m.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            m.GetComponent<MonsterAI>().enabled = true;
        }
        startPvP = true;
    }
    public void ChangeLevelUp()
    {
        GridManager.Instance.CLear();
        Char.Instance.level++;
        foreach (var m in enemyTeam)
        {
            if (m == null) continue;
            Destroy(m);
        }
        enemyTeam.Clear();
        foreach (var m in playerTeam)
        {
            m.SetActive(true);
            m.GetComponent<MonsterAI>().enabled = false;
            m.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            MonsterHealth mht = m.GetComponent<MonsterHealth>();
            mht.ResetStatus();
            GridManager.Instance.Place(mht, mht.gridX, mht.gridY);
        }
        GameObject obj;
        MonsterHealth mh;
        for (int i = 5; i >= 3; i--)
        {
            int sl = Random.Range(1, 4);
            List<int> arr = new List<int> { 0, 1, 2, 3, 4 };
            for (int j =0; j < sl; j++)
            {
                obj = Instantiate(i == 5 ? rangeEnemyPrefabs: meleeEnemyPrefabs);
                mh = obj.GetComponent<MonsterHealth>();
                obj.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                mh.LevelUp(Random.Range(1, 3));
                enemyTeam.Add(obj);
                int x = arr[Random.Range(0, arr.Count)];
                arr.Remove(x);
                GridManager.Instance.Place(mh, x, i);
            }
        }
        winPanel.SetActive(false);
    }
}
