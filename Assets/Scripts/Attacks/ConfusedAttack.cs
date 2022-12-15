using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfusedAttack : BaseAttack
{
    public ConfusedAttack()
    {
        attackName = "ConfusedAttack";
        attackDamage = 10;
        attackManaCost = 0;
        statBuffQuantity = 0;
        statNerfQuantity = 0;
        statusSuccessRate = 0;

        ignoreArmor = false;

        numberOfTargets = typeOfTarget.Random;
        buffedStat = modifiedStat.NONE;
        nerfedStat = modifiedStat.NONE;
        attackType = typeOfAttack.Damage;
        statusChange = StatusEffect.NONE;
    }
}
