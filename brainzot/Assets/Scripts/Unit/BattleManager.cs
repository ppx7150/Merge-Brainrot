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
    public GameObject Booster;
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
    public float GetDifficulty(int level)
    {
        var cfg = BattleConfig.Instance.difficulty;
        var manual = cfg.manualLevels.ToDictionary();

        if (manual.ContainsKey(level))
            return manual[level];

        int groupIndex = (level - 11) / 5;
        int indexInGroup = (level - 11) % 5;

        return cfg.baseGroup[indexInGroup] + groupIndex * cfg.groupIncrease;
    }
    public long CalulatorReward(bool isWin)
    {
        var rewardCfg = BattleConfig.Instance.reward;
        double reward = (UnitSpawner.Instance.costMelee + UnitSpawner.Instance.costRange) / 2.0 * (isWin ? rewardCfg.winMultiplier : rewardCfg.loseMultiplier) * GetDifficulty(Char.Instance.level);
        //FAIL SAFE REWARD
        if (!isWin && LoseTracker.IsFailSafeActive())
        {
            reward *= FailSafeConfig.Instance.failSafe.rewardBonus;
        }
        return (long)System.Math.Round(reward);
    }

    void CheckBattleEnd() //Kiểm tra xem team nào thắng team nào thua
    {
        if (!enemyTeam.Exists(m => m.activeSelf))
        {
            GameLog.Log("battle_win", new
            {
                level = Char.Instance.level,
                loseStreak = LoseTracker.loseStreak,
                mergeCount = MergeTracker.mergeCount
            });
            LoseTracker.OnWin();
            Debug.Log("Player Win");
            winPanel.SetActive(true);
            AudioManager.Instance.Play(GameSound.victorySound);
            long coin = CalulatorReward(true);
            txtCoinReward[0].SetText("+" + coin + "$");
            Char.Instance.AddCoins(coin);
            startPvP = false;
            ButtonList.SetActive(true);
            Booster.SetActive(false);
            Char.Instance.level++;
            if (Char.Instance.level <= 2) Char.Instance.Save(Application.persistentDataPath + "/save.json");
            Time.timeScale = 0f;
        }
        else if (!playerTeam.Exists(m => m.activeSelf))
        {
            GameLog.Log("battle_lose", new
            {
                level = Char.Instance.level,
                loseStreak = LoseTracker.loseStreak
            });
            LoseTracker.OnLose();
            Debug.Log("Enemy Win");
            losePanel.SetActive(true);
            AudioManager.Instance.Play(GameSound.loseSound);
            long coin = CalulatorReward(false);
            txtCoinReward[1].SetText("+" + coin + "$");
            Char.Instance.AddCoins(coin);
            startPvP = false;
            ButtonList.SetActive(true);
            Booster.SetActive(false);
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
            Destroy(ai.projectile);
            m.GetComponent<MonsterHealth>().ResetStatus();
        }
        foreach (var m in playerTeam)
        {
            m.SetActive(true);
            MonsterAI ai = m.GetComponent<MonsterAI>();
            ai.enabled = false;
            ai.isReady = false;
            Destroy(ai.projectile);
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
        Booster.SetActive(true);
    }
    public void ChangeLevelUp() //Thắng nên bấm nút sẽ chuyển tới level tiếp theo
    {
        MergeTracker.Reset();
        Time.timeScale = 1f;
        GridManager.Instance.CLear(4,5);
        arrUnitReady.Clear();
        foreach (var m in playerTeam)
        {
            m.SetActive(true);
            MonsterAI ai = m.GetComponent<MonsterAI>();
            ai.enabled = false;
            ai.isReady = false;
            Destroy(ai.projectile);
            m.GetComponent<MonsterHealth>().ResetStatus();
        }
        LoadLevel();

        winPanel.SetActive(false);
        AudioManager.Instance.Play(GameSound.coinSound);
        if (Char.Instance.level == 2) {
            TutorialController.Instance.StartPhase2_Merge();
            UnitSpawner.Instance.OnCost();
        }
    }

    //public void GenerateEnemy()
    //{
    //    GameObject obj;
    //    MonsterHealth mh;

    //    for (int i = 5; i >= 3; i--)
    //    {
    //        int sl = Random.Range(1, 4);
    //        List<int> arr = new List<int> { 0, 1, 2, 3, 4 };
    //        for (int j = 0; j < sl; j++)
    //        {
    //            obj = Instantiate(i == 5 ? rangeEnemyPrefabs : meleeEnemyPrefabs);
    //            mh = obj.GetComponent<MonsterHealth>();
    //            mh.LevelUp(Random.Range(1, 3));
    //            enemyTeam.Add(obj);
    //            int x = arr[Random.Range(0, arr.Count)];
    //            arr.Remove(x);
    //            GridManager.Instance.Place(mh, x, i);
    //        }
    //    }
    //}
    public void LoadLevel()
    {
        GridManager grid = GridManager.Instance;

        // Clear enemy cũ
        grid.CLearEnemy(4, 3);
        foreach (var m in enemyTeam)
        {
            if (m == null) continue;
            MonsterAI ai = m.GetComponent<MonsterAI>();
            Destroy(ai.projectile);
            Destroy(m.gameObject);
        }
        enemyTeam.Clear();
        Char.Instance.txtLevel.SetText(Noti.Get("level_format", Char.Instance.level));
        // Load JSON từ Resources
        string path = "Level/" + Char.Instance.level;
        TextAsset jsonFile = Resources.Load<TextAsset>(path);

        if (jsonFile == null)
        {
            Debug.LogError($"❌ Không tìm thấy file level: Resources/{path}.json");
            return;
        }

        DataSave dataSave = JsonUtility.FromJson<DataSave>(jsonFile.text);

        foreach (var m in dataSave.enemyTeam.units)
        {
            GameObject prefab = m.type == MonsterType.Melee.ToString() ? meleeEnemyPrefab : rangeEnemyPrefab;
            GameObject obj = Instantiate(prefab);
            MonsterHealth mh = obj.GetComponent<MonsterHealth>();
            mh.stats.type = (MonsterType)System.Enum.Parse(typeof(MonsterType), m.type);
            mh.SetGridPos(m.gridX, m.gridY);
            mh.LevelUp(m.level - 1);
            grid.Place(mh, mh.gridX, mh.gridY);
            enemyTeam.Add(obj);
        }
    }

}
