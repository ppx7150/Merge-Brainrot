using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public bool startPvP;
    public List<GameObject> playerTeam = new List<GameObject>();
    public List<GameObject> enemyTeam = new List<GameObject>();

    public static BattleManager Instance;
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
        playerTeam.RemoveAll(m => m == null);
        enemyTeam.RemoveAll(m => m == null);
        if (playerTeam.Count == 0)
        {
            Debug.Log("Enemy Win");
            Time.timeScale = 0f;
        } else if (enemyTeam.Count == 0)
        {
            Debug.Log("Player Win");
            Time.timeScale = 0f;
        }
    }
    public void StartBattle()
    {
        playerTeam.RemoveAll(m => m == null);
        enemyTeam.RemoveAll(m => m == null);
        foreach (var m in playerTeam)
            m.GetComponent<MonsterAI>().enabled = true;

        foreach (var m in enemyTeam)
            m.GetComponent<MonsterAI>().enabled = true;
        startPvP = true;
    }
}
