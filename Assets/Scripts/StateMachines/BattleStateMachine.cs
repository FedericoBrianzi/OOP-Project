using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleStateMachine : MonoBehaviour
{
    public List<GameObject> playerTeam { get; private set; } = new List<GameObject>();
    public List<GameObject> enemyTeam { get; private set; } = new List<GameObject>();

    [SerializeField] private List<HandleTurn> actionsToPerform = new List<HandleTurn>();

    private List<GameObject> heroesToManage = new List<GameObject>();
    private List<GameObject> targetsToManage = new List<GameObject>();
    private HandleTurn heroAction;

    [SerializeField] private GameObject actionPanel;
    [SerializeField] private GameObject selectTargetPanel;
    [SerializeField] private GameObject actionDescriptionPanel;

    private HandleUI uiHandler;

    private bool isTargetSelected = false;
    private enum BattleState
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

    private BattleState battleState;
    private HeroStates heroInput;
    private bool isAttacking = false;

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
                //just waiting for the battle to start
                break;

            case BattleState.WAIT: //waiting for every unit on the field to choose an action
                if (actionsToPerform.Count == playerTeam.Count + enemyTeam.Count) 
                {
                    battleState = BattleState.SORTACTIONS;
                }
                break;

            case BattleState.SORTACTIONS:
                actionsToPerform.Sort();    //implemented an iComparable interface to sort the list of HandleTurns by the currentSpeed of the performer of the action
                actionsToPerform.Reverse();
                for(int i = 0; i < actionsToPerform.Count; i++)
                {
                    if(actionsToPerform[i].attack.attackType == BaseAttack.typeOfAttack.Defend) //Defend actions are always first
                    {
                        actionsToPerform.Insert(0, actionsToPerform[i]);
                        actionsToPerform.RemoveAt(i + 1);
                    }
                }
                battleState = BattleState.PERFORMACTION;
                break;

            case BattleState.PERFORMACTION: //performing every action
                if (actionsToPerform.Count > 0 && !isAttacking)
                {
                    StartCoroutine(PerformAction());
                }
                else if (actionsToPerform.Count > 0 && isAttacking) break;
                else battleState = BattleState.WAIT;
                break;

        }

        switch (heroInput)
        {
            case HeroStates.TEAMSELECTION:
                //waiting for battle to start
                break;
            case HeroStates.ACTIVATE:
                if(heroesToManage.Count > 0)
                {
                    uiHandler.ActivateActionPanel(heroesToManage[0]);
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

    IEnumerator PerformAction()
    {
        isAttacking = true;
        actionDescriptionPanel.SetActive(true);     /////da spostare nel uihandler
        TextMeshProUGUI actionText = actionDescriptionPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        if(actionsToPerform[0].attackTargets.Count == 1)
        {
            actionText.text = actionsToPerform[0].attackerName + " uses " + actionsToPerform[0].attack.attackName + " on " + actionsToPerform[0].attackTargets[0].GetComponent<BaseClass>().name;
        }
        else if(actionsToPerform[0].attackTargets.Count == enemyTeam.Count)
        {
            actionText.text = actionsToPerform[0].attackerName + " uses " + actionsToPerform[0].attack.attackName + " on the other team";
        } //////
        
        else actionText.text = actionsToPerform[0].attackerName + " uses " + actionsToPerform[0].attack.attackName + " on " + actionsToPerform[0].attackTargets[0].GetComponent<BaseClass>().name + " amd " + actionsToPerform[0].attackTargets[1].GetComponent<BaseClass>().name;
        actionsToPerform[0].attackerGO.GetComponent<BaseClass>().SubtractMana(actionsToPerform[0].attack.GetComponent<BaseAttack>().attackManaCost);
        int damageTotal = CalculateDamage(actionsToPerform[0].attack.attackDamage, actionsToPerform[0].attackerGO.GetComponent<BaseClass>().currentAttack);
        actionsToPerform[0].attackTargets[0].GetComponent<BaseClass>().EvaluateAttack(actionsToPerform[0].attack, damageTotal);
        actionsToPerform.RemoveAt(0);
        yield return new WaitForSeconds(2f);
        uiHandler.UpdateBattleUI();
        actionDescriptionPanel.SetActive(false);
        isAttacking = false;
    }

    public void SetNewActionToPerform(HandleTurn action)
    {
        actionsToPerform.Add(action);
    }

    public int CalculateDamage(int atkDmg, int unitAtk)
    {
        return atkDmg + unitAtk; //da tweakare
    }

    public void AddHeroToList(GameObject pUnit)
    {
        heroesToManage.Add(pUnit);
    }

    public void AttackInput(BaseAttack chosenAttack)
    {
        isTargetSelected = false;
        targetsToManage.Clear();

        heroAction.attackerName = heroesToManage[0].GetComponent<BaseClass>().name;
        heroAction.attackerGO = heroesToManage[0].gameObject;
        heroAction.attack = chosenAttack;
        SelectTarget(heroAction, heroAction.attack);

        uiHandler.DeactivateActionPanel();
        if (!isTargetSelected)
        {
            uiHandler.ActivateTargetPanel(targetsToManage.ToArray());
        }
    }

    public void TargetInput(GameObject target)
    {
        uiHandler.DeactivateTargetPanel();
        heroAction.attackTargets.Add(target);
        actionsToPerform.Add(heroAction);
        heroesToManage.RemoveAt(0);
        heroInput = HeroStates.ACTIVATE;
    }

    public void MultiTargetInput(GameObject[] targets)
    {
        uiHandler.DeactivateTargetPanel();
        heroAction.attackTargets.AddRange(targets);
        actionsToPerform.Add(heroAction);
        heroesToManage.RemoveAt(0);
        heroInput = HeroStates.ACTIVATE;
    }

    public void SelectTarget(HandleTurn heroAction, BaseAttack attack)
    {
        switch (attack.numberOfTargets)
        {
            case BaseAttack.typeOfTarget.SingleEnemyTarget:
                targetsToManage.AddRange(enemyTeam);
                break;

            case BaseAttack.typeOfTarget.MultiEnemyTargets:
                isTargetSelected = true;
                targetsToManage.AddRange(enemyTeam);
                uiHandler.ActivateMultiTargetPanel(targetsToManage.ToArray());
                break;

            case BaseAttack.typeOfTarget.AllEnemyTargets:
                isTargetSelected = true;
                heroAction.attackTargets.AddRange(enemyTeam);
                actionsToPerform.Add(heroAction);
                heroesToManage.RemoveAt(0);
                heroInput = HeroStates.ACTIVATE;
                break;

            case BaseAttack.typeOfTarget.SingleAllyTarget:
                targetsToManage.AddRange(playerTeam);
                break;

            case BaseAttack.typeOfTarget.MultiAllyTargets:
                isTargetSelected = true;
                targetsToManage.AddRange(playerTeam);
                uiHandler.ActivateMultiTargetPanel(targetsToManage.ToArray());
                break;

            case BaseAttack.typeOfTarget.AllAllyTargets:
                isTargetSelected = true;
                heroAction.attackTargets.AddRange(playerTeam);
                actionsToPerform.Add(heroAction);
                heroesToManage.RemoveAt(0);
                heroInput = HeroStates.ACTIVATE;
                break;

            case BaseAttack.typeOfTarget.Self:
                isTargetSelected = true;
                heroAction.attackTargets.Add(heroesToManage[0]);
                actionsToPerform.Add(heroAction);
                heroesToManage.RemoveAt(0);
                heroInput = HeroStates.ACTIVATE;
                break;
        }
    }
}
