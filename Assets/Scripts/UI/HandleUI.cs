using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    [Header("Attack input")]
    [SerializeField] private List<GameObject> attackButtons = new List<GameObject>();
    [SerializeField] private GameObject attackButtonsHolder;
    [SerializeField] private GameObject attackButtonPrefab;

    [Header("Target input")]
    [SerializeField] private List<GameObject> targetButtons = new List<GameObject>();
    [SerializeField] private GameObject targetButtonsHolder;
    [SerializeField] private GameObject targetButtonPrefab;

    private BattleStateMachine BSM;
    private List<GameObject> unitsOnField = new List<GameObject>();


    private void Awake()
    {
        GameManager.OnGameStateChanged += GameManagerOnGameStateChanged;
        BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
    }

    private void GameManagerOnGameStateChanged(GameManager.GameState state)
    {
        teamsSelectPanel.SetActive(state == GameManager.GameState.PickTeams);

        battlePanel.SetActive(state == GameManager.GameState.BattleStart || state == GameManager.GameState.PlayerTurn);
        if (state == GameManager.GameState.BattleStart)
        {
            unitsOnField.AddRange(BSM.playerTeam);
            unitsOnField.AddRange(BSM.enemyTeam);
            UpdateBattleUI();
        }
    }

    public void UpdateBattleUI()    //shows current stats for every unit
    {
        int i = 0;
        foreach(GameObject unitUI in teamsStatsUI)
        {
            unitUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = unitsOnField[i].GetComponent<BaseClass>().name; //da problemi quando una unita verra distrutta
            unitUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "HP: " + unitsOnField[i].GetComponent<BaseClass>().currentHealth + " / " + unitsOnField[i].GetComponent<BaseClass>().maxHealth;
            unitUI.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "MP: " + unitsOnField[i].GetComponent<BaseClass>().currentMana + " / " + unitsOnField[i].GetComponent<BaseClass>().maxMana;
            i++;
        }
    }

    public void ActivateActionPanel(GameObject hero)  //shows the player's current unit name and creates the attack buttons for that unit
    {
        actionPanel.SetActive(true);
        attackButtons.Clear();
        BaseClass heroClass = hero.GetComponent<BaseClass>();

        unitNameText.GetComponent<TextMeshProUGUI>().text = heroClass.name;

        foreach (BaseAttack attack in heroClass.attacks)
        {
            GameObject atkButton = Instantiate(attackButtonPrefab);
            atkButton.transform.SetParent(attackButtonsHolder.transform, false);
            attackButtons.Add(atkButton);

            atkButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = attack.attackName;
            atkButton.GetComponent<AttackButton>().buttonAttack = attack;
            atkButton.GetComponent<Button>().onClick.AddListener(() => BSM.AttackInput(attack));
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

    public void ActivateTargetPanel(GameObject[] targets)
    {
        targetPanel.SetActive(true);
        targetButtons.Clear();
        foreach (GameObject target in targets)
        {
            GameObject targetButton = Instantiate(targetButtonPrefab);
            targetButton.transform.SetParent(targetButtonsHolder.transform, false);
            targetButtons.Add(targetButton);

            targetButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = target.GetComponent<BaseClass>().name;
            targetButton.GetComponent<TargetButton>().buttonTarget = target.GetComponent<BaseClass>();
            targetButton.GetComponent<Button>().onClick.AddListener(() => BSM.TargetInput(target));
        }
    }

    public void ActivateMultiTargetPanel(GameObject[] targets) 
    {
        Debug.Log(targets.Length);
        targetPanel.SetActive(true);
        targetButtons.Clear();
        foreach (GameObject target in targets)
        {
            GameObject targetButton = Instantiate(targetButtonPrefab);
            targetButton.transform.SetParent(targetButtonsHolder.transform, false);
            targetButtons.Add(targetButton);

            targetButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = target.GetComponent<BaseClass>().name;
            targetButton.GetComponent<TargetButton>().buttonTarget = target.GetComponent<BaseClass>();
        }

        if (targets.Length == 1)
        {
            targetButtons[0].GetComponent<Button>().onClick.AddListener(() => BSM.TargetInput(targets[0]));
        }
        else if (targets.Length == 3)
        {
            targetButtons[0].GetComponent<Button>().onClick.AddListener(() => BSM.MultiTargetInput(SetMultiTargets(0).ToArray()));
            targetButtons[1].GetComponent<Button>().onClick.AddListener(() => BSM.MultiTargetInput(SetMultiTargets(1).ToArray()));
            targetButtons[2].GetComponent<Button>().onClick.AddListener(() => BSM.MultiTargetInput(SetMultiTargets(2).ToArray()));
        }
        else
        {
            foreach(GameObject targetButton in targetButtons)
            {
                targetButton.GetComponent<Button>().onClick.AddListener(() => BSM.MultiTargetInput(targets));
            }
        }
    }

    private List<GameObject> SetMultiTargets(int buttonIndex)
    {
        List<GameObject> targets = new List<GameObject>();
        if (buttonIndex == 0)
        {
            targets.Add(targetButtons[0].GetComponent<TargetButton>().buttonTarget.gameObject);
            targets.Add(targetButtons[1].GetComponent<TargetButton>().buttonTarget.gameObject);
        }
        else if (buttonIndex == 1)
        {
            targets.Add(targetButtons[0].GetComponent<TargetButton>().buttonTarget.gameObject);
            targets.Add(targetButtons[1].GetComponent<TargetButton>().buttonTarget.gameObject);
            targets.Add(targetButtons[2].GetComponent<TargetButton>().buttonTarget.gameObject);
        }
        else
        {
            targets.Add(targetButtons[1].GetComponent<TargetButton>().buttonTarget.gameObject);
            targets.Add(targetButtons[2].GetComponent<TargetButton>().buttonTarget.gameObject);
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
}
