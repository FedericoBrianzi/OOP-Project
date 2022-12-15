using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : BaseAttack
{
    public Fireball()
    {
        attackName = "Fireball";
        attackDamage = 40;
        attackManaCost = 30;
        statBuffQuantity = 0;
        statNerfQuantity = 0;
        statusSuccessRate = 15;

        ignoreArmor = false;

        numberOfTargets = typeOfTarget.MultiEnemyTargets;
        buffedStat = modifiedStat.NONE;
        nerfedStat = modifiedStat.NONE;
        attackType = typeOfAttack.Damage;
        statusChange = StatusEffect.Burn;
    }
}
