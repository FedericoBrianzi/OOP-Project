using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    private BattleStateMachine BSM;
    private BaseClass myClass;

    [SerializeField] private TurnState currentState;
    private void Awake()
    {
        GameManager.OnGameStateChanged += GameManagerOnGameStateChanged;
        BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
        myClass = GetComponent<BaseClass>();
        currentState = TurnState.TEAMSELECTION;
    }

    private void GameManagerOnGameStateChanged(GameManager.GameState state)
    {
        if (state == GameManager.GameState.BattleStart)
        {
            currentState = TurnState.ADDTOLIST;
        }
    }

    void Update()
    {
        switch (currentState)
        {
            case TurnState.TEAMSELECTION:
                break;
            case TurnState.ADDTOLIST:
                BSM.AddHeroToList(gameObject);
                currentState = TurnState.WAITING;
                break;
            case TurnState.WAITING:
                //waiting to perform the action
                if (BSM.actionsToPerform.Count == 0 && BSM.battleState == BattleStateMachine.BattleState.WAIT)    //questo andrebbe credo in action, poi vedremo
                {
                    currentState = TurnState.ADDTOLIST;
                }
                break;
            case TurnState.ACTION:
                break;
            case TurnState.DEAD:
                break;
        }
    }

    private enum TurnState
    {
        TEAMSELECTION,
        ADDTOLIST,
        WAITING,
        ACTION,
        DEAD
    }
}
