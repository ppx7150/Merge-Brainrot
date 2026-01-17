using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.IO;
public class BattleManager : MonoBehaviour
{
    public bool startPvP; //Kiểm tra xem có đang ở trạng thái Fight không
    public List<GameObject> playerTeam = new List<GameObject>();
    public List<GameObject> enemyTeam = new List<GameObject>();
    public List<GameObject> arrUnitReady = new List<GameObject>();
    public static BattleManager Instance;
    public GameObject meleeEnemyPrefabs;
    public GameObject rangeEnemyPrefabs;
    public GameObject winPanel;
    public GameObject losePanel;
    public GameObject ButtonList;
    public TMP_Text[] txtCoinReward;
    public GameObject rangeEnemyPrefab;
    public GameObject meleeEnemyPrefab;
    private void Awake()
    {
        Instance = this;
    }
    public bool isOkPvP()
    {
        return arrUnitReady.Count == playerTeam.Count + enemyTeam.Count;
    }
    void Update()
    {
        if(startPvP) CheckBattleEnd();
    }
    void CheckBattleEnd() //Kiểm tra xem team nào thắng team nào thua
    {
        if (!enemyTeam.Exists(m => m.activeSelf))
        {
            Debug.Log("Player Win");
            winPanel.SetActive(true);
            AudioManager.Instance.Play(GameSound.victorySound);
            int coin = Random.Range(150, 300);
            txtCoinReward[0].SetText("+" + coin + "$");
            Char.Instance.AddCoins(coin);
            startPvP = false;
            ButtonList.SetActive(true);
            Char.Instance.level++;
            if (Char.Instance.level <= 2) Char.Instance.Save(Application.persistentDataPath + "/save.json");
            Time.timeScale = 0f;
        }
        else if (!playerTeam.Exists(m => m.activeSelf))
        {
            Debug.Log("Enemy Win");
            losePanel.SetActive(true);
            AudioManager.Instance.Play(GameSound.loseSound);
            int coin = Random.Range(100, 200);
            txtCoinReward[1].SetText("+" + coin + "$");
            Char.Instance.AddCoins(coin);
            startPvP = false;
            ButtonList.SetActive(true);
            Time.timeScale = 0f;
        }
    }
    public void resetlevel() //Thua nên bấm nút sẽ chơi lại màn đấy
    {
        Time.timeScale = 1f;
        GridManager.Instance.CLear(4,5);
        arrUnitReady.Clear();
        foreach (var m in enemyTeam)
        {
            m.SetActive(true);
            MonsterAI ai = m.GetComponent<MonsterAI>();
            ai.enabled = false;
            ai.isReady = false;
            m.GetComponent<MonsterHealth>().ResetStatus();
        }
        foreach (var m in playerTeam)
        {
            m.SetActive(true);
            MonsterAI ai = m.GetComponent<MonsterAI>();
            ai.enabled = false;
            ai.isReady = false;
            m.GetComponent<MonsterHealth>().ResetStatus();
        }
        losePanel.SetActive(false);
        AudioManager.Instance.Play(GameSound.coinSound);
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
        Time.timeScale = 1f;
        GridManager.Instance.CLear(4,5);
        arrUnitReady.Clear();
        foreach (var m in playerTeam)
        {
            m.SetActive(true);
            MonsterAI ai = m.GetComponent<MonsterAI>();
            ai.enabled = false;
            ai.isReady = false;
            m.GetComponent<MonsterHealth>().ResetStatus();
        }
        LoadLevel();

        winPanel.SetActive(false);
        AudioManager.Instance.Play(GameSound.coinSound);
        if (Char.Instance.level == 2) TutorialController.Instance.StartPhase2_Merge();
        else if (Char.Instance.level == 3) UnitSpawner.Instance.OnCost();
    }

    public void GenerateEnemy()
    {
        GameObject obj;
        MonsterHealth mh;

        for (int i = 5; i >= 3; i--)
        {
            int sl = Random.Range(1, 4);
            List<int> arr = new List<int> { 0, 1, 2, 3, 4 };
            for (int j = 0; j < sl; j++)
            {
                obj = Instantiate(i == 5 ? rangeEnemyPrefabs : meleeEnemyPrefabs);
                mh = obj.GetComponent<MonsterHealth>();
                mh.LevelUp(Random.Range(1, 3));
                enemyTeam.Add(obj);
                int x = arr[Random.Range(0, arr.Count)];
                arr.Remove(x);
                GridManager.Instance.Place(mh, x, i);
            }
        }
    }
    public void LoadLevel()
    {
        if (!File.Exists(Application.persistentDataPath + "/" + Char.Instance.level + ".json"))
        {
            Debug.Log("No save file level");
            return;
        }
        GridManager grid = GridManager.Instance;
        //CLear het
        grid.CLearEnemy(4,3);
        foreach (var m in enemyTeam)
        {
            if (m == null) continue;
            Destroy(m.gameObject);
        }
        enemyTeam.Clear();
        string json = File.ReadAllText(Application.persistentDataPath + "/" + Char.Instance.level + ".json");
        DataSave dataSave = JsonUtility.FromJson<DataSave>(json);
        foreach (var m in dataSave.enemyTeam.units)
        {
            GameObject obj = Instantiate(m.type == MonsterType.Melee.ToString() ? meleeEnemyPrefab : rangeEnemyPrefab);
            MonsterHealth mh = obj.GetComponent<MonsterHealth>();
            mh.stats.type = (MonsterType)System.Enum.Parse(typeof(MonsterType), m.type);
            mh.SetGridPos(m.gridX, m.gridY);
            mh.LevelUp(m.level - 1);
            grid.Place(mh, mh.gridX, mh.gridY);
            enemyTeam.Add(obj);
        }
    }
}
