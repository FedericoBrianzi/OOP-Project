using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStateMachine : MonoBehaviour
{
    public List<GameObject> playerTeam { get; private set; } = new List<GameObject>();
    public List<GameObject> enemyTeam { get; private set; } = new List<GameObject>();

    [SerializeField] private List<HandleTurn> actionsToPerform = new List<HandleTurn>();

    private enum BattleState
    {
        TEAMSELECTION,
        WAIT,
        SORTACTIONS,
        PERFORMACTION
    }

    private BattleState battleState;

    private void Awake()
    {
        GameManager.OnGameStateChanged += GameManagerOnGameStateChanged;
    }

    private void GameManagerOnGameStateChanged(GameManager.GameState state)
    {
        if(state == GameManager.GameState.BattleStart)
        {
            battleState = BattleState.WAIT;
            playerTeam.AddRange(GameObject.FindGameObjectsWithTag("Player"));
            enemyTeam.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        }
    }

    private void Update()
    {
        switch (battleState)
        {
            case BattleState.TEAMSELECTION:
                break;

            case BattleState.WAIT:
                if(actionsToPerform.Count == /*playerTeam.Count +*/ enemyTeam.Count)
                {
                    battleState = BattleState.SORTACTIONS;
                }
                break;

            case BattleState.SORTACTIONS:
                actionsToPerform.Sort();
                actionsToPerform.Reverse();
                battleState = BattleState.PERFORMACTION;
                break;

            case BattleState.PERFORMACTION: // devo farlo per ogni oggetto
                actionsToPerform[0].attackerGO.GetComponent<BaseClass>().SubtractMana(actionsToPerform[0].attack.GetComponent<BaseAttack>().attackManaCost);
                actionsToPerform[0].attackTargets[0].GetComponent<BaseClass>().EvaluateAttack(actionsToPerform[0].attack);
                break;
        }
    }

    public void SetNewActionToPerform(HandleTurn action)
    {
        actionsToPerform.Add(action);
    }
}
