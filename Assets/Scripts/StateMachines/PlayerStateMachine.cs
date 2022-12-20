using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    private BattleStateMachine BSM;
    private BaseClass myClass;
    private bool isAlive = true;

    public TurnState currentState;
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
        BSM.targetDied = true;

        for (int i = 0; i < BSM.actionsToPerform.Count; i++)
        {
            if (BSM.actionsToPerform[i].attackerGO == gameObject)
            {
                BSM.actionsToPerform.RemoveAt(i);
                break;
            }
        }

        for (int i = 0; i < BSM.actionsToPerform.Count; i++)  ///I need to check from the next action because the action[0] is still performing and self removing this target at this point
        {
            if (BSM.actionsToPerform[i].attackTargets.Contains(gameObject))
            {
                BSM.actionsToPerform[i].attackTargets.Remove(gameObject);
                BSM.SetNewTarget(BSM.actionsToPerform[i]);
            }
        }

        foreach (GameObject enemy in BSM.enemyTeam)//potrei doverlo mettere sopra al for sopra questo
        {
            if (enemy.GetComponent<BaseClass>().provokerGO == gameObject)
            {
                enemy.GetComponent<BaseClass>().activeStatusEffects.Remove(BaseClass.StatusEffect.Provoked);
            }
        }

        GetComponent<MeshRenderer>().material.color = Color.gray;
        //potrei ruotarlo per farlo sembrare muerto

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
