using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState State;

    public static event Action<GameState> OnGameStateChanged;

    public enum GameState
    {
        PickTeams,
        BattleStart,
        BattleWon,
        BattleLost
    }

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
            case GameState.BattleWon:
                break;
            case GameState.BattleLost:
                break;
        }
        OnGameStateChanged?.Invoke(newState); ///if the game state changes, the event is sent with the newState
    }

    public void StartBattle()
    {
        UpdateGameState(GameState.BattleStart);
    }

    public void EndBattle(bool won)
    {
        if(won) UpdateGameState(GameState.BattleWon);
        else UpdateGameState(GameState.BattleLost);
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
