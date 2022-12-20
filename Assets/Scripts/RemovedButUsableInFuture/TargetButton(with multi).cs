using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TargetButtonMulti : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public BaseClass buttonTargetClass;
    [SerializeField] private Material redMaterial;
    [SerializeField] private Material yellowMaterial;
    private Material originalMaterial;
    private BaseAttack.typeOfTarget targetType;
    private BattleStateMachine BSM;
    private List<BaseClass> unitsOnField = new List<BaseClass>();
    private void Start()
    {
        originalMaterial = buttonTargetClass.indicator.GetComponent<MeshRenderer>().material;
        BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        unitsOnField.Clear();
        switch (targetType)
        {
            case BaseAttack.typeOfTarget.SingleEnemyTarget:
                buttonTargetClass.indicator.GetComponent<MeshRenderer>().material = redMaterial;
                buttonTargetClass.indicator.SetActive(true);
                break;
            //case BaseAttack.typeOfTarget.MultiEnemyTargets:
            //    foreach(GameObject enemyOnField in BSM.enemyTeam)
            //    {
            //        BaseClass enemyClass = enemyOnField.GetComponent<BaseClass>();
            //        unitsOnField.Add(enemyClass);
            //    }
            //    if(buttonTargetClass == unitsOnField[0])
            //    {
            //        unitsOnField[0].indicator.GetComponent<MeshRenderer>().material = redMaterial;
            //        unitsOnField[0].indicator.SetActive(true);
            //        unitsOnField[1].indicator.GetComponent<MeshRenderer>().material = redMaterial;
            //        unitsOnField[1].indicator.SetActive(true);
            //    }
            //    else if(buttonTargetClass == unitsOnField[1])
            //    {
            //        unitsOnField[0].indicator.GetComponent<MeshRenderer>().material = redMaterial;
            //        unitsOnField[0].indicator.SetActive(true);
            //        unitsOnField[1].indicator.GetComponent<MeshRenderer>().material = redMaterial;
            //        unitsOnField[1].indicator.SetActive(true);
            //        unitsOnField[2].indicator.GetComponent<MeshRenderer>().material = redMaterial;
            //        unitsOnField[2].indicator.SetActive(true);
            //    }
            //    else
            //    {
            //        unitsOnField[1].indicator.GetComponent<MeshRenderer>().material = redMaterial;
            //        unitsOnField[1].indicator.SetActive(true);
            //        unitsOnField[2].indicator.GetComponent<MeshRenderer>().material = redMaterial;
            //        unitsOnField[2].indicator.SetActive(true);
            //    }
            //    break;
            case BaseAttack.typeOfTarget.AllEnemyTargets:
                foreach (GameObject enemyOnField in BSM.enemyTeam)
                {
                    BaseClass enemyClass = enemyOnField.GetComponent<BaseClass>();
                    unitsOnField.Add(enemyClass);
                }
                unitsOnField[0].indicator.GetComponent<MeshRenderer>().material = redMaterial;
                unitsOnField[0].indicator.SetActive(true);
                unitsOnField[1].indicator.GetComponent<MeshRenderer>().material = redMaterial;
                unitsOnField[1].indicator.SetActive(true);
                unitsOnField[2].indicator.GetComponent<MeshRenderer>().material = redMaterial;
                unitsOnField[2].indicator.SetActive(true);
                break;
            case BaseAttack.typeOfTarget.SingleAllyTarget:
                buttonTargetClass.indicator.GetComponent<MeshRenderer>().material = yellowMaterial;
                buttonTargetClass.indicator.SetActive(true);
                break;
            //case BaseAttack.typeOfTarget.MultiAllyTargets:
            //    foreach (GameObject enemyOnField in BSM.playerTeam)
            //    {
            //        BaseClass playerClass = enemyOnField.GetComponent<BaseClass>();
            //        unitsOnField.Add(playerClass);
            //    }
            //    if (buttonTargetClass == unitsOnField[0])
            //    {
            //        unitsOnField[0].indicator.GetComponent<MeshRenderer>().material = yellowMaterial;
            //        unitsOnField[0].indicator.SetActive(true);
            //        unitsOnField[1].indicator.GetComponent<MeshRenderer>().material = yellowMaterial;
            //        unitsOnField[1].indicator.SetActive(true);
            //    }
            //    else if (buttonTargetClass == unitsOnField[1])
            //    {
            //        unitsOnField[0].indicator.GetComponent<MeshRenderer>().material = yellowMaterial;
            //        unitsOnField[0].indicator.SetActive(true);
            //        unitsOnField[1].indicator.GetComponent<MeshRenderer>().material = yellowMaterial;
            //        unitsOnField[1].indicator.SetActive(true);
            //        unitsOnField[2].indicator.GetComponent<MeshRenderer>().material = yellowMaterial;
            //        unitsOnField[2].indicator.SetActive(true);
            //    }
            //    else
            //    {
            //        unitsOnField[1].indicator.GetComponent<MeshRenderer>().material = yellowMaterial;
            //        unitsOnField[1].indicator.SetActive(true);
            //        unitsOnField[2].indicator.GetComponent<MeshRenderer>().material = yellowMaterial;
            //        unitsOnField[2].indicator.SetActive(true);
            //    }
            //    break;
            case BaseAttack.typeOfTarget.AllAllyTargets:
                foreach (GameObject enemyOnField in BSM.playerTeam)
                {
                    BaseClass playerClass = enemyOnField.GetComponent<BaseClass>();
                    unitsOnField.Add(playerClass);
                }
                unitsOnField[0].indicator.GetComponent<MeshRenderer>().material = yellowMaterial;
                unitsOnField[0].indicator.SetActive(true);
                unitsOnField[1].indicator.GetComponent<MeshRenderer>().material = yellowMaterial;
                unitsOnField[1].indicator.SetActive(true);
                unitsOnField[2].indicator.GetComponent<MeshRenderer>().material = yellowMaterial;
                unitsOnField[2].indicator.SetActive(true);
                break;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideIndicator();
    }

    public void HideIndicator() //funziona ma toglie l'indicatore da chi sta compiendo l'azione
    {
        unitsOnField.Clear();
        foreach (GameObject enemyOnField in BSM.playerTeam)
        {
            BaseClass playerClass = enemyOnField.GetComponent<BaseClass>();
            unitsOnField.Add(playerClass);
        }
        foreach (GameObject enemyOnField in BSM.enemyTeam)
        {
            BaseClass enemyClass = enemyOnField.GetComponent<BaseClass>();
            unitsOnField.Add(enemyClass);
        }
        foreach(BaseClass unit in unitsOnField)
        {
            unit.indicator.SetActive(false);
            unit.indicator.GetComponent<MeshRenderer>().material = originalMaterial;
        }
        //buttonTargetClass.GetComponent<BaseClass>().indicator.SetActive(false);
        //buttonTargetClass.indicator.GetComponent<MeshRenderer>().material = originalMaterial;
    }

    public void setTargetType(BaseAttack.typeOfTarget type)
    {
        targetType = type;
    }
}

