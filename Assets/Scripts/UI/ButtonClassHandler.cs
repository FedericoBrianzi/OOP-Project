using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class ButtonClassHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI classDescription;
    [SerializeField] private TextMeshProUGUI classStats;
    [SerializeField] private TextMeshProUGUI classAttacks;

    [SerializeField] private Button unitSelectionButton;
    [SerializeField] private GameObject unitPrefab;

    private BaseClass classInfo;
    private UnitSelectionButton unitSelection;
    private void Awake()
    {
        classInfo = unitPrefab.GetComponent<BaseClass>();
        unitSelection = unitSelectionButton.GetComponent<UnitSelectionButton>();
    }

    public void ShowDescription()
    {
        classDescription.text = classInfo.description;
        classStats.text = "Health: " + classInfo.health + "\n" + "Mana: " + classInfo.mana + "\n" + "Attack: " + classInfo.attack + "\n" + "Armor: " + classInfo.armor + "\n" + "Speed: " + classInfo.speed + "\n" + "Evasion: " + classInfo.evasion;
        classAttacks.text = "";
        for (int i = 0; i < classInfo.attacks.Count; i++)
        {
            if(classAttacks.text == "")
            {
                classAttacks.text += classInfo.attacks[i];
            }
            else  classAttacks.text += "\n" + classInfo.attacks[i];
        }
    }

    public void ShowSelectedDescription()
    {
        if (unitSelection.selectedUnit != null)
        {
            classDescription.text = unitSelection.selectedUnit.GetComponent<BaseClass>().description;
            classStats.text = "Health: " + classInfo.health + "\n" + "Mana: " + classInfo.mana + "\n" + "Attack: " + classInfo.attack + "\n" + "Armor: " + classInfo.armor + "\n" + "Speed: " + classInfo.speed + "\n" + "Evasion: " + classInfo.evasion;
            classAttacks.text = "";
            for (int i = 0; i < classInfo.attacks.Count; i++)
            {
                if (classAttacks.text == "")
                {
                    classAttacks.text += classInfo.attacks[i];
                }
                else classAttacks.text += "\n" + classInfo.attacks[i];
            }
        }
        else
        {
            classDescription.text = "";
            classStats.text = "";
            classAttacks.text = "";
        }
    }

    public void SelectUnit()
    {
        unitSelection.selectedUnit = unitPrefab;
    }
}
