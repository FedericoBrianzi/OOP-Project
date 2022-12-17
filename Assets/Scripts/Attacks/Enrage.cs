using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enrage : BaseAttack
{
    public Enrage()
    {
        attackName = "Enrage";
        attackDamage = 0;
        attackManaCost = 10;
        statBuffQuantity = 25;
        statNerfQuantity = 10;
        statusSuccessRate = 0;

        ignoreArmor = false;

        numberOfTargets = typeOfTarget.Self;
        buffedStat = modifiedStat.Attack;
        nerfedStat = modifiedStat.Armor;
        attackType = typeOfAttack.StatChange;
        statusChange = StatusEffect.NONE;
    }
}
