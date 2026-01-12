using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
[Serializable]
public class DataUnit
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
public class SaveData
{
    public int coins;
    public int gems;
    public int level;
    public int costMelee;
    public int costRange;
    public Team dataMyTeam;
}
[Serializable]
public class Team
{
    public List<DataUnit> units;
}
public class Char : MonoBehaviour
{
    public int level;
    public int coins;
    public int gems;
    public TMP_Text txtCoins;
    public TMP_Text txtGems;
    public List<MonsterHealth> dataMyTeam = new List<MonsterHealth>();
    public static Char Instance;
    public GameObject meleePrefabs;
    public GameObject rangePrefabs;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        Load(Application.persistentDataPath + "/save.json");
        txtCoins.SetText(coins + "$");
        txtGems.SetText(gems.ToString());
    }
    public void Save(string path)
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
    public DataUnit formatTodata(MonsterHealth m)
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
    public void Load(string path)
    {
        if (!File.Exists(path))
        {
            Debug.Log("No save file");
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
            mh.gridX = m.gridX;
            mh.gridY = m.gridY;
            mh.stats.type = tp;
            mh.stats.level = m.level;
            mh.stats.maxHP = m.maxHP;
            mh.stats.attackDamage = m.attackDamage;
            mh.stats.attackSpeed = m.attackSpeed;
            mh.stats.attackRange = m.attackRange;
            mh.stats.moveSpeed = m.moveSpeed;
            dataMyTeam.Add(mh);
            BattleManager.Instance.playerTeam.Add(obj);
            GridManager.Instance.Place(mh, mh.gridX, mh.gridY);
            mh.UpdateVisual();
        }
        Debug.Log("Loaded game");
    }
    public void OnApplicationPause(bool pause)
    {
        if (pause) Save(Application.persistentDataPath + "/save.json");
    }
    private void OnApplicationQuit()
    {
        Save(Application.persistentDataPath + "/save.json");
    }
    public bool SubCoins(int a)
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
    public void AddCoins(int a)
    {
        coins += a;
        coins = Mathf.Min(coins, int.MaxValue);
        txtCoins.SetText(coins + "$");
    }
    public bool SubGems(int a)
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
    public void AddGems(int a)
    {
        gems += a;
        gems = Mathf.Min(gems, int.MaxValue);
        txtGems.SetText(gems.ToString());
    }
}