using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

public class BattleStateMachineMulti : MonoBehaviour
{
    public List<GameObject> playerTeam { get; private set; } = new List<GameObject>();
    public List<GameObject> enemyTeam { get; private set; } = new List<GameObject>();

    public List<HandleTurn> actionsToPerform = new List<HandleTurn>();

    private List<GameObject> heroesToManage = new List<GameObject>();
    private List<GameObject> targetsToManage = new List<GameObject>();
    private HandleTurn heroAction;
    private BaseClass attackerClass;
    private List<BaseClass> unitsClasses = new List<BaseClass>();

    private HandleUI uiHandler;

    private bool isTargetSelected = false;
    private bool isAttacking = false;
    public bool targetDied = false;
    public enum BattleState
    {
        TEAMSELECTION,
        WAIT,
        SORTACTIONS,
        PERFORMACTION
    }

    private enum HeroStates
    {
        TEAMSELECTION,
        ACTIVATE,
        WAITING,
        DONE
    }

    public BattleState battleState { get; private set; }
    private HeroStates heroInput;

    private void Awake()
    {
        GameManager.OnGameStateChanged += GameManagerOnGameStateChanged;
        uiHandler = GameObject.Find("Canvas").GetComponent<HandleUI>();
        battleState = BattleState.TEAMSELECTION;
        heroInput = HeroStates.TEAMSELECTION;
    }

    private void GameManagerOnGameStateChanged(GameManager.GameState state)
    {
        if(state == GameManager.GameState.BattleStart)
        {
            battleState = BattleState.WAIT;
            heroInput = HeroStates.ACTIVATE;
            playerTeam.AddRange(GameObject.FindGameObjectsWithTag("Player"));
            enemyTeam.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        }
    }

    private void Update()
    {
        switch (battleState)
        {
            case BattleState.TEAMSELECTION:
                ///just waiting for the battle to start
                break;

            case BattleState.WAIT: ///waiting for every unit on the field to choose an action
                if (actionsToPerform.Count == playerTeam.Count + enemyTeam.Count) 
                {
                    battleState = BattleState.SORTACTIONS;
                }
                break;

            case BattleState.SORTACTIONS:
                actionsToPerform.Sort();    ///implemented an iComparable interface to sort the list of HandleTurns by the currentSpeed of the performer of the action
                actionsToPerform.Reverse();
                for(int i = 0; i < actionsToPerform.Count; i++)
                {
                    if(actionsToPerform[i].attack.attackType == BaseAttack.typeOfAttack.Defend) ///Defend actions are always first
                    {
                        actionsToPerform.Insert(0, actionsToPerform[i]);
                        actionsToPerform.RemoveAt(i + 1);
                    }
                }
                battleState = BattleState.PERFORMACTION;
                break;

            case BattleState.PERFORMACTION: ///performing every action
                if (actionsToPerform.Count > 0 && !isAttacking)
                {
                    StartCoroutine(PerformAction());
                }
                else if (actionsToPerform.Count > 0 && isAttacking) return;
                else if (actionsToPerform.Count == 0 && !isAttacking)
                {
                    uiHandler.HideActionDescription();
                    battleState = BattleState.WAIT;
                }
                break;
        }

        switch (heroInput)
        {
            case HeroStates.TEAMSELECTION:
                ///waiting for battle to start
                break;
            case HeroStates.ACTIVATE:
                if(heroesToManage.Count > 0)
                {
                    uiHandler.ActivateActionPanel(heroesToManage[0]);
                    heroesToManage[0].GetComponent<BaseClass>().indicator.SetActive(true);
                    heroAction = new HandleTurn();
                    heroInput = HeroStates.WAITING;
                }
                break;
            case HeroStates.WAITING:
                break;
            case HeroStates.DONE:
                break;
        }
    }

    #region Player HandleTurn Methods
    public void AddHeroToList(GameObject pUnit)
    {
        heroesToManage.Add(pUnit);
    }

    public void AttackInput(BaseAttack chosenAttack)
    {
        isTargetSelected = false;
        targetsToManage.Clear();

        heroAction.attackerName = heroesToManage[0].GetComponent<BaseClass>().unitName;
        heroAction.attackerGO = heroesToManage[0].gameObject;
        heroAction.attack = chosenAttack;
        SelectTarget(heroAction, heroAction.attack);

        uiHandler.DeactivateActionPanel();
        if (!isTargetSelected)
        {
            uiHandler.ActivateTargetPanel(targetsToManage.ToArray(), heroAction.attack);
        }
    }

    public void TargetInput(GameObject target)      //TargetInput e MultiTargetInput possono forse essere condensati in un solo metodo se entrambi prendono come parametro
    {                                               //una lista di GameObject e all'aggiunta dei target c'è un check sul numero di oggetti nella lista
        uiHandler.DeactivateTargetPanel();          //TargetInput prende solo l'elemento 0 e MultiTargetInput li prende tutti.
        heroAction.attackTargets.Add(target);       //Così facendo posso richiamare quest'unica funzione anche da SelectTarget nei casi AllEnemy/AllAlly/Self.
        heroesToManage[0].GetComponent<BaseClass>().indicator.SetActive(false);
        actionsToPerform.Add(heroAction);
        heroesToManage.RemoveAt(0);
        heroInput = HeroStates.ACTIVATE;
    }

    public void MultiTargetInput(GameObject[] targets)
    {
        uiHandler.DeactivateTargetPanel();
        heroAction.attackTargets.AddRange(targets);
        heroesToManage[0].GetComponent<BaseClass>().indicator.SetActive(false);
        actionsToPerform.Add(heroAction);
        heroesToManage.RemoveAt(0);
        heroInput = HeroStates.ACTIVATE;
    }

    private void SelectTarget(HandleTurn heroAction, BaseAttack attack)
    {
        switch (attack.numberOfTargets)
        {
            case BaseAttack.typeOfTarget.SingleEnemyTarget:
                targetsToManage.AddRange(enemyTeam);
                break;

            //case BaseAttack.typeOfTarget.MultiEnemyTargets:
            //    isTargetSelected = true;
            //    targetsToManage.AddRange(enemyTeam);
            //    uiHandler.ActivateMultiTargetPanel(targetsToManage.ToArray(), heroAction.attack);
            //    break;

            case BaseAttack.typeOfTarget.AllEnemyTargets:
                isTargetSelected = true;
                heroAction.attackTargets.AddRange(enemyTeam);
                heroesToManage[0].GetComponent<BaseClass>().indicator.SetActive(false);
                actionsToPerform.Add(heroAction);
                heroesToManage.RemoveAt(0);
                heroInput = HeroStates.ACTIVATE;
                break;

            case BaseAttack.typeOfTarget.SingleAllyTarget:
                targetsToManage.AddRange(playerTeam);
                break;

            //case BaseAttack.typeOfTarget.MultiAllyTargets:
            //    isTargetSelected = true;
            //    targetsToManage.AddRange(playerTeam);
            //    uiHandler.ActivateMultiTargetPanel(targetsToManage.ToArray(), heroAction.attack);
            //    break;

            case BaseAttack.typeOfTarget.AllAllyTargets:
                isTargetSelected = true;
                heroAction.attackTargets.AddRange(playerTeam);
                heroesToManage[0].GetComponent<BaseClass>().indicator.SetActive(false);
                actionsToPerform.Add(heroAction);
                heroesToManage.RemoveAt(0);
                heroInput = HeroStates.ACTIVATE;
                break;

            case BaseAttack.typeOfTarget.Self:
                isTargetSelected = true;
                heroAction.attackTargets.Add(heroesToManage[0]);
                heroesToManage[0].GetComponent<BaseClass>().indicator.SetActive(false);
                actionsToPerform.Add(heroAction);
                heroesToManage.RemoveAt(0);
                heroInput = HeroStates.ACTIVATE;
                break;
        }
    }
    #endregion

    #region Enemy HandleTurn Methods
    public void SetNewActionToPerform(HandleTurn action)
    {
        actionsToPerform.Add(action);
    }
    #endregion

    #region PerformAction Methods
    IEnumerator PerformAction()
    {
        attackerClass = actionsToPerform[0].attackerGO.GetComponent<BaseClass>();
        isAttacking = true;
        if (attackerClass.activeStatusEffects.Count == 0)
        {
            yield return StartCoroutine(uiHandler.ShowActionDescription(actionsToPerform[0]));
            yield return StartCoroutine(PerformAttack());
            actionsToPerform.RemoveAt(0);
        }
        else yield return StartCoroutine(EvaluateStatusEffectBeforeAttack());
        uiHandler.UpdateBattleUI();
        if (actionsToPerform.Count == 0)
        {
            yield return StartCoroutine(EvaluateStatusEffectAfterAttack());
        }
        isAttacking = false;
    }

    private IEnumerator PerformAttack()
    {
        attackerClass.SubtractMana(actionsToPerform[0].attack.attackManaCost);
        int damageTotal = CalculateDamage(actionsToPerform[0].attack.attackDamage, attackerClass.currentAttack);
        while(actionsToPerform[0].attackTargets.Count != 0)
        {
            yield return StartCoroutine(actionsToPerform[0].attackTargets[0].GetComponent<BaseClass>().EvaluateAttack(actionsToPerform[0].attack, damageTotal, actionsToPerform[0].attackerGO));
            if (!targetDied)
            {
                actionsToPerform[0].attackTargets.RemoveAt(0);
            }
            else targetDied = true;
        }
    }

