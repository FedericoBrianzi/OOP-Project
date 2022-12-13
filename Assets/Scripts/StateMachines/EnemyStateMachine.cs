using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyStateMachine : MonoBehaviour
{
    private BattleStateMachine BSM;
    private BaseClass myClass;

    [SerializeField] private TurnState currentState;

    private void Awake()
    {
        GameManager.OnGameStateChanged += GameManagerOnGameStateChanged;
        currentState = TurnState.WAITING;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManagerOnGameStateChanged;
    }

    private void GameManagerOnGameStateChanged(GameManager.GameState state)
    {
        if(state == GameManager.GameState.BattleStart)
        {
            BSM = FindObjectOfType<BattleStateMachine>();
            myClass = GetComponent<BaseClass>();
            currentState = TurnState.CHOOSEACTION;
        }
    }

    private void Update()
    {
        switch (currentState)
        {
            case TurnState.CHOOSEACTION:
                ChooseAction();
                break;
            case TurnState.WAITING:
                break;
            case TurnState.ACTION:
                break;
            case TurnState.DEAD:
                break;
        }
    }

    private void ChooseAction()
    {
        HandleTurn myAttack = new HandleTurn();
        myAttack.attackerName = myClass.name;
        myAttack.attackerGO = this.gameObject;
        myAttack.attack = myClass.attacks[Random.Range(0, myClass.attacks.Count)];
        SelectTarget(myAttack, myAttack.attack);
    }

    private void SelectTarget(HandleTurn myAttack, BaseAttack attack)
    {
        switch (attack.numberOfTargets)
        {
            case BaseAttack.typeOfTarget.SingleEnemyTarget:
                myAttack.attackTargets.Add(BSM.playerTeam[UnityEngine.Random.Range(0, BSM.playerTeam.Count)]);
                break;

            case BaseAttack.typeOfTarget.MultiEnemyTargets:
                SetMultiEnemyTargets(myAttack);
                break;

            case BaseAttack.typeOfTarget.AllEnemyTargets:
                myAttack.attackTargets.AddRange(BSM.playerTeam);
                break;

            case BaseAttack.typeOfTarget.SingleAllyTarget:
                myAttack.attackTargets.Add(BSM.enemyTeam[UnityEngine.Random.Range(0, BSM.enemyTeam.Count)]);
                break;

            case BaseAttack.typeOfTarget.MultiAllyTargets:
                SetMultiAllyTargets(myAttack);
                break;

            case BaseAttack.typeOfTarget.AllAllyTargets:
                myAttack.attackTargets.AddRange(BSM.enemyTeam);
                break;

            case BaseAttack.typeOfTarget.Self:
                myAttack.attackTargets.Add(gameObject);
                break;
        }
        BSM.SetNewActionToPerform(myAttack);
        currentState = TurnState.WAITING;
    }

    private void SetMultiAllyTargets(HandleTurn myAttack)
    {
        if (UnityEngine.Random.Range(0, BSM.enemyTeam.Count) == 0)
        {
            myAttack.attackTargets.Add(BSM.enemyTeam[0]);
            if (BSM.enemyTeam.Count >= 2)
            {
                myAttack.attackTargets.Add(BSM.enemyTeam[1]);
            }
        }
        else if (UnityEngine.Random.Range(0, BSM.enemyTeam.Count) == BSM.enemyTeam.Count - 1)
        {
            myAttack.attackTargets.Add(BSM.enemyTeam[BSM.enemyTeam.Count - 1]);
            if (BSM.enemyTeam.Count >= 2)
            {
                myAttack.attackTargets.Add(BSM.enemyTeam[BSM.enemyTeam.Count - 2]);
            }
        }
        else
        {
            myAttack.attackTargets.Add(BSM.enemyTeam[0]);
            myAttack.attackTargets.Add(BSM.enemyTeam[1]);
            myAttack.attackTargets.Add(BSM.enemyTeam[2]);
        }
    }

    private void SetMultiEnemyTargets(HandleTurn myAttack)
    {
        if (UnityEngine.Random.Range(0, BSM.playerTeam.Count) == 0)
        {
            myAttack.attackTargets.Add(BSM.playerTeam[0]);
            if (BSM.playerTeam.Count >= 2)
            {
                myAttack.attackTargets.Add(BSM.playerTeam[1]);
            }
        }
        else if (UnityEngine.Random.Range(0, BSM.playerTeam.Count) == BSM.playerTeam.Count - 1)
        {
            myAttack.attackTargets.Add(BSM.enemyTeam[BSM.enemyTeam.Count - 1]);
            if (BSM.playerTeam.Count >= 2) //non servirebbe perche se ci fosse solo un nemico entreresti sempre nel primo if
            {
                myAttack.attackTargets.Add(BSM.enemyTeam[BSM.enemyTeam.Count - 2]);
            }
        }
        else
        {   //non ha bisogno di altre condizioni perche negli altri casi finiresti negli if precedenti
            myAttack.attackTargets.Add(BSM.playerTeam[0]);
            myAttack.attackTargets.Add(BSM.playerTeam[1]);
            myAttack.attackTargets.Add(BSM.playerTeam[2]);
        }
    }

    private enum TurnState
    {
        CHOOSEACTION,
        WAITING,
        ACTION,
        DEAD
    }
}
