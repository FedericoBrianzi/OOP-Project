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
    [field: SerializeField] public BaseAttack confusedAttack { get; private set; }

    protected bool isDefending = false;

    public bool myTurn = false;
    public enum StatusEffect
    {
        Burned,
        Poisoned,
        Stunned,
        Paralyzed,
        Provoked,
        Asleep,
        Confused,
        BerserkWrath,
        NONE
    }

    public List<StatusEffect> activeStatusEffects { get; private set; } = new List<StatusEffect>();

    protected void Start()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
        currentAttack = attack;
        currentArmor = armor;
        currentSpeed = speed;
        currentEvasion = evasion;
    }

    public virtual void EvaluateAttack(BaseAttack attack, int totalDamage)
    {
        switch (attack.attackType)
        {
            case BaseAttack.typeOfAttack.Damage:
                TakeDamage(totalDamage, attack.ignoreArmor);
                if(gameObject.TryGetComponent(out Berserk berserk) && currentHealth <= Mathf.RoundToInt(maxHealth / 3))
                {
                    activeStatusEffects.Add(StatusEffect.BerserkWrath);
                }
                break;
            case BaseAttack.typeOfAttack.Defend:
                isDefending = true; 
                break;
            case BaseAttack.typeOfAttack.StatChange:
                ChangeStat(attack.buffedStat, attack.nerfedStat, attack.statBuffQuantity, attack.statNerfQuantity);
                break;
            case BaseAttack.typeOfAttack.DmgAndStatChange:
                TakeDamage(totalDamage, attack.ignoreArmor);
                ChangeStat(attack.buffedStat, attack.nerfedStat, attack.statBuffQuantity, attack.statNerfQuantity);
                break;
            case BaseAttack.typeOfAttack.Status:
                TakeDamage(totalDamage, attack.ignoreArmor);
                break;
            case BaseAttack.typeOfAttack.DmgAndStatus:
                TakeDamage(totalDamage, attack.ignoreArmor);
                break;
        }
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

    public virtual void ChangeStat(BaseAttack.modifiedStat statToBuff, BaseAttack.modifiedStat statToNerf, int buff, int nerf)
    {
        switch (statToBuff)
        {
            case BaseAttack.modifiedStat.Health:
                currentHealth += buff;
                break;
            case BaseAttack.modifiedStat.Mana:
                currentMana += buff;
                break;
            case BaseAttack.modifiedStat.Attack:
                currentAttack += buff;
                break;
            case BaseAttack.modifiedStat.Armor:
                currentArmor += buff;
                break;
            case BaseAttack.modifiedStat.Speed:
                currentSpeed += buff;
                break;
            case BaseAttack.modifiedStat.Evasion:
                currentEvasion += buff;
                break;
            case BaseAttack.modifiedStat.NONE:
                break;
        }

        switch (statToNerf)
        {
            case BaseAttack.modifiedStat.Health:
                currentHealth += nerf;
                break;
            case BaseAttack.modifiedStat.Mana:
                currentMana += nerf;
                break;
            case BaseAttack.modifiedStat.Attack:
                currentAttack += nerf;
                break;
            case BaseAttack.modifiedStat.Armor:
                currentArmor += nerf;
                break;
            case BaseAttack.modifiedStat.Speed:
                currentSpeed += nerf;
                break;
            case BaseAttack.modifiedStat.Evasion:
                currentEvasion += nerf;
                break;
            case BaseAttack.modifiedStat.NONE:
                break;
        }
    }

    public virtual void SubtractMana(int manaCost)
    {
        currentMana -= manaCost;
    }

    public virtual void TakeStatusEffectDamage(StatusEffect status)
    {
        switch (status)
        {
            case StatusEffect.Burned:
                currentHealth -= Mathf.RoundToInt(currentHealth * 5 / 100);
                break;
            case StatusEffect.Poisoned:
                currentHealth -= Mathf.RoundToInt(currentHealth * 10 / 100);
                break;
        }
    }
}
