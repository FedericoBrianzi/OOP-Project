using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class HandleUI : MonoBehaviour
{
    [Header("Team Selection Phase")]
    [SerializeField] private GameObject teamsSelectPanel;
    [SerializeField] private GameObject battlePanel;

    [Header("Battle Phase")]
    [SerializeField] private GameObject[] teamsStatsUI = new GameObject[6];

    [SerializeField] private GameObject actionPanel;
    [SerializeField] private GameObject targetPanel;
    [SerializeField] private GameObject unitNameText;
    [SerializeField] private GameObject targetTeamText;

    [Header("Attack buttons")]
    [SerializeField] private List<GameObject> attackButtons = new List<GameObject>();
    [SerializeField] private GameObject attackButtonsHolder;
    [SerializeField] private GameObject attackButtonPrefab;
    [SerializeField] private GameObject previousTurnButton;

    [Header("Target buttons")]
    [SerializeField] private List<GameObject> targetButtons = new List<GameObject>();
    [SerializeField] private GameObject targetButtonsHolder;
    [SerializeField] private GameObject targetButtonPrefab;
    [SerializeField] private GameObject previousAttackButton;

    [Header("Action Description")]
    [SerializeField] private GameObject actionDescriptionPanel;
    private TextMeshProUGUI actionText;

    [Header("Battle End Phase")]
    [SerializeField] private GameObject battleEndPanel;
    [SerializeField] private GameObject wonBattleText;
    [SerializeField] private GameObject lostBattleText;

    private BattleStateMachine BSM;
    private List<GameObject> unitsOnField = new List<GameObject>();

    private BaseClass heroClass;

    public List<TextMeshProUGUI> nameTexts, hpTexts, mpTexts = new List<TextMeshProUGUI>();

    [System.Serializable]
    public class unitInfo
    {
        BaseClass unitClass;
        string nameInBattle;
        string prefix;

        public unitInfo (BaseClass unit, string name)
        {
            unitClass = unit;
            nameInBattle = name;
            prefix = "";
        }
        
        public BaseClass GetBaseClass()
        {
            return unitClass;
        }

        public string GetName()
        {
            return nameInBattle; 
        }

        public string GetNameAndPrefix()
        {
            return prefix + nameInBattle;
        }

        public void SetNameSuffix(string suffix)
        {
            nameInBattle += suffix;
        }

        public void AddEnemyPrefix()
        {
            prefix = "Enemy ";
        }
    }
    
    public List<unitInfo> unitIdentifiers = new List<unitInfo>();
    
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
            for (int i = 0; i < teamsStatsUI.Length; i++)
            {
                nameTexts.Insert(i, teamsStatsUI[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>());
                hpTexts.Insert(i, teamsStatsUI[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>());
                mpTexts.Insert(i, teamsStatsUI[i].transform.GetChild(2).GetComponent<TextMeshProUGUI>());
            }

            unitsOnField.AddRange(BSM.playerTeam);
            foreach (GameObject unit in BSM.playerTeam)
            {
                unitInfo unitID = new unitInfo(unit.GetComponent<BaseClass>(), unit.GetComponent<BaseClass>().unitName);
                unitIdentifiers.Add(unitID);
            }

            unitsOnField.AddRange(BSM.enemyTeam);
            foreach (GameObject unit in BSM.enemyTeam)
            {
                unitInfo unitID = new unitInfo(unit.GetComponent<BaseClass>(), unit.GetComponent<BaseClass>().unitName);
                unitID.AddEnemyPrefix();
                unitIdentifiers.Add(unitID);
            }

            CheckForSuffix();   ///Same names in same team will get a "(2)", "(3)" suffix
            UpdateBattleUI();
        }

        battleEndPanel.SetActive(state == GameManager.GameState.BattleWon || state == GameManager.GameState.BattleLost);
        wonBattleText.SetActive(state == GameManager.GameState.BattleWon);
        lostBattleText.SetActive(state == GameManager.GameState.BattleLost);
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManagerOnGameStateChanged;
    }

    private void CheckForSuffix()
    {
        int pCounter = 2;
        if (unitIdentifiers[0].GetName() == unitIdentifiers[1].GetName())
        {
            unitIdentifiers[1].SetNameSuffix("(" + pCounter + ")");
            pCounter++;
        }
        if (unitIdentifiers[0].GetName() == unitIdentifiers[2].GetName())
        {
            unitIdentifiers[2].SetNameSuffix("(" + pCounter + ")");
        }
        else if (unitIdentifiers[1].GetName() == unitIdentifiers[2].GetName())
        {
            unitIdentifiers[2].SetNameSuffix("(" + pCounter + ")");
        }

        int eCounter = 2;
        if (unitIdentifiers[3].GetName() == unitIdentifiers[4].GetName())
        {
            unitIdentifiers[4].SetNameSuffix("(" + eCounter + ")");
            eCounter++;
        }
        if (unitIdentifiers[3].GetName() == unitIdentifiers[5].GetName())
        {
            unitIdentifiers[5].SetNameSuffix("(" + eCounter + ")");
        }
        else if (unitIdentifiers[4].GetName() == unitIdentifiers[5].GetName())
        {
            unitIdentifiers[5].SetNameSuffix("(" + eCounter + ")");
        }
    }

    public void UpdateBattleUI()    ///shows current stats for every unit
    {
        for (int i = 0; i < teamsStatsUI.Length; i++)
        {
            nameTexts[i].text = unitIdentifiers[i].GetName();
            hpTexts[i].text = "HP: " + unitIdentifiers[i].GetBaseClass().GetCurrentHealth() + " / " + unitIdentifiers[i].GetBaseClass().maxHealth;
            mpTexts[i].text = "MP: " + unitIdentifiers[i].GetBaseClass().GetCurrentMana() + " / " + unitIdentifiers[i].GetBaseClass().maxMana;
        }
    }

    public string GetFullNameFromBase(BaseClass classToCheck)
    {
        foreach (unitInfo info in unitIdentifiers)
        {
            if (classToCheck == info.GetBaseClass())
            {
                return info.GetNameAndPrefix();
            }
        }
        return "";
    }   ///gives back prefix(if present), name and suffix(if present)

    public string GetNameWithSuffixFromBase(BaseClass classToCheck)
    {
        foreach (unitInfo info in unitIdentifiers)
        {
            if (classToCheck == info.GetBaseClass())
            {
                return info.GetName();
            }
        }
        return "";
    }   ///gives back name and suffix(if present)

    #region PlayerUI
    public void ActivateActionPanel(GameObject hero) 
    {
        actionPanel.SetActive(true);
        attackButtons.Clear();
        heroClass = hero.GetComponent<BaseClass>();

        previousTurnButton.SetActive(true);
        if (hero == BSM.playerTeam[0]) previousTurnButton.GetComponent<Button>().interactable = false;
        else previousTurnButton.GetComponent<Button>().interactable = true;

        unitNameText.GetComponent<TextMeshProUGUI>().text = GetFullNameFromBase(heroClass);

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
                if (attack.numberOfTargets == BaseAttack.typeOfTarget.SingleEnemyTarget || attack.numberOfTargets == BaseAttack.typeOfTarget.AllEnemyTargets) { }
                else atkButton.GetComponent<Button>().interactable = false;
            }
            if(attack.attackManaCost > heroClass.GetCurrentMana())
            {
                atkButton.GetComponent<Button>().interactable = false;
            }
        }
    }    ///shows the player's current unit name and creates the attack buttons for that unit

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

        previousAttackButton.SetActive(true);

        if (targets[0].CompareTag("Enemy"))
        {
            targetTeamText.GetComponent<TextMeshProUGUI>().text = "Select enemy";
        }
        else targetTeamText.GetComponent<TextMeshProUGUI>().text = "Select ally";

        foreach (GameObject target in targets)
        {
            GameObject targetButton = Instantiate(targetButtonPrefab);
            targetButton.transform.SetParent(targetButtonsHolder.transform, false);
            targetButtons.Add(targetButton);

            targetButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = GetNameWithSuffixFromBase(target.GetComponent<BaseClass>());
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
        BaseClass attackerClass = action.attackerGO.GetComponent<BaseClass>();

        if (action.attack.attackType == BaseAttack.typeOfAttack.Defend)
        {
            actionText.text = GetFullNameFromBase(attackerClass) + " defends himself";
            yield return new WaitForSeconds(2);
        }
        else if (action.attackTargets.Count == 1 && action.attackTargets[0] == action.attackerGO)
        {
            actionText.text = GetFullNameFromBase(attackerClass) + " uses " + action.attack.attackName + " on himself";
            yield return new WaitForSeconds(2);
        }
        else if (action.attackTargets.Count == 1)
        {
            actionText.text = GetFullNameFromBase(attackerClass) + " uses " + action.attack.attackName + " on " + GetFullNameFromBase(action.attackTargets[0].GetComponent<BaseClass>()) + ".";
            yield return new WaitForSeconds(2);
        }
        else if (action.attackTargets.Count == BSM.playerTeam.Count && action.attackTargets[0].gameObject.CompareTag("Player"))
        {
            actionText.text = GetFullNameFromBase(attackerClass) + " uses " + action.attack.attackName + " on player's team.";
            yield return new WaitForSeconds(2);
        }
        else if (action.attackTargets.Count == BSM.enemyTeam.Count && action.attackTargets[0].gameObject.CompareTag("Enemy"))
        {
            actionText.text = action.attackerName + " uses " + action.attack.attackName + " on enemy team.";
            yield return new WaitForSeconds(2);
        }
    }

    ///Damage descriptions
    public IEnumerator ShowDamageTakenDescription(int damageTaken)
    {
        actionDescriptionPanel.SetActive(true);
        actionText.text = GetFullNameFromBase(BSM.actionsToPerform[0].attackTargets[0].GetComponent<BaseClass>()) + " takes " + damageTaken + " damage.";
        UpdateBattleUI();
        yield return new WaitForSeconds(2);
    }   //POLYMORPHISM

    public IEnumerator ShowDamageTakenDescription(bool dodged)
    {
        actionDescriptionPanel.SetActive(true);
        if(dodged) actionText.text = GetFullNameFromBase(BSM.actionsToPerform[0].attackTargets[0].GetComponent<BaseClass>()) + " dodges the attack."; ///the bool check is not required
        UpdateBattleUI();
        yield return new WaitForSeconds(2);
    }   ///dodge    //POLYMORPHISM

    public IEnumerator ShowStatusDamageDescription(BaseClass.StatusEffect status, int damage, BaseClass unitClass)
    {
        actionDescriptionPanel.SetActive(true);
        switch (status)
        {
            case BaseClass.StatusEffect.Burned:
                actionText.text = GetFullNameFromBase(unitClass) + " suffers from his burn and takes " + damage + " damage.";
                UpdateBattleUI();
                yield return new WaitForSeconds(2);
                break;
            case BaseClass.StatusEffect.Poisoned:
                actionText.text = GetFullNameFromBase(unitClass) + " suffers from his poison and takes " + damage + " damage.";
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
            actionText.text = GetFullNameFromBase(BSM.actionsToPerform[0].attackTargets[0].GetComponent<BaseClass>()) + "'s " + stat + " goes up by " + changeValue + ".";
            yield return new WaitForSeconds(2);
        }
        else
        {
            actionText.text = GetFullNameFromBase(BSM.actionsToPerform[0].attackTargets[0].GetComponent<BaseClass>()) + "'s " + stat + " goes down by " + changeValue + ".";
            yield return new WaitForSeconds(2);
        }
    }

    ///Status descriptions
    public IEnumerator ShowStatusEffectActionDescription(BaseClass.StatusEffect activeStatus, bool canAttack)
    {
        actionDescriptionPanel.SetActive(true);
        BaseClass attackerClass = BSM.actionsToPerform[0].attackerGO.GetComponent<BaseClass>();
        switch (activeStatus)
        {
            case BaseClass.StatusEffect.Asleep:
                actionText.text = GetFullNameFromBase(attackerClass) + " is sleeping and cannot attack.";
                yield return new WaitForSeconds(2);
                break;
            case BaseClass.StatusEffect.Confused:
                actionText.text = GetFullNameFromBase(attackerClass) + " is confused..";
                yield return new WaitForSeconds(2);
                break;
            case BaseClass.StatusEffect.Paralyzed:
                if (canAttack)
                {
                    actionText.text = GetFullNameFromBase(attackerClass) + " is paralyzed..";
                    yield return new WaitForSeconds(2);
                }
                else
                {
                    actionText.text = GetFullNameFromBase(attackerClass) + " is paralyzed..";
                    yield return new WaitForSeconds(2);
                    actionText.text = GetFullNameFromBase(attackerClass) + " is not able to move.";
                    yield return new WaitForSeconds(2);
                }
                break;
            case BaseClass.StatusEffect.Provoked:
                actionText.text = GetFullNameFromBase(attackerClass) + " is provoked by " + GetFullNameFromBase(attackerClass.provokerGO.GetComponent<BaseClass>()) + ".";
                yield return new WaitForSeconds(2);
                break;
            case BaseClass.StatusEffect.Stunned:
                actionText.text = GetFullNameFromBase(attackerClass) + " is stunned and cannot move.";
                yield return new WaitForSeconds(2);
                break;
        }
        yield return null;
    }

    public IEnumerator ShowStatusTakenDescription(BaseClass.StatusEffect status) 
    {
        actionDescriptionPanel.SetActive(true);
        BaseClass targetClass = BSM.actionsToPerform[0].attackTargets[0].GetComponent<BaseClass>();
        switch (status)
        {
            case BaseClass.StatusEffect.Asleep:
                actionText.text = GetFullNameFromBase(targetClass) + " falls asleep.";
                yield return new WaitForSeconds(2);
                break;
            case BaseClass.StatusEffect.Burned:
                actionText.text = GetFullNameFromBase(targetClass) + " was burned.";
                yield return new WaitForSeconds(2);
                break;
            case BaseClass.StatusEffect.Confused:
                actionText.text = GetFullNameFromBase(targetClass) + " was confused.";
                yield return new WaitForSeconds(2);
                break;
            case BaseClass.StatusEffect.Paralyzed:
                actionText.text = GetFullNameFromBase(targetClass) + " was paralyzed.";
                yield return new WaitForSeconds(2);
                break;
            case BaseClass.StatusEffect.Poisoned:
                actionText.text = GetFullNameFromBase(targetClass) + " was poisoned.";
                yield return new WaitForSeconds(2);
                break;
            case BaseClass.StatusEffect.Stunned:
                actionText.text = GetFullNameFromBase(targetClass) + " was stunned.";
                yield return new WaitForSeconds(2);
                break;
        }
    }   //POLYMORPHISM

    public IEnumerator ShowStatusTakenDescription(BaseClass.StatusEffect status, GameObject provoker)
    {
        actionDescriptionPanel.SetActive(true);
        actionText.text = GetFullNameFromBase(BSM.actionsToPerform[0].attackTargets[0].GetComponent<BaseClass>()) + " is provoked by " + GetFullNameFromBase(provoker.GetComponent<BaseClass>()) + ".";
        yield return new WaitForSeconds(2);
    }   ///only for provoke //POLYMORPHISM

    public IEnumerator ShowStatusRemovedDescription(BaseClass.StatusEffect status)
    {
        actionDescriptionPanel.SetActive(true);
        BaseClass attackerClass = BSM.actionsToPerform[0].attackerGO.GetComponent<BaseClass>();
        switch (status)
        {
            case BaseClass.StatusEffect.Asleep:
                actionText.text = GetFullNameFromBase(attackerClass) + " wakes up.";
                yield return new WaitForSeconds(2);
                break;
            case BaseClass.StatusEffect.Burned:
                actionText.text = GetFullNameFromBase(attackerClass) + " is no longer burned.";
                yield return new WaitForSeconds(2);
                break;
            case BaseClass.StatusEffect.Confused:
                actionText.text = GetFullNameFromBase(attackerClass) + " is no longer confused.";
                yield return new WaitForSeconds(2);
                break;
            case BaseClass.StatusEffect.Paralyzed:
                actionText.text = GetFullNameFromBase(attackerClass) + " is no longer paralyzed.";
                yield return new WaitForSeconds(2);
                break;
            case BaseClass.StatusEffect.Poisoned:
                actionText.text = GetFullNameFromBase(attackerClass) + " is no longer poisoned.";
                yield return new WaitForSeconds(2);
                break;
            case BaseClass.StatusEffect.Provoked:
                actionText.text = GetFullNameFromBase(attackerClass) + " is no longer provoked.";
                yield return new WaitForSeconds(2);
                break;
            case BaseClass.StatusEffect.Stunned:
                actionText.text = GetFullNameFromBase(attackerClass) + " is no longer stunned.";
                yield return new WaitForSeconds(2);
                break;
        }
    }

    public IEnumerator ShowDeadUnitDescription(BaseClass targetClass)
    {
        actionDescriptionPanel.SetActive(true);
        actionText.text = GetFullNameFromBase(targetClass) + " fainted.";
        yield return new WaitForSeconds(2);
    }

    ///Hide 
    public void HideActionDescription()
    {
        actionDescriptionPanel.SetActive(false);
    }
    #endregion
}


