using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HandleTurn : IComparable<HandleTurn>
{
    public string attackerName;
    public GameObject attackerGO;
    public List<GameObject> attackTargets = new List<GameObject>();

    public BaseAttack attack;

    public int CompareTo(HandleTurn other)
    {
        return this.attackerGO.GetComponent<BaseClass>().GetCurrentSpeed().CompareTo(other.attackerGO.GetComponent<BaseClass>().GetCurrentSpeed());
    }
}
