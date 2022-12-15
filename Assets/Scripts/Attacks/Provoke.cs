using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Provoke : BaseAttack
{
    public Provoke()
    {
        attackName = "Provoke";
        attackDamage = 0;
        attackManaCost = 10;
        statBuffQuantity = 0;
        statNerfQuantity = 0;
        statusSuccessRate = 100;

        ignoreArmor = false;

        numberOfTargets = typeOfTarget.SingleEnemyTarget;
        buffedStat = modifiedStat.NONE;
        nerfedStat = modifiedStat.NONE;
        attackType = typeOfAttack.Status;
        statusChange = StatusEffect.Provoke;
    }
}
