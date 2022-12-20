using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : BaseAttack
{
    public Fireball()
    {
        attackName = "Fireball";
        attackDamage = 15;
        attackManaCost = 25;
        statBuffQuantity = 0;
        statNerfQuantity = 0;
        statusSuccessRate = 15;

        ignoreArmor = false;

        numberOfTargets = typeOfTarget.AllEnemyTargets;
        buffedStat = modifiedStat.NONE;
        nerfedStat = modifiedStat.NONE;
        attackType = typeOfAttack.DmgAndStatus;
        statusChange = StatusEffect.Burn;
    }
}
