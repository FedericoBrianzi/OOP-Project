using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    private BattleStateMachine BSM;
    private bool isAlive = true;

    public TurnState currentState;
    private void Awake()
    {
        GameManager.OnGameStateChanged += GameManagerOnGameStateChanged;
        BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
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
                if (BSM.actionsToPerform.Count == 0 && BSM.battleState == BattleStateMachine.BattleState.WAIT)
                {
                    currentState = TurnState.ADDTOLIST;
                }
                break;
            case TurnState.ACTION:
                //animations could maybe be put here just to not fill the BSM scripts with them (calling currentState = TurnState.ACTION and starting an animation/Coroutine)
                break;
            case TurnState.DEAD:
                if (!isAlive)
                {
                    return;
                }
                else
                {   
                    if(BSM.actionsToPerform.Count == 0 && BSM.battleState == BattleStateMachine.BattleState.WAIT)
                    {
                        currentState = TurnState.ADDTOLIST;
                    }
                }
                break;
        }
    }

    public IEnumerator DeadPlayerRoutine()
    {
        gameObject.tag = "DeadPlayer";

        BSM.playerTeam.Remove(gameObject);
        BSM.SetTargetAliveBool(false);

        for (int i = 0; i < BSM.actionsToPerform.Count; i++)
        {
            if (BSM.actionsToPerform[i].attackerGO == gameObject)
            {
                BSM.actionsToPerform.RemoveAt(i);
                break;
            }
        }

        for (int i = 0; i < BSM.actionsToPerform.Count; i++)
        {
            if (BSM.actionsToPerform[i].attackTargets.Contains(gameObject))
            {
                BSM.actionsToPerform[i].attackTargets.Remove(gameObject);
                if(BSM.actionsToPerform[i] != BSM.actionsToPerform[0] && BSM.playerTeam.Count > 0) BSM.SetNewTarget(BSM.actionsToPerform[i]);
            }
        }

        foreach (GameObject enemy in BSM.enemyTeam)
        {
            if (enemy.GetComponent<BaseClass>().provokerGO == gameObject)
            {
                enemy.GetComponent<BaseClass>().activeStatusEffects.Remove(BaseClass.StatusEffect.Provoked);
            }
        }

        GetComponent<MeshRenderer>().material.color = Color.gray;
        //Here i could rotate the dead unit to make it seems more dead

        isAlive = false;

        yield return null;
    }
    public enum TurnState
    {
        TEAMSELECTION,
        ADDTOLIST,
        WAITING,
        ACTION,
        DEAD
    }
}
