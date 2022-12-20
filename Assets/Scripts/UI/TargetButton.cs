using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TargetButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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

            case BaseAttack.typeOfTarget.SingleAllyTarget:
                buttonTargetClass.indicator.GetComponent<MeshRenderer>().material = yellowMaterial;
                buttonTargetClass.indicator.SetActive(true);
                break;

                ///The system automatically selects all the target so this section will never be visible but could be used anyway if the system gets changed
                //case BaseAttack.typeOfTarget.AllEnemyTargets:     
                //    foreach (GameObject enemyOnField in BSM.enemyTeam)
                //    {
                //        BaseClass enemyClass = enemyOnField.GetComponent<BaseClass>();
                //        unitsOnField.Add(enemyClass);
                //    }
                //    unitsOnField[0].indicator.GetComponent<MeshRenderer>().material = redMaterial;
                //    unitsOnField[0].indicator.SetActive(true);
                //    unitsOnField[1].indicator.GetComponent<MeshRenderer>().material = redMaterial;
                //    unitsOnField[1].indicator.SetActive(true);
                //    unitsOnField[2].indicator.GetComponent<MeshRenderer>().material = redMaterial;
                //    unitsOnField[2].indicator.SetActive(true);
                //    break;

                ///Same reasons as above
                //case BaseAttack.typeOfTarget.AllAllyTargets:
                //    foreach (GameObject enemyOnField in BSM.playerTeam)
                //    {
                //        BaseClass playerClass = enemyOnField.GetComponent<BaseClass>();
                //        unitsOnField.Add(playerClass);
                //    }
                //    unitsOnField[0].indicator.GetComponent<MeshRenderer>().material = yellowMaterial;
                //    unitsOnField[0].indicator.SetActive(true);
                //    unitsOnField[1].indicator.GetComponent<MeshRenderer>().material = yellowMaterial;
                //    unitsOnField[1].indicator.SetActive(true);
                //    unitsOnField[2].indicator.GetComponent<MeshRenderer>().material = yellowMaterial;
                //    unitsOnField[2].indicator.SetActive(true);
                //    break;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideIndicator();
    }

    public void HideIndicator()
    {
        unitsOnField.Clear();
        foreach (GameObject heroOnField in BSM.playerTeam)
        {
            BaseClass playerClass = heroOnField.GetComponent<BaseClass>();
            unitsOnField.Add(playerClass);
        }
        foreach (GameObject enemyOnField in BSM.enemyTeam)
        {
            BaseClass enemyClass = enemyOnField.GetComponent<BaseClass>();
            unitsOnField.Add(enemyClass);
        }
        foreach(BaseClass unit in unitsOnField)
        {
            if(unit == BSM.GetHeroToManageClass())
            {
                unit.indicator.GetComponent<MeshRenderer>().material = originalMaterial;
            }
            else
            {
                unit.indicator.SetActive(false);
                unit.indicator.GetComponent<MeshRenderer>().material = originalMaterial;
            }  
        }
    }

    public void setTargetType(BaseAttack.typeOfTarget type)
    {
        targetType = type;
    }
}