    private IEnumerator EvaluateStatusEffectBeforeAttack()
    {
        OrderStatusEffects();
        List<BaseClass.StatusEffect> unitStatusEffects = new List<BaseClass.StatusEffect>();
        unitStatusEffects.AddRange(attackerClass.activeStatusEffects);
        foreach (BaseClass.StatusEffect activeStatus in unitStatusEffects)
        {
            switch (activeStatus)
            {
                case BaseClass.StatusEffect.Confused:
                    if (Random.Range(0, 100) < 33)  ///chance to remove status
                    {
                        attackerClass.activeStatusEffects.Remove(BaseClass.StatusEffect.Confused);
                        yield return StartCoroutine(uiHandler.ShowStatusRemovedDescription(BaseClass.StatusEffect.Confused));
                    }
                    else
                    {
                        if (Random.Range(0, 100) < 33)  ///chance to attack randomly
                        {
                            actionsToPerform[0].attack = attackerClass.confusedAttack;
                            actionsToPerform[0].attackTargets.Clear();
                            if (Random.Range(0, 2) == 0)
                            {
                                actionsToPerform[0].attackTargets.Add(playerTeam[Random.Range(0, playerTeam.Count)]);
                            }
                            else
                            {
                                actionsToPerform[0].attackTargets.Add(enemyTeam[Random.Range(0, enemyTeam.Count)]);
                            }
                            yield return StartCoroutine(uiHandler.ShowStatusEffectActionDescription(BaseClass.StatusEffect.Confused, true));
                        }
                        else
                        {
                            yield return StartCoroutine(uiHandler.ShowStatusEffectActionDescription(BaseClass.StatusEffect.Confused, true));
                        }
                    } 
                    break;
                case BaseClass.StatusEffect.Paralyzed:
                    if (Random.Range(0, 100) < 33)  ///chance to remove status
                    {
                        attackerClass.activeStatusEffects.Remove(BaseClass.StatusEffect.Paralyzed);
                        yield return StartCoroutine(uiHandler.ShowStatusRemovedDescription(BaseClass.StatusEffect.Paralyzed));
                    }
                    else
                    {
                        if (Random.Range(0, 100) < 50)  ///chance to not perform the action
                        {
                            yield return StartCoroutine(uiHandler.ShowStatusEffectActionDescription(BaseClass.StatusEffect.Paralyzed, false));
                            actionsToPerform.RemoveAt(0);
                            yield break;
                        }
                        else yield return StartCoroutine(uiHandler.ShowStatusEffectActionDescription(BaseClass.StatusEffect.Paralyzed, true));
                    }
                    break;
                case BaseClass.StatusEffect.Provoked:
                    if (Random.Range(0, 100) < 33)  ///chance to remove status
                    {
                        attackerClass.activeStatusEffects.Remove(BaseClass.StatusEffect.Provoked);
                        yield return StartCoroutine(uiHandler.ShowStatusRemovedDescription(BaseClass.StatusEffect.Provoked));
                    }
                    else
                    {
                        actionsToPerform[0].attackTargets.Clear();
                        actionsToPerform[0].attackTargets.Add(attackerClass.provokerGO);    ///the provoke status effect makes the multi target attacks single target
                        yield return StartCoroutine(uiHandler.ShowStatusEffectActionDescription(BaseClass.StatusEffect.Provoked, true));
                    }
                    break;
                case BaseClass.StatusEffect.Stunned:
                    yield return StartCoroutine(uiHandler.ShowStatusEffectActionDescription(BaseClass.StatusEffect.Stunned, false));    ///stunned attacker cant perform the attack in this turn
                    yield return StartCoroutine(uiHandler.ShowStatusRemovedDescription(BaseClass.StatusEffect.Stunned));                ///and then the stunned status effect ends
                    attackerClass.activeStatusEffects.Remove(BaseClass.StatusEffect.Stunned);
                    actionsToPerform.RemoveAt(0);
                    yield break;
                case BaseClass.StatusEffect.Asleep:
                    if (Random.Range(0, 100) < 33)  ///chance to wake up
                    {
                        attackerClass.activeStatusEffects.Remove(BaseClass.StatusEffect.Asleep);
                        yield return StartCoroutine(uiHandler.ShowStatusRemovedDescription(BaseClass.StatusEffect.Asleep));
                    }
                    else
                    {
                        yield return StartCoroutine(uiHandler.ShowStatusEffectActionDescription(BaseClass.StatusEffect.Asleep, false));
                        actionsToPerform.RemoveAt(0);
                        yield break;
                    }
                    break;
            }
        }
        yield return StartCoroutine(uiHandler.ShowActionDescription(actionsToPerform[0]));
        //qui andrebbe una coroutine per far muovere l'attaccante verso il target
        yield return StartCoroutine(PerformAttack()); ///once all status effects have been evaluated if it did not stop the attack, it performs.
        actionsToPerform.RemoveAt(0);
    }

