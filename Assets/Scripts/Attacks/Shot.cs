using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot : BaseAttack
{
    public Shot()
    {
        attackName = "Shot";
        attackDamage = 15;
        attackManaCost = 0;
        statBuffQuantity = 0;
        statNerfQuantity = 0;
        statusSuccessRate = 0;

        ignoreArmor = false;

        numberOfTargets = typeOfTarget.SingleEnemyTarget;
        buffedStat = modifiedStat.NONE;
        nerfedStat = modifiedStat.NONE;
        attackType = typeOfAttack.Damage;
        statusChange = StatusEffect.NONE;
    }
}
