using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dodge : BaseAttack
{
    public Dodge()
    {
        attackName = "Dodge";
        attackDamage = 0;
        attackManaCost = 5;
        statBuffQuantity = 35;
        statNerfQuantity = 0;

        ignoreArmor = false;

        numberOfTargets = typeOfTarget.Self;
        buffedStat = modifiedStat.Evasion;
        nerfedStat = modifiedStat.NONE;
        attackType = typeOfAttack.StatChange;
    }
}
