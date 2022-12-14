using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defend : BaseAttack
{
    public Defend()
    {
        attackName = "Defend";
        attackDamage = 0;
        attackManaCost = 0;
        statBuffQuantity = 0;
        statNerfQuantity = 0;

        ignoreArmor = false;

        numberOfTargets = typeOfTarget.Self;
        buffedStat = modifiedStat.NONE;
        nerfedStat = modifiedStat.NONE;
        attackType = typeOfAttack.Defend;
    }
}