    private void OrderStatusEffects() 
    {
        if (attackerClass.activeStatusEffects.Contains(BaseClass.StatusEffect.Poisoned))
        {
            attackerClass.activeStatusEffects.Remove(BaseClass.StatusEffect.Poisoned);
            attackerClass.activeStatusEffects.Insert(0, BaseClass.StatusEffect.Poisoned);
        }
        if (attackerClass.activeStatusEffects.Contains(BaseClass.StatusEffect.Burned))
        {
            attackerClass.activeStatusEffects.Remove(BaseClass.StatusEffect.Burned);
            attackerClass.activeStatusEffects.Insert(0, BaseClass.StatusEffect.Burned);
        }
        if (attackerClass.activeStatusEffects.Contains(BaseClass.StatusEffect.Paralyzed))
        {
            attackerClass.activeStatusEffects.Remove(BaseClass.StatusEffect.Paralyzed);
            attackerClass.activeStatusEffects.Insert(0, BaseClass.StatusEffect.Paralyzed);
        }
        if (attackerClass.activeStatusEffects.Contains(BaseClass.StatusEffect.Confused))
        {
            attackerClass.activeStatusEffects.Remove(BaseClass.StatusEffect.Confused);
            attackerClass.activeStatusEffects.Insert(0, BaseClass.StatusEffect.Confused);
        }
        if (attackerClass.activeStatusEffects.Contains(BaseClass.StatusEffect.Provoked))
        {
            attackerClass.activeStatusEffects.Remove(BaseClass.StatusEffect.Provoked);
            attackerClass.activeStatusEffects.Insert(0, BaseClass.StatusEffect.Provoked);
        }
        if (attackerClass.activeStatusEffects.Contains(BaseClass.StatusEffect.Asleep))
        {
            attackerClass.activeStatusEffects.Remove(BaseClass.StatusEffect.Asleep);
            attackerClass.activeStatusEffects.Insert(0, BaseClass.StatusEffect.Asleep);
        }
        if (attackerClass.activeStatusEffects.Contains(BaseClass.StatusEffect.Stunned))
        {
            attackerClass.activeStatusEffects.Remove(BaseClass.StatusEffect.Stunned);
            attackerClass.activeStatusEffects.Insert(0, BaseClass.StatusEffect.Stunned);
        }
    } ///this sorts the status effect so that they get evaluated in the order i want

    private IEnumerator EvaluateStatusEffectAfterAttack()
    {
        unitsClasses.Clear();
        foreach (GameObject unit in playerTeam)
        {
            unitsClasses.Add(unit.GetComponent<BaseClass>());
        }
        foreach (GameObject unit in enemyTeam)
        {
            unitsClasses.Add(unit.GetComponent<BaseClass>());
        }
        foreach(BaseClass unitClass in unitsClasses)
        {
            foreach(BaseClass.StatusEffect activeStatus in unitClass.activeStatusEffects)
            {
                switch (activeStatus)
                {
                    ///I must explicit the type of status effect because in the foreach loop it finds also the other status effects like Asleep, Provoke etc.
                    case BaseClass.StatusEffect.Burned:
                        yield return StartCoroutine(unitClass.TakeStatusEffectDamage(activeStatus));
                        break;
                    case BaseClass.StatusEffect.Poisoned:
                        yield return StartCoroutine(unitClass.TakeStatusEffectDamage(activeStatus));
                        break;
                }
            } 
        }
    }

    private int CalculateDamage(int atkDmg, int unitAtk)
    {
        return atkDmg + unitAtk; //da tweakare(?)
    }

    public void SetNewTarget(HandleTurn action)
    {
        if (action.attackerGO.CompareTag("Player"))
        {
            switch (action.attack.numberOfTargets)
            {
                case BaseAttack.typeOfTarget.SingleAllyTarget:
                    action.attackTargets.Add(action.attackerGO);
                    break;
                case BaseAttack.typeOfTarget.SingleEnemyTarget:
                    action.attackTargets.Add(enemyTeam[Random.Range(0, enemyTeam.Count)]);
                    break;
                //case BaseAttack.typeOfTarget.MultiAllyTargets:
                //    if(playerTeam.Count == 2)
                //    {

                //    }
                //    break;
                case BaseAttack.typeOfTarget.AllAllyTargets:
                    ///I already removed the dead ally in the PlayerStateMachine
                    break;
            }
        }
        else action.attackerGO.GetComponent<EnemyStateMachine>().SelectTarget(action, action.attack);
        
    }
    #endregion
}
