using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HandleUIMulti : MonoBehaviour
{
    [Header("Team Selection Phase")]
    [SerializeField] private GameObject teamsSelectPanel;
    [SerializeField] private GameObject battlePanel;

    [Header("Battle Phase")]
    [SerializeField] private GameObject[] teamsStatsUI = new GameObject[6];

    [SerializeField] private GameObject actionPanel;
    [SerializeField] private GameObject targetPanel;
    [SerializeField] private GameObject unitNameText;

    [Header("Attack buttons")]
    [SerializeField] private List<GameObject> attackButtons = new List<GameObject>();
    [SerializeField] private GameObject attackButtonsHolder;
    [SerializeField] private GameObject attackButtonPrefab;

    [Header("Target buttons")]
    [SerializeField] private List<GameObject> targetButtons = new List<GameObject>();
    [SerializeField] private GameObject targetButtonsHolder;
    [SerializeField] private GameObject targetButtonPrefab;

    [Header("Action Description")]
    [SerializeField] private GameObject actionDescriptionPanel;
    private TextMeshProUGUI actionText;

    private BattleStateMachine BSM;
    private List<GameObject> unitsOnField = new List<GameObject>();

    private BaseClass heroClass;


    private void Awake()
    {
        GameManager.OnGameStateChanged += GameManagerOnGameStateChanged;
        BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
        actionText = actionDescriptionPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    private void GameManagerOnGameStateChanged(GameManager.GameState state)
    {
        teamsSelectPanel.SetActive(state == GameManager.GameState.PickTeams);

        battlePanel.SetActive(state == GameManager.GameState.BattleStart);
        if (state == GameManager.GameState.BattleStart)
        {
            unitsOnField.AddRange(BSM.playerTeam);
            unitsOnField.AddRange(BSM.enemyTeam);
            UpdateBattleUI();
        }
    }

    public void UpdateBattleUI()    ///shows current stats for every unit
    {
        int i = 0;
        foreach(GameObject unitUI in teamsStatsUI)
        {
            unitUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = unitsOnField[i].GetComponent<BaseClass>().unitName; //da problemi quando una unita verra distrutta
            unitUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "HP: " + unitsOnField[i].GetComponent<BaseClass>().GetCurrentHealth() + " / " + unitsOnField[i].GetComponent<BaseClass>().maxHealth;
            unitUI.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "MP: " + unitsOnField[i].GetComponent<BaseClass>().GetCurrentMana() + " / " + unitsOnField[i].GetComponent<BaseClass>().maxMana;
            i++;
        }
    }

    #region PlayerUI
    public void ActivateActionPanel(GameObject hero)  ///shows the player's current unit name and creates the attack buttons for that unit
    {
        actionPanel.SetActive(true);
        attackButtons.Clear();
        heroClass = hero.GetComponent<BaseClass>();

        unitNameText.GetComponent<TextMeshProUGUI>().text = heroClass.unitName;

        foreach (BaseAttack attack in heroClass.attacks)
        {
            GameObject atkButton = Instantiate(attackButtonPrefab);
            atkButton.transform.SetParent(attackButtonsHolder.transform, false);
            attackButtons.Add(atkButton);

            atkButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = attack.attackName;
            atkButton.GetComponent<AttackButton>().buttonAttack = attack;
            atkButton.GetComponent<Button>().onClick.AddListener(() => BSM.AttackInput(attack));
            if (heroClass.activeStatusEffects.Contains(BaseClass.StatusEffect.Provoked))
            {
                if (attack.numberOfTargets == BaseAttack.typeOfTarget.SingleEnemyTarget || /*attack.numberOfTargets == BaseAttack.typeOfTarget.MultiEnemyTargets ||*/ attack.numberOfTargets == BaseAttack.typeOfTarget.AllEnemyTargets) { }
                else atkButton.GetComponent<Button>().interactable = false;
            }
            if(attack.attackManaCost > heroClass.GetCurrentMana())
            {
                atkButton.GetComponent<Button>().interactable = false;
            }
        }
    }

    public void DeactivateActionPanel()
    {
        actionPanel.SetActive(false);
        foreach(GameObject button in attackButtons)
        {
            Destroy(button); //vorrei disattivarli e riatttivarli
        }
    }

    public void ActivateTargetPanel(GameObject[] targets, BaseAttack heroAttack)
    {
        targetPanel.SetActive(true);
        targetButtons.Clear();
        foreach (GameObject target in targets)
        {
            GameObject targetButton = Instantiate(targetButtonPrefab);
            targetButton.transform.SetParent(targetButtonsHolder.transform, false);
            targetButtons.Add(targetButton);

            targetButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = target.GetComponent<BaseClass>().unitName;
            targetButton.GetComponent<TargetButton>().buttonTargetClass = target.GetComponent<BaseClass>();
            targetButton.GetComponent<TargetButton>().setTargetType(heroAttack.numberOfTargets);
            targetButton.GetComponent<Button>().onClick.AddListener(() => BSM.TargetInput(target));
            if (heroClass.activeStatusEffects.Contains(BaseClass.StatusEffect.Provoked))
            {
                if(target != heroClass.provokerGO)
                {
                    targetButton.GetComponent<Button>().interactable = false;
                }
            }
        }
    }

    public void ActivateMultiTargetPanel(GameObject[] targets, BaseAttack heroAttack) 
    {
        targetPanel.SetActive(true);
        targetButtons.Clear();
        foreach (GameObject target in targets)
        {
            GameObject targetButton = Instantiate(targetButtonPrefab);
            targetButton.transform.SetParent(targetButtonsHolder.transform, false);
            targetButtons.Add(targetButton);

            targetButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = target.GetComponent<BaseClass>().unitName;
            targetButton.GetComponent<TargetButton>().buttonTargetClass = target.GetComponent<BaseClass>();
            targetButton.GetComponent<TargetButton>().setTargetType(heroAttack.numberOfTargets);
            if (heroClass.activeStatusEffects.Contains(BaseClass.StatusEffect.Provoked))
            {
                if (target != heroClass.provokerGO)
                {
                    targetButton.GetComponent<Button>().interactable = false;
                }
            }
        }

        if (targets.Length == 1)
        {
            targetButtons[0].GetComponent<Button>().onClick.AddListener(() => BSM.TargetInput(targets[0]));
        }
        else if (targets.Length == 3)
        {
            //targetButtons[0].GetComponent<Button>().onClick.AddListener(() => BSM.MultiTargetInput(SetMultiTargets(0).ToArray()));    //must be used with BSM with multi
            //targetButtons[1].GetComponent<Button>().onClick.AddListener(() => BSM.MultiTargetInput(SetMultiTargets(1).ToArray()));
            //targetButtons[2].GetComponent<Button>().onClick.AddListener(() => BSM.MultiTargetInput(SetMultiTargets(2).ToArray()));
        }
        else
        {
            foreach(GameObject targetButton in targetButtons)
            {
                //targetButton.GetComponent<Button>().onClick.AddListener(() => BSM.MultiTargetInput(targets));
            }
        }
    }

    private List<GameObject> SetMultiTargets(int buttonIndex)
    {
        List<GameObject> targets = new List<GameObject>();
        if (buttonIndex == 0)
        {
            targets.Add(targetButtons[0].GetComponent<TargetButton>().buttonTargetClass.gameObject);
            targets.Add(targetButtons[1].GetComponent<TargetButton>().buttonTargetClass.gameObject);
        }
        else if (buttonIndex == 1)
        {
            targets.Add(targetButtons[0].GetComponent<TargetButton>().buttonTargetClass.gameObject);
            targets.Add(targetButtons[1].GetComponent<TargetButton>().buttonTargetClass.gameObject);
            targets.Add(targetButtons[2].GetComponent<TargetButton>().buttonTargetClass.gameObject);
        }
        else
        {
            targets.Add(targetButtons[1].GetComponent<TargetButton>().buttonTargetClass.gameObject);
            targets.Add(targetButtons[2].GetComponent<TargetButton>().buttonTargetClass.gameObject);
        }
        return targets;
    }

    public void DeactivateTargetPanel()
    {
        targetPanel.SetActive(false);
        foreach (GameObject button in targetButtons)
        {
            Destroy(button); //vorrei disattivarli e riatttivarli
        }
    }
    #endregion

    #region PerformActionUI
    ///Action description
    public IEnumerator ShowActionDescription(HandleTurn action)
    {
        actionDescriptionPanel.SetActive(true);
        if (action.attackerGO.CompareTag("Enemy")) actionText.text = "Enemy ";  //non basta: enemy mage uses x on mage, mage uses x on (enemy non si legge) mage
        else actionText.text = "";

        if (action.attack.attackType == BaseAttack.typeOfAttack.Defend)
        {
            actionText.text += action.attackerName + " defends himself";
            yield return new WaitForSeconds(2);
        }
        else if (action.attackTargets.Count == 1 && action.attackTargets[0] == action.attackerGO)
        {
            actionText.text += action.attackerName + " uses " + action.attack.attackName + " on himself";
            yield return new WaitForSeconds(2);
        }
        else if (action.attackTargets.Count == 1)
        {
            actionText.text += action.attackerName + " uses " + action.attack.attackName + " on " + action.attackTargets[0].GetComponent<BaseClass>().unitName + ".";
            yield return new WaitForSeconds(2);
        }
        else if (action.attackTargets.Count == BSM.playerTeam.Count && action.attackTargets[0].gameObject.CompareTag("Player"))
        {
            actionText.text += action.attackerName + " uses " + action.attack.attackName + " on player's team.";
            yield return new WaitForSeconds(2);
        }
        else if (action.attackTargets.Count == BSM.enemyTeam.Count && action.attackTargets[0].gameObject.CompareTag("Enemy"))
        {
            actionText.text += action.attackerName + " uses " + action.attack.attackName + " on enemy team.";
            yield return new WaitForSeconds(2);
        }
    }

    ///Damage descriptions
    public IEnumerator ShowDamageTakenDescription(int damageTaken)
    {
        actionDescriptionPanel.SetActive(true);
        actionText.text = BSM.actionsToPerform[0].attackTargets[0].GetComponent<BaseClass>().unitName + " takes " + damageTaken + " damage.";
        UpdateBattleUI();
        yield return new WaitForSeconds(2);
    }

    public IEnumerator ShowDamageTakenDescription(bool dodged)
    {
        actionDescriptionPanel.SetActive(true);
        if(dodged) actionText.text = BSM.actionsToPerform[0].attackTargets[0].GetComponent<BaseClass>().unitName + " dodges the attack."; ///the bool check is not required
        UpdateBattleUI();
        yield return new WaitForSeconds(2);
    }   ///dodge

    public IEnumerator ShowStatusDamageDescription(BaseClass.StatusEffect status, int damage, string unitName)
    {
        actionDescriptionPanel.SetActive(true);
        switch (status)
        {
            case BaseClass.StatusEffect.Burned:
                actionText.text = unitName + " suffers from his burn and takes " + damage + " damage.";
                UpdateBattleUI();
                yield return new WaitForSeconds(2);
                break;
            case BaseClass.StatusEffect.Poisoned:
                actionText.text = unitName + " suffers from his poison and takes " + damage + " damage.";
                UpdateBattleUI();
                yield return new WaitForSeconds(2);
                break;
        }
    }

    ///Stat description
    public IEnumerator ShowStatChangeDescription(BaseAttack.modifiedStat stat, int changeValue, bool isBuff)
    {
        actionDescriptionPanel.SetActive(true);
        if (isBuff)
        {
            actionText.text = BSM.actionsToPerform[0].attackTargets[0].GetComponent<BaseClass>().unitName + "'s " + stat + " goes up by " + changeValue + ".";
            yield return new WaitForSeconds(2);
        }
        else
        {
            actionText.text = BSM.actionsToPerform[0].attackTargets[0].GetComponent<BaseClass>().unitName + "'s " + stat + " goes down by " + changeValue + ".";
            yield return new WaitForSeconds(2);
        }
    }

    ///Status descriptions
    public IEnumerator ShowStatusEffectActionDescription(BaseClass.StatusEffect activeStatus, bool canAttack)
    {
        actionDescriptionPanel.SetActive(true);
        switch (activeStatus)
        {
            case BaseClass.StatusEffect.Asleep:
                actionText.text = BSM.actionsToPerform[0].attackerName + " is sleeping and cannot attack.";
                yield return new WaitForSeconds(2);
                break;
            case BaseClass.StatusEffect.Confused:
                actionText.text = BSM.actionsToPerform[0].attackerName + " is confused..";
                yield return new WaitForSeconds(2);
                break;
            case BaseClass.StatusEffect.Paralyzed:
                if (canAttack)
                {
                    actionText.text = BSM.actionsToPerform[0].attackerName + " is paralyzed..";
                    yield return new WaitForSeconds(2);
                }
                else
                {
                    actionText.text = BSM.actionsToPerform[0].attackerName + " is paralyzed..";
                    yield return new WaitForSeconds(2);
                    actionText.text = BSM.actionsToPerform[0].attackerName + " is not able to move.";
                    yield return new WaitForSeconds(2);
                }
                break;
            case BaseClass.StatusEffect.Provoked:
                actionText.text = BSM.actionsToPerform[0].attackerName + " is provoked by " + BSM.actionsToPerform[0].attackerGO.GetComponent<BaseClass>().provokerGO.GetComponent<BaseClass>().unitName + ".";
                yield return new WaitForSeconds(2);
                break;
            case BaseClass.StatusEffect.Stunned:
                actionText.text = BSM.actionsToPerform[0].attackerName + " is stunned and cannot move.";
                yield return new WaitForSeconds(2);
                break;
        }
        yield return null;
    }

    public IEnumerator ShowStatusTakenDescription(BaseClass.StatusEffect status)
    {
        actionDescriptionPanel.SetActive(true);
        switch (status)
        {
            case BaseClass.StatusEffect.Asleep:
                actionText.text = BSM.actionsToPerform[0].attackTargets[0].GetComponent<BaseClass>().unitName + " falls asleep.";
                yield return new WaitForSeconds(2);
                break;
            case BaseClass.StatusEffect.Burned:
                actionText.text = BSM.actionsToPerform[0].attackTargets[0].GetComponent<BaseClass>().unitName + " was burned.";
                yield return new WaitForSeconds(2);
                break;
            case BaseClass.StatusEffect.Confused:
                actionText.text = BSM.actionsToPerform[0].attackTargets[0].GetComponent<BaseClass>().unitName + " was confused.";
                yield return new WaitForSeconds(2);
                break;
            case BaseClass.StatusEffect.Paralyzed:
                actionText.text = BSM.actionsToPerform[0].attackTargets[0].GetComponent<BaseClass>().unitName + " was paralyzed.";
                yield return new WaitForSeconds(2);
                break;
            case BaseClass.StatusEffect.Poisoned:
                actionText.text = BSM.actionsToPerform[0].attackTargets[0].GetComponent<BaseClass>().unitName + " was poisoned.";
                yield return new WaitForSeconds(2);
                break;
            case BaseClass.StatusEffect.Stunned:
                actionText.text = BSM.actionsToPerform[0].attackTargets[0].GetComponent<BaseClass>().unitName + " was stunned.";
                yield return new WaitForSeconds(2);
                break;
        }
    }

    public IEnumerator ShowStatusTakenDescription(BaseClass.StatusEffect status, GameObject provoker)
    {
        actionDescriptionPanel.SetActive(true);
        actionText.text = BSM.actionsToPerform[0].attackTargets[0].GetComponent<BaseClass>().unitName + " is provoked by " + provoker.GetComponent<BaseClass>().unitName + ".";
        yield return new WaitForSeconds(2);
    }   ///only for provoke

    public IEnumerator ShowStatusRemovedDescription(BaseClass.StatusEffect status)
    {
        actionDescriptionPanel.SetActive(true);
        switch (status)
        {
            case BaseClass.StatusEffect.Asleep:
                actionText.text = BSM.actionsToPerform[0].attackerName + " wakes up.";
                yield return new WaitForSeconds(2);
                break;
            case BaseClass.StatusEffect.Burned:
                actionText.text = BSM.actionsToPerform[0].attackerName + " is no longer burned.";
                yield return new WaitForSeconds(2);
                break;
            case BaseClass.StatusEffect.Confused:
                actionText.text = BSM.actionsToPerform[0].attackerName + " is no longer confused.";
                yield return new WaitForSeconds(2);
                break;
            case BaseClass.StatusEffect.Paralyzed:
                actionText.text = BSM.actionsToPerform[0].attackerName + " is no longer paralyzed.";
                yield return new WaitForSeconds(2);
                break;
            case BaseClass.StatusEffect.Poisoned:
                actionText.text = BSM.actionsToPerform[0].attackerName + " is no longer poisoned.";
                yield return new WaitForSeconds(2);
                break;
            case BaseClass.StatusEffect.Provoked:
                actionText.text = BSM.actionsToPerform[0].attackerName + " is no longer provoked.";
                yield return new WaitForSeconds(2);
                break;
            case BaseClass.StatusEffect.Stunned:
                actionText.text = BSM.actionsToPerform[0].attackerName + " is no longer stunned.";
                yield return new WaitForSeconds(2);
                break;
        }
    }

    ///Hide 
    public void HideActionDescription()
    {
        actionDescriptionPanel.SetActive(false);
    }
    #endregion
}


