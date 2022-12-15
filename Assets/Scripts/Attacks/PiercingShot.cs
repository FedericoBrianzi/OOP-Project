using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiercingShot : BaseAttack
{
    public PiercingShot()
    {
        attackName = "PiercingShot";
        attackDamage = 20;
        attackManaCost = 10;
        statBuffQuantity = 0;
        statNerfQuantity = 0;
        statusSuccessRate = 0;

        ignoreArmor = true;

        numberOfTargets = typeOfTarget.SingleEnemyTarget;
        buffedStat = modifiedStat.NONE;
        nerfedStat = modifiedStat.NONE;
        attackType = typeOfAttack.Damage;
        statusChange = StatusEffect.NONE;
    }
}
