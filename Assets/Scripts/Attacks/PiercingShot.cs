using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiercingShot : BaseAttack
{
    public PiercingShot()
    {
        attackName = "PiercingShot";
        attackDamage = 10;
        attackManaCost = 10;
        statBuffQuantity = 0;
        statNerfQuantity = 0;

        ignoreArmor = true;
        defendPose = false;

        numberOfTargets = typeOfTarget.SingleEnemyTarget;
        buffedStat = modifiedStat.NONE;
        nerfedStat = modifiedStat.NONE;
    }
}
