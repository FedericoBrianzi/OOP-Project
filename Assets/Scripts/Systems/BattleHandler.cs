using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleHandler : MonoBehaviour
{
    public static BattleHandler Instance;
    private GameObject[] playerTeam = new GameObject[3];
    private GameObject[] enemyTeam = new GameObject[3];

    private void Awake()
    {
        Instance = this;
        GameManager.OnGameStateChanged += GameManagerOnGameStateChanged;
    }

    private void GameManagerOnGameStateChanged(GameManager.GameState state)
    {
        if(state == GameManager.GameState.BattleStart)
        {
            SetPlayerTeam();
            SetEnemyTeam();
            GameManager.Instance.UpdateGameState(GameManager.GameState.PlayerTurn);
        }
    }

    public void SetPlayerTeam()
    {
        playerTeam[0] = SpawnManager.Instance.units[0];
        playerTeam[1] = SpawnManager.Instance.units[1];
        playerTeam[2] = SpawnManager.Instance.units[2];
    }
    public void SetEnemyTeam()
    {
        enemyTeam[0] = SpawnManager.Instance.units[3]; ;
        enemyTeam[1] = SpawnManager.Instance.units[4]; ;
        enemyTeam[2] = SpawnManager.Instance.units[5]; ;
    }
    public GameObject[] GetPlayerTeam()
    {
        return playerTeam;
    }
    public GameObject[] GetEnemyTeam()
    {
        return enemyTeam;
    }
}
