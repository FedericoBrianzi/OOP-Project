using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseClass : MonoBehaviour
{
    [field: SerializeField] public int health { get; private set; }
    [field: SerializeField] public int mana { get; private set; }
    [field: SerializeField] public int attack { get; private set; }
    [field: SerializeField] public int armor { get; private set; }
    [field: SerializeField] public int speed { get; private set; }
    [field: SerializeField] public int evasion { get; private set; }
    [field: SerializeField] public string description { get; private set; }
    [field: SerializeField] public List<string> attacks { get; private set; } = new List<string>();

    protected bool isDefending = false;

    protected virtual void Attack(GameObject target)
    {
        target.GetComponent<BaseClass>().TakeDamage(attack, false);
    }

    public virtual void TakeDamage(int damage, bool ignoreArmor)
    {
        if(Random.Range(0, 100) < evasion)
        {
            return;
            Debug.Log(gameObject + "dodged the attack.");
        }

        if (ignoreArmor)
        {
            if(isDefending) health -= damage / 2;
            else health -= damage;
        }
        else
        {
            if(isDefending) health -= (damage - armor) / 2;
            else health -= (damage - armor);
        }
    }
}
