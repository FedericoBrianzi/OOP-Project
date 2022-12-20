using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AttackButton : MonoBehaviour
{
    public BaseAttack buttonAttack;
    [SerializeField] private TextMeshProUGUI mPCostText;
    private void Start()
    {
        mPCostText.text = "MP: " + buttonAttack.attackManaCost.ToString();
    }
}
