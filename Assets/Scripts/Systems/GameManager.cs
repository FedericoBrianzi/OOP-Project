using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState State;

    public static event Action<GameState> OnGameStateChanged;

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
            case GameState.PlayerTurn:
                HandlePlayerTurn();
                break;
            case GameState.PlayAttacks:
                HandleBattleEnd();
                break;
            case GameState.BattleEnd:
                HandleBattleEnd();
                break;
        }
        OnGameStateChanged?.Invoke(newState);
    }

    private void HandlePlayerTurn()
    {
        throw new NotImplementedException();
    }

    private void HandleBattleEnd()
    {
        throw new NotImplementedException();
    }

    public void StartBattle()
    {
        UpdateGameState(GameState.PlayerTurn);
    }

    public enum GameState
    {
        PickTeams,
        PlayerTurn,
        PlayAttacks,
        BattleEnd
    }
}
