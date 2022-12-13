using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Haste : BaseAttack
{
    public Haste()
    {
        attackName = "Haste";
        attackDamage = 0;
        attackManaCost = 10;
        statBuffQuantity = 20;
        statNerfQuantity = 0;

        ignoreArmor = false;
        defendPose = false;

        numberOfTargets = typeOfTarget.SingleAllyTarget;
        buffedStat = modifiedStat.Speed;
        nerfedStat = modifiedStat.NONE;
    }
}
