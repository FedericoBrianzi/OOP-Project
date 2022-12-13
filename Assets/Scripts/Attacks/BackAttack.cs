using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackAttack : BaseAttack
{
    public BackAttack()
    {
        attackName = "Back Attack";
        attackDamage = 20;
        attackManaCost = 15;
        statBuffQuantity = 0;
        statNerfQuantity = 10;

        ignoreArmor = true;
        defendPose = false;

        numberOfTargets = typeOfTarget.SingleEnemyTarget;
        buffedStat = modifiedStat.NONE;
        nerfedStat = modifiedStat.Armor;
    }
}
