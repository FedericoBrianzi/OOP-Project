using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Encourage : BaseAttack
{
    public Encourage()
    {
        attackName = "Encourage";
        attackDamage = 0;
        attackManaCost = 10;
        statBuffQuantity = 20;
        statNerfQuantity = 0;

        ignoreArmor = false;

        numberOfTargets = typeOfTarget.SingleAllyTarget;
        buffedStat = modifiedStat.Attack;
        nerfedStat = modifiedStat.NONE;
        attackType = typeOfAttack.StatChange;
    }
}
