using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : BaseAttack
{
    public Barrier()
    {
        attackName = "Barrier";
        attackDamage = 0;
        attackManaCost = 15;
        statBuffQuantity = 10;
        statNerfQuantity = 0;
        statusSuccessRate = 0;

        ignoreArmor = false;

        numberOfTargets = typeOfTarget.AllAllyTargets;
        buffedStat = modifiedStat.Armor;
        nerfedStat = modifiedStat.NONE;
        attackType = typeOfAttack.StatChange;
        statusChange = StatusEffect.NONE;
    }
}
