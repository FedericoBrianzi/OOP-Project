using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evade : BaseAttack
{
    public Evade()
    {
        attackName = "Evade";
        attackDamage = 0;
        attackManaCost = 5;
        statBuffQuantity = 25;
        statNerfQuantity = 0;
        statusSuccessRate = 0;

        ignoreArmor = false;

        numberOfTargets = typeOfTarget.Self;
        buffedStat = modifiedStat.Evasion;
        nerfedStat = modifiedStat.NONE;
        attackType = typeOfAttack.StatChange;
        statusChange = StatusEffect.NONE;
    }
}
