using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smash : BaseAttack
{
    public Smash()
    {
        attackName = "Smash";
        attackDamage = 10;
        attackManaCost = 5;
        statBuffQuantity = 0;
        statNerfQuantity = 0;
        statusSuccessRate = 10;

        ignoreArmor = false;

        numberOfTargets = typeOfTarget.SingleEnemyTarget;
        buffedStat = modifiedStat.NONE;
        nerfedStat = modifiedStat.NONE;
        attackType = typeOfAttack.Damage;
        statusChange = StatusEffect.Stun;
    }
}
