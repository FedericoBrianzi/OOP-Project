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
            case GameState.BattleStart:
                break;
            case GameState.BattleEnd:
                HandleBattleEnd();
                break;
        }
        OnGameStateChanged?.Invoke(newState); //if the game state changes, the event is sent with the newState
    }

    private void HandleBattleEnd()
    {
        //throw new NotImplementedException();
    }

    public void StartBattle()
    {
        UpdateGameState(GameState.BattleStart);
    }

    public enum GameState
    {
        PickTeams,
        BattleStart,
        BattleEnd
    }
}
