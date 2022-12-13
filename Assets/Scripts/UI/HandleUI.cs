using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HandleUI : MonoBehaviour
{
    [SerializeField] private GameObject teamsSelectPanel;
    [SerializeField] private GameObject battlePanel;

    [SerializeField] private GameObject[] teamsStats = new GameObject[6];

    [SerializeField] private GameObject actionPanel;
    [SerializeField] private GameObject unitNameText;
    [SerializeField] private Button[] attackButtons;

    private void Awake()
    {
        GameManager.OnGameStateChanged += GameManagerOnGameStateChanged;
    }

    private void GameManagerOnGameStateChanged(GameManager.GameState state)
    {
        teamsSelectPanel.SetActive(state == GameManager.GameState.PickTeams);
        battlePanel.SetActive(state == GameManager.GameState.BattleStart || state == GameManager.GameState.PlayerTurn);
        if (state == GameManager.GameState.BattleStart)
        {
            UpdateBattleUI();
        }
        if (state == GameManager.GameState.PlayerTurn)
        {
            int i = 0;
            //unitNameText.GetComponent<TextMeshProUGUI>().text = GameManager.Instance.GetPlayerTeam()[0].GetComponent<BaseClass>().name;
            //foreach(Button button in attackButtons)
            //{
            //    button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = GameManager.Instance.GetPlayerTeam()[0].GetComponent<BaseClass>().attacks[i].attackName;
            //    //button.onClick()
            //    i++;
            //}
        }

    }

    private void UpdateBattleUI()
    {
        int i = 0;
        foreach(GameObject unitUI in teamsStats)
        {
            unitUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = SpawnManager.Instance.units[i].GetComponent<BaseClass>().name; //da problemi quando una unita verra distrutta
            unitUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "HP: " + SpawnManager.Instance.units[i].GetComponent<BaseClass>().currentHealth + " / " + SpawnManager.Instance.units[i].GetComponent<BaseClass>().maxHealth;
            unitUI.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "MP: " + SpawnManager.Instance.units[i].GetComponent<BaseClass>().currentMana + " / " + SpawnManager.Instance.units[i].GetComponent<BaseClass>().maxMana; ;
            i++;
        }
    }
}
