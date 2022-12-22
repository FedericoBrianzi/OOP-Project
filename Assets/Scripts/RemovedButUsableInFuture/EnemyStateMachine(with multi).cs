using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyStateMachineMulti : MonoBehaviour
{
    private BattleStateMachine BSM;
    private BaseClass myClass;

    public TurnState currentState;
    private bool isAlive = true;

    public enum TurnState
    {
        TEAMSELECTION,
        CHOOSEACTION,
        WAITING,
        ACTION,
        DEAD
    }
    private void Awake()
    {
        GameManager.OnGameStateChanged += GameManagerOnGameStateChanged;
        currentState = TurnState.TEAMSELECTION;
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
            case TurnState.TEAMSELECTION:
                break;
            case TurnState.CHOOSEACTION:
                ChooseAction();
                break;
            case TurnState.WAITING:
                if(BSM.actionsToPerform.Count == 0 && BSM.battleState == BattleStateMachine.BattleState.WAIT)
                {
                    currentState = TurnState.CHOOSEACTION;
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
                    currentState = TurnState.WAITING;
                }
                break;
        }
    }

    private void ChooseAction()
    {
        HandleTurn myAction = new HandleTurn();
        myAction.attackerName = myClass.unitName;
        myAction.attackerGO = this.gameObject;
        if (myClass.activeStatusEffects.Contains(BaseClass.StatusEffect.Provoked))
        {
            myAction.attack = SelectAttackWhenProvoked();
        }
        else myAction.attack = CheckManaCost(myClass.attacks)[Random.Range(0, CheckManaCost(myClass.attacks).Count)];   ///CheckManaCost returns a list of usable attacks
        SelectTarget(myAction, myAction.attack); //non sto selezionando il provocatore qua perchè teoricamente se è provocato lo selezionerà successivamente
                                                 //tuttavia se il provoke finisce attacca chi ha selezionato qui(e non è così che dovrebbe funzionare)
        BSM.SetNewActionToPerform(myAction);
        currentState = TurnState.WAITING;
    }

    private BaseAttack SelectAttackWhenProvoked()
    {
        List<BaseAttack> attacksForEnemies = new List<BaseAttack>();
        foreach (BaseAttack attack in myClass.attacks)
        {
            if(attack.numberOfTargets == BaseAttack.typeOfTarget.SingleEnemyTarget || /*attack.numberOfTargets == BaseAttack.typeOfTarget.MultiEnemyTargets ||*/ attack.numberOfTargets == BaseAttack.typeOfTarget.AllEnemyTargets)
            {
                attacksForEnemies.Add(attack);
            }
        }
        return CheckManaCost(attacksForEnemies)[Random.Range(0, CheckManaCost(attacksForEnemies).Count)];
    }

    private List<BaseAttack> CheckManaCost(List<BaseAttack> attacksToCheck)
    {
        List<BaseAttack> usableAttacks = new List<BaseAttack>();
        foreach(BaseAttack attack in attacksToCheck)
        {
            if (attack.attackManaCost <= myClass.GetCurrentMana()) usableAttacks.Add(attack);
        }
        return usableAttacks;
    }

    public void SelectTarget(HandleTurn myAction, BaseAttack attack)
    {
        switch (attack.numberOfTargets)
        {
            case BaseAttack.typeOfTarget.SingleEnemyTarget:
                myAction.attackTargets.Add(BSM.playerTeam[Random.Range(0, BSM.playerTeam.Count)]);
                break;

            //case BaseAttack.typeOfTarget.MultiEnemyTargets:
            //    SetMultiEnemyTargets(myAction);
            //    break;

            case BaseAttack.typeOfTarget.AllEnemyTargets:
                myAction.attackTargets.AddRange(BSM.playerTeam);
                break;

            case BaseAttack.typeOfTarget.SingleAllyTarget:
                myAction.attackTargets.Add(BSM.enemyTeam[Random.Range(0, BSM.enemyTeam.Count)]);
                break;

            //case BaseAttack.typeOfTarget.MultiAllyTargets:
            //    SetMultiAllyTargets(myAction);
            //    break;

            case BaseAttack.typeOfTarget.AllAllyTargets:
                myAction.attackTargets.AddRange(BSM.enemyTeam);
                break;

            case BaseAttack.typeOfTarget.Self:
                myAction.attackTargets.Add(gameObject);
                break;
        }
    }

    private void SetMultiAllyTargets(HandleTurn myAttack)
    {
        if (Random.Range(0, BSM.enemyTeam.Count) == 0)
        {
            myAttack.attackTargets.Add(BSM.enemyTeam[0]);
            if (BSM.enemyTeam.Count >= 2)
            {
                myAttack.attackTargets.Add(BSM.enemyTeam[1]);
            }
        }
        else if (Random.Range(0, BSM.enemyTeam.Count) == BSM.enemyTeam.Count - 1)
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
        if (Random.Range(0, BSM.playerTeam.Count) == 0)
        {
            myAttack.attackTargets.Add(BSM.playerTeam[0]);
            if (BSM.playerTeam.Count >= 2)
            {
                myAttack.attackTargets.Add(BSM.playerTeam[1]);
            }
        }
        else if (Random.Range(0, BSM.playerTeam.Count) == BSM.playerTeam.Count - 1)
        {
            myAttack.attackTargets.Add(BSM.playerTeam[BSM.playerTeam.Count - 1]);
            if (BSM.playerTeam.Count >= 2) //non servirebbe perche se ci fosse solo un nemico entreresti sempre nel primo if
            {
                myAttack.attackTargets.Add(BSM.playerTeam[BSM.playerTeam.Count - 2]);
            }
        }
        else
        {   //non ha bisogno di altre condizioni perche negli altri casi finiresti negli if precedenti
            myAttack.attackTargets.Add(BSM.playerTeam[0]);
            myAttack.attackTargets.Add(BSM.playerTeam[1]);
            myAttack.attackTargets.Add(BSM.playerTeam[2]);
        }
    }

    public IEnumerator DeadEnemyRoutine()
    {
        gameObject.tag = "DeadEnemy";

        BSM.enemyTeam.Remove(gameObject);
        BSM.SetTargetAliveBool(false);

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
                BSM.actionsToPerform[i].attackTargets.Remove(gameObject); //negli attacchi allTarget mi rimuove tutti i target
                BSM.SetNewTarget(BSM.actionsToPerform[i]);
            }
        }

        foreach (GameObject playerUnit in BSM.playerTeam)//potrei doverlo mettere sopra al for sopra questo
        {
            if (playerUnit.GetComponent<BaseClass>().provokerGO == gameObject)
            {
                playerUnit.GetComponent<BaseClass>().activeStatusEffects.Remove(BaseClass.StatusEffect.Provoked);
            }
        }

        GetComponent<MeshRenderer>().material.color = Color.gray;
        //potrei ruotarlo per farlo sembrare muerto

        isAlive = false;

        yield return null;
    }
}
