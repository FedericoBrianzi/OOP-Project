using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSwitch : MonoBehaviour
{
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioSource voiceSource;
    [SerializeField] private AudioClip teamSelectClip, battleClip, winScreenClip, loseScreenClip;

    private void Awake()
    {
        GameManager.OnGameStateChanged += GameManagerOnGameStateChanged;
    }

    private void Start()
    {
        if (PlayerPrefs.GetInt("firstTimeAppOpens", 0) == 0)
        {
            voiceSource.enabled = false;
        }
        else
        {
            PlayerPrefs.SetInt("firstTimeAppOpens", 1);
            PlayerPrefs.Save();
        }
    }

    private void GameManagerOnGameStateChanged(GameManager.GameState state)
    {
        if (state == GameManager.GameState.PickTeams)
        {
            source.clip = teamSelectClip;
            source.Play();
        }
        else if (state == GameManager.GameState.BattleStart)
        {
            voiceSource.Stop();
            source.clip = battleClip;
            source.Play();
        }
        else if (state == GameManager.GameState.BattleWon)
        {
            voiceSource.Stop();
            source.clip = winScreenClip;
            source.Play();
        }
        else
        {
            voiceSource.Stop();
            source.clip = loseScreenClip;
            source.Play();
        }
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManagerOnGameStateChanged;
    }
}
