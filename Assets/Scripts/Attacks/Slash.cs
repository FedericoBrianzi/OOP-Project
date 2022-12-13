using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash : BaseAttack
{
    public Slash()
    {
        attackName = "Slash";
        attackDamage = 10;
        attackManaCost = 5;
        statBuffQuantity = 0;
        statNerfQuantity = 0;

        ignoreArmor = false;
        defendPose = false;

        numberOfTargets = typeOfTarget.SingleEnemyTarget;
        buffedStat = modifiedStat.NONE;
        nerfedStat = modifiedStat.NONE;
    }
}
