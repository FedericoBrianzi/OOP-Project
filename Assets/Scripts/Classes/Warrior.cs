using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warrior : BaseClass
{
    public void Slash(GameObject target)
    {
        target.GetComponent<BaseClass>().TakeDamage(Mathf.RoundToInt(attack * 1.5f), false);
    }

    public void Defend()
    {
        isDefending = true;
    }
}
