using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelectionButton : MonoBehaviour
{
    public GameObject selectedUnit = null;

    public void PlaceUnit()
    {
        if(selectedUnit != null) SpawnManager.Instance.PlaceOnField(selectedUnit);
    }
}
