using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//there are quite basic types of attacks but theorically there could be attacks that change 1 stat of the target and 1 stat of the user,
//or other powerful attacks that forces the unit's health/mana to be like 50% until the end of battle(it should change the maximum health and not the current).
//Also there could be attacks that deal damage to both the targets and the user(like kamikaze from DQ and Explosion from Pokemon).
//All these scenarios should have their own type of targets, type of attacks and these attacks should be evaluated by the BaseClass of the characters.

public class BaseAttack : MonoBehaviour
{
    public string attackName;
    public int attackDamage;
    public int attackManaCost;
    public int statBuffQuantity;
    public int statNerfQuantity;

    public bool ignoreArmor;
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
        StatChange,
        DmgAndStatChange
    }

    public typeOfTarget numberOfTargets;
    public modifiedStat buffedStat;
    public modifiedStat nerfedStat;
    public typeOfAttack attackType;
}