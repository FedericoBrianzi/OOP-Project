using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class BaseClass : MonoBehaviour
{
    [field: SerializeField] public string name { get; private set; }
    [field: SerializeField] public int maxHealth { get; private set; }
    public int currentHealth { get; private set; }
    [field: SerializeField] public int maxMana { get; private set; }
    public int currentMana { get; private set; }
    [field: SerializeField] public int attack { get; private set; }
    public int currentAttack { get; private set; }
    [field: SerializeField] public int armor { get; private set; }
    public int currentArmor { get; private set; }
    [field: SerializeField] public int speed { get; private set; }
    public int currentSpeed { get; private set; }
    [field: SerializeField] public int evasion { get; private set; }
    public int currentEvasion { get; private set; }
    [field: SerializeField] public string description { get; private set; }
    [field: SerializeField] public List<BaseAttack> attacks { get; private set; } = new List<BaseAttack>();

    protected bool isDefending = false;

    public bool myTurn = false;

    protected void Start()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
        currentAttack = attack;
        currentArmor = armor;
        currentSpeed = speed;
        currentEvasion = evasion;
    }

    public virtual void TakeDamage(int damage, bool ignoreArmor)
    {
        if(Random.Range(0, 100) < currentEvasion)
        {
            Debug.Log(gameObject + "dodged the attack.");
            return;     
        }

        if (ignoreArmor)
        {
            if(isDefending) currentHealth -= damage / 2;
            else currentHealth -= damage;
        }
        else
        {
            if(isDefending) currentHealth -= (damage - currentArmor) / 2;
            else currentHealth -= (damage - currentArmor);
        }
    }

    public virtual void EvaluateAttack(BaseAttack attack)
    {
        switch (attack.attackType)
        {
            case BaseAttack.typeOfAttack.Damage:
                TakeDamage(attack.attackDamage, attack.ignoreArmor);
                break;
            case BaseAttack.typeOfAttack.Defend:
                isDefending = true; //vorrei che chi si difende fosse sempre il primo a eseguire l'azione
                break;
            case BaseAttack.typeOfAttack.StatChange:
                break;
        }
    }

    public virtual void SubtractMana(int manaCost)
    {
        currentMana -= manaCost;
    }
}
