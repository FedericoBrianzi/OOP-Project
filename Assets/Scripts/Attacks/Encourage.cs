using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Encourage : BaseAttack
{
    public Encourage()
    {
        attackName = "Encourage";
        attackDamage = 0;
        attackManaCost = 5;
        statBuffQuantity = 15;
        statNerfQuantity = 0;
        statusSuccessRate = 0;

        ignoreArmor = false;

        numberOfTargets = typeOfTarget.SingleAllyTarget;
        buffedStat = modifiedStat.Attack;
        nerfedStat = modifiedStat.NONE;
        attackType = typeOfAttack.StatChange;
        statusChange = StatusEffect.NONE;
    }
}
