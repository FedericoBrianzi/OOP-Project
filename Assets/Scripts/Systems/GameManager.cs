using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState State;

    public static event Action<GameState> OnGameStateChanged;

    private GameObject[] playerTeam = new GameObject[3];
    private GameObject[] enemyTeam = new GameObject[3];

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateGameState(GameState.PickTeams);
    }

    public void UpdateGameState(GameState newState)
    {
        State = newState;

        switch (newState)
        {
            case GameState.PickTeams:
                break;
            case GameState.BattleStart:
                HandleBattleStart();
                break;
            case GameState.PlayerTurn:
                HandlePlayerTurn();
                break;
            case GameState.PlayAttacks:

                break;
            case GameState.BattleEnd:
                HandleBattleEnd();
                break;
        }
        OnGameStateChanged?.Invoke(newState);
    }
    private void HandleBattleStart()
    {
        //playerTeam[0] = SpawnManager.Instance.units[0];
        //playerTeam[1] = SpawnManager.Instance.units[1];
        //playerTeam[2] = SpawnManager.Instance.units[2];
        //enemyTeam[0] = SpawnManager.Instance.units[3];
        //enemyTeam[1] = SpawnManager.Instance.units[4];
        //enemyTeam[2] = SpawnManager.Instance.units[5];
        UpdateGameState(GameState.PlayerTurn);
    } 
    private void HandlePlayerTurn()
    {
        //playerTeam[0].GetComponent<BaseClass>().myTurn = true;
    }

    private void HandleBattleEnd()
    {
        //throw new NotImplementedException();
    }

    public void StartBattle()
    {
        UpdateGameState(GameState.BattleStart);
    }

    public GameObject[] GetPlayerTeam()
    {
        return playerTeam;
    }
    public GameObject[] GetEnemyTeam()
    {
        return enemyTeam;
    }

    public enum GameState
    {
        PickTeams,
        BattleStart,
        PlayerTurn,
        PlayAttacks,
        BattleEnd
    }
}
