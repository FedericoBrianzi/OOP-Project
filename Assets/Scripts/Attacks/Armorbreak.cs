using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armorbreak : BaseAttack    //INHERITANCE
{
    public Armorbreak()
    {
        attackName = "Armorbreak";
        attackDamage = 15;
        attackManaCost = 15;
        statBuffQuantity = 0;
        statNerfQuantity = 15;
        statusSuccessRate = 0;

        ignoreArmor = false;

        numberOfTargets = typeOfTarget.SingleEnemyTarget;
        buffedStat = modifiedStat.NONE;
        nerfedStat = modifiedStat.Armor;
        attackType = typeOfAttack.DmgAndStatChange;
        statusChange = StatusEffect.NONE;
    }
}
