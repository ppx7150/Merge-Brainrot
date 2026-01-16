using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
[Serializable]
public class DataUnit //Dữ liệu của mỗi Unit
{
    public string type;
    public int level;
    public float maxHP;
    public float attackDamage;
    public float attackSpeed;   // thời gian giữa các đòn
    public float attackRange;   // range đánh / bắn
    public float moveSpeed;
    public int gridX;
    public int gridY;
}
[Serializable]
public class SaveData //Dữ liệu cần lưu
{
    public int coins;
    public int gems;
    public int level;
    public float costMelee;
    public float costRange;
    public Team dataMyTeam;
}
[Serializable]
public class Team
{
    public List<DataUnit> units; //Danh sách Unit hiện có
}
public class Char : MonoBehaviour
{
    public int level; //Level màn chơi của người chơi 
    public int coins; //Số tiền của người chơi 
    public int gems; //Số tiền của người chơi 
    public TMP_Text txtCoins; 
    public TMP_Text txtGems;
    public List<MonsterHealth> dataMyTeam = new List<MonsterHealth>();
    public static Char Instance;
    public GameObject meleePrefabs;
    public GameObject rangePrefabs;
    public int[] damagesRange = { 2, 5, 12, 27, 64, 152, 315, 645 }; //sửa trên unity obj char
    public int[] hpsRange = { 7, 18, 43, 105, 225, 605, 1320, 2765 };
    public int[] damagesMelee = { 1, 3, 7, 15, 33, 70, 150, 315 };
    public int[] hpsMelee = { 18, 45, 115, 270, 625, 1320, 2675, 5550 };
    private void Awake()
    {
        Instance = this;
    }
    public void LoadMyTeamNewBie()
    {
        for (int i = 0; i < 2; i++)
        {
            GameObject obj = Instantiate(i == 0 ? meleePrefabs : rangePrefabs);
            MonsterHealth mh = obj.GetComponent<MonsterHealth>();
            mh.SetGridPos(i == 0 ? 1 : 3, 0);
            mh.stats.type = (i == 0 ? MonsterType.Melee : MonsterType.Ranged);
            mh.LevelUp(0);
            dataMyTeam.Add(mh);
            BattleManager.Instance.playerTeam.Add(obj);
            GridManager.Instance.Place(mh, mh.gridX, mh.gridY);
        }
    }
    void Start()
    {
        Load(Application.persistentDataPath + "/save.json");
        txtCoins.SetText(coins + "$");
        txtGems.SetText(gems.ToString());
        if (level <= 1) TutorialController.Instance.StartPhase1();
    }
    public void Save(string path) //Lưu lại dữ liệu của người chơi
    {
        SaveData saveData = new SaveData();
        saveData.level = level;
        saveData.coins = coins;
        saveData.gems = gems;
        saveData.costMelee = UnitSpawner.Instance.costMelee;
        saveData.costRange = UnitSpawner.Instance.costRange;
        List<DataUnit> data = new List<DataUnit>();
        foreach(var m in dataMyTeam)
        {
            if (m == null) continue;
            data.Add(formatTodata(m));
        }
        saveData.dataMyTeam = new Team
        {
            units = data
        };
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(path, json);
        Debug.Log("Saved: " + path);
    }
    public DataUnit formatTodata(MonsterHealth m) //Hàm chuyển đổi dữ liệu MonsterHealth sang DataUnit
    {
        return new DataUnit
        {
            type = m.stats.type.ToString(),
            level = m.stats.level,
            maxHP = m.stats.maxHP,
            attackDamage = m.stats.attackDamage,
            attackSpeed = m.stats.attackSpeed,
            attackRange = m.stats.attackRange,
            moveSpeed = m.stats.moveSpeed,
            gridX = m.gridX,
            gridY = m.gridY
        };
    }
    public void Load(string path) //Load dữ liệu của người chơi
    {
        if (!File.Exists(path))
        {
            Debug.Log("No save file");
            //LoadMyTeamNewBie();
            return;
        }
        string json = File.ReadAllText(path);
        SaveData saveData = JsonUtility.FromJson<SaveData>(json);
        level = saveData.level;
        coins = saveData.coins;
        gems = saveData.gems;
        UnitSpawner.Instance.LoadCost(saveData.costMelee, saveData.costRange);
        foreach (var m in saveData.dataMyTeam.units)
        {
            MonsterType tp = (MonsterType)Enum.Parse(typeof(MonsterType), m.type); ;
            GameObject obj = Instantiate(tp == MonsterType.Melee? meleePrefabs: rangePrefabs);
            MonsterHealth mh = obj.GetComponent<MonsterHealth>();
            mh.SetGridPos(m.gridX, m.gridY);
            mh.stats.type = tp;
            mh.LevelUp(m.level - 1);
            mh.stats.attackSpeed = m.attackSpeed;
            mh.stats.attackRange = m.attackRange;
            mh.stats.moveSpeed = m.moveSpeed;
            dataMyTeam.Add(mh);
            BattleManager.Instance.playerTeam.Add(obj);
            GridManager.Instance.Place(mh, mh.gridX, mh.gridY);
        }
        Debug.Log("Loaded game");
    }
    public void OnApplicationPause(bool pause) //Lưu dữ liệu khi rời khỏi game(chưa out game)
    {
        if (pause) Save(Application.persistentDataPath + "/save.json");
    }
    private void OnApplicationQuit() //Lưu dữ liệu khi out game
    {
        Save(Application.persistentDataPath + "/save.json");
    }
    public bool SubCoins(int a) //Trừ coin của người chơi
    {
        if (a > coins)
        {
            Debug.Log("Don't enough coins");
            return false;
        }
        coins -= a;
        coins = Mathf.Max(coins, 0);
        txtCoins.SetText(coins + "$");
        return true;
    }
    public void AddCoins(int a) //Thêm coin của người chơi
    {
        coins += a;
        coins = Mathf.Min(coins, int.MaxValue);
        txtCoins.SetText(coins + "$");
    }
    public bool SubGems(int a) //Trừ gem của người chơi
    {
        if (a > gems)
        {
            Debug.Log("Don't enough gems");
            return false;
        }
        gems -= a;
        gems = Mathf.Max(gems, 0);
        txtGems.SetText(gems.ToString());
        return true;
    }
    public void AddGems(int a) //Thêm gem của người chơi
    {
        gems += a;
        gems = Mathf.Min(gems, int.MaxValue);
        txtGems.SetText(gems.ToString());
    }
}