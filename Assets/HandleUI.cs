using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandleUI : MonoBehaviour
{
    [SerializeField] private GameObject teamsPanel;

    private void Awake()
    {
        GameManager.OnGameStateChanged += GameManagerOnGameStateChanged;
    }

    private void GameManagerOnGameStateChanged(GameManager.GameState state)
    {
        teamsPanel.SetActive(state == GameManager.GameState.PickTeams);
    }
}
