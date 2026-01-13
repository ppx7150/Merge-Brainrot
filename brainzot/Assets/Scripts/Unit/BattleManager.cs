using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class BattleManager : MonoBehaviour
{
    public bool startPvP; //Kiểm tra xem có đang ở trạng thái Fight không
    public List<GameObject> playerTeam = new List<GameObject>();
    public List<GameObject> enemyTeam = new List<GameObject>();
    public static BattleManager Instance;
    public GameObject meleeEnemyPrefabs;
    public GameObject rangeEnemyPrefabs;
    public GameObject winPanel;
    public GameObject losePanel;
    public GameObject ButtonList;
    public TMP_Text[] txtCoinReward;
    private void Awake()
    {
        Instance = this;
    }
    void Update()
    {
        if(startPvP) CheckBattleEnd();
    }
    void CheckBattleEnd() //Kiểm tra xem team nào thắng team nào thua
    {
        if (!playerTeam.Exists(m => m.activeSelf))
        {
            Debug.Log("Enemy Win");
            losePanel.SetActive(true);
            int coin = Random.Range(100, 200);
            txtCoinReward[1].SetText("+" + coin + "$");
            Char.Instance.AddCoins(coin);
            startPvP = false;
            ButtonList.SetActive(true);
        } else if (!enemyTeam.Exists(m => m.activeSelf))
        {
            Debug.Log("Player Win");
            winPanel.SetActive(true);
            int coin = Random.Range(150, 300);
            txtCoinReward[0].SetText("+" + coin + "$");
            Char.Instance.AddCoins(coin);
            startPvP = false;
            ButtonList.SetActive(true);
        }
    }
    public void resetlevel() //Thua nên bấm nút sẽ chơi lại màn đấy
    {
        GridManager.Instance.CLear();
        foreach (var m in enemyTeam)
        {
            m.SetActive(true);
            m.GetComponent<MonsterAI>().enabled = false;
            m.GetComponent<MonsterHealth>().ResetStatus();
        }
        foreach (var m in playerTeam)
        {
            m.SetActive(true);
            m.GetComponent<MonsterAI>().enabled = false;
            m.GetComponent<MonsterHealth>().ResetStatus();
        }
        losePanel.SetActive(false);
    }
    public void StartBattle() //Bắt đầu Fight
    {
        playerTeam.RemoveAll(m => m == null);
        if (!playerTeam.Exists(m => m.activeSelf)) return;
        foreach (var m in playerTeam)
        {
            if (m == null) continue;
            m.GetComponent<MonsterAI>().enabled = true;
        }
        foreach (var m in enemyTeam)
        {
            if (m == null) continue;
            m.GetComponent<MonsterAI>().enabled = true;
        }
        startPvP = true;
        ButtonList.SetActive(false);
    }
    public void ChangeLevelUp() //Thắng nên bấm nút sẽ chuyển tới level tiếp theo
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
            m.GetComponent<MonsterHealth>().ResetStatus();
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
