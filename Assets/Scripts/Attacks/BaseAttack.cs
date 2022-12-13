using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[System.Serializable]
public class BaseAttack : MonoBehaviour
{
    public string attackName;
    public int attackDamage;
    public int attackManaCost;
    public int statBuffQuantity;
    public int statNerfQuantity;

    public bool ignoreArmor;
    public bool defendPose;
    public enum typeOfTarget
    {
        SingleEnemyTarget,
        MultiEnemyTargets,
        AllEnemyTargets,
        Self,
        SingleAllyTarget,
        MultiAllyTargets,
        AllAllyTargets
    }

    public enum modifiedStat
    {
        Health,
        Mana,
        Attack,
        Armor,
        Speed,
        Evasion,
        NONE
    }

    public enum typeOfAttack
    {
        Damage,
        Defend,
        StatChange
    }

    public typeOfTarget numberOfTargets;
    public modifiedStat buffedStat;
    public modifiedStat nerfedStat;
    public typeOfAttack attackType;
}