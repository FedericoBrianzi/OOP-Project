using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class BaseClass : MonoBehaviour
{
    [field: SerializeField] public string unitName { get; private set; }
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
    [field: SerializeField] public GameObject provokerGO { get; private set; }
    [field: SerializeField] public GameObject indicator { get; private set; }

    private HandleUI uiHandler;

    protected bool isDefending = false;

    private bool dodgedTheAttack = false;
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
        uiHandler = GameObject.Find("Canvas").GetComponent<HandleUI>();
    }

    public virtual IEnumerator EvaluateAttack(BaseAttack attack, int totalDamage, GameObject attacker)
    {
        switch (attack.attackType)
        {
            case BaseAttack.typeOfAttack.Damage:
                yield return StartCoroutine(TakeDamage(totalDamage, attack.ignoreArmor));
                if (gameObject.TryGetComponent(out Berserk berserk) && currentHealth <= Mathf.RoundToInt(maxHealth / 3))
                {
                    if (!activeStatusEffects.Contains(StatusEffect.BerserkWrath))
                    {
                        activeStatusEffects.Add(StatusEffect.BerserkWrath);
                    }
                }
                break;
            case BaseAttack.typeOfAttack.Defend:
                isDefending = true;
                break;
            case BaseAttack.typeOfAttack.StatChange:
                yield return StartCoroutine(ChangeStat(attack.buffedStat, attack.nerfedStat, attack.statBuffQuantity, attack.statNerfQuantity));
                break;
            case BaseAttack.typeOfAttack.DmgAndStatChange:
                yield return StartCoroutine(TakeDamage(totalDamage, attack.ignoreArmor));
                if (!dodgedTheAttack)
                {
                    yield return StartCoroutine(ChangeStat(attack.buffedStat, attack.nerfedStat, attack.statBuffQuantity, attack.statNerfQuantity));
                }
                break;
            case BaseAttack.typeOfAttack.Status:
                yield return StartCoroutine(ApplyStatusEffect(attack.statusChange, attack.statusSuccessRate, attacker));
                break;
            case BaseAttack.typeOfAttack.DmgAndStatus:
                yield return StartCoroutine(TakeDamage(totalDamage, attack.ignoreArmor));
                if (!dodgedTheAttack)
                {
                    yield return StartCoroutine(ApplyStatusEffect(attack.statusChange, attack.statusSuccessRate, attacker));
                }
                break;
        }
    }

    public virtual IEnumerator TakeDamage(int damage, bool ignoreArmor)
    {
        if (Random.Range(0, 100) < currentEvasion)
        {
            dodgedTheAttack = true;
            yield return StartCoroutine(uiHandler.ShowDamageTakenDescription(dodgedTheAttack));
            yield break;
        }
        else dodgedTheAttack = false;

        int damageTaken;
        if (ignoreArmor)
        {
            if (isDefending)
            {
                damageTaken = damage / 2;
                currentHealth -= damageTaken;
                yield return StartCoroutine(uiHandler.ShowDamageTakenDescription(damageTaken));
                if (currentHealth < 0)
                {
                    currentHealth = 0;
                    if (gameObject.CompareTag("Player"))
                    {
                        yield return StartCoroutine(GetComponent<PlayerStateMachine>().DeadPlayerRoutine());
                        GetComponent<PlayerStateMachine>().currentState = PlayerStateMachine.TurnState.DEAD;
                    }
                    else
                    {
                        yield return StartCoroutine(GetComponent<EnemyStateMachine>().DeadEnemyRoutine());
                        GetComponent<EnemyStateMachine>().currentState = EnemyStateMachine.TurnState.DEAD;
                    }
                }
            }
            else
            {
                damageTaken = damage;
                currentHealth -= damageTaken;
                yield return StartCoroutine(uiHandler.ShowDamageTakenDescription(damageTaken));
                if (currentHealth < 0)
                {
                    currentHealth = 0;
                    if (gameObject.CompareTag("Player"))
                    {
                        yield return StartCoroutine(GetComponent<PlayerStateMachine>().DeadPlayerRoutine());
                        GetComponent<PlayerStateMachine>().currentState = PlayerStateMachine.TurnState.DEAD;
                    }
                    else
                    {
                        yield return StartCoroutine(GetComponent<EnemyStateMachine>().DeadEnemyRoutine());
                        GetComponent<EnemyStateMachine>().currentState = EnemyStateMachine.TurnState.DEAD;
                    }
                }
            }
        }
        else
        {
            if (isDefending) 
            {
                damageTaken = (damage - currentArmor) / 2;
                currentHealth -= damageTaken;
                yield return StartCoroutine(uiHandler.ShowDamageTakenDescription(damageTaken));
                if (currentHealth < 0)
                {
                    currentHealth = 0;
                    if (gameObject.CompareTag("Player"))
                    {
                        yield return StartCoroutine(GetComponent<PlayerStateMachine>().DeadPlayerRoutine());
                        GetComponent<PlayerStateMachine>().currentState = PlayerStateMachine.TurnState.DEAD;
                    }
                    else
                    {
                        yield return StartCoroutine(GetComponent<EnemyStateMachine>().DeadEnemyRoutine());
                        GetComponent<EnemyStateMachine>().currentState = EnemyStateMachine.TurnState.DEAD;
                    } 
                       
                }
            }
            else
            {
                damageTaken = (damage - currentArmor);
                currentHealth -= damageTaken;
                yield return StartCoroutine(uiHandler.ShowDamageTakenDescription(damageTaken));
                if (currentHealth < 0)
                {
                    currentHealth = 0;
                    if (gameObject.CompareTag("Player"))
                    {
                        yield return StartCoroutine(GetComponent<PlayerStateMachine>().DeadPlayerRoutine());
                        GetComponent<PlayerStateMachine>().currentState = PlayerStateMachine.TurnState.DEAD;
                    }
                    else
                    {
                        yield return StartCoroutine(GetComponent<EnemyStateMachine>().DeadEnemyRoutine());
                        GetComponent<EnemyStateMachine>().currentState = EnemyStateMachine.TurnState.DEAD;
                    }
                }
            }   
        }
        
    }

    public virtual IEnumerator ChangeStat(BaseAttack.modifiedStat statToBuff, BaseAttack.modifiedStat statToNerf, int buff, int nerf)
    {
        switch (statToBuff)
        {
            //case BaseAttack.modifiedStat.Health:  ///idk any spell that removes currentHealth but its not a damaging attack
            //    currentHealth += buff;
            //    yield return StartCoroutine(uiHandler.ShowStatChangeDescription(BaseAttack.modifiedStat.Health, buff, true));
            //    break;
            case BaseAttack.modifiedStat.Mana:
                currentMana += buff;
                yield return StartCoroutine(uiHandler.ShowStatChangeDescription(BaseAttack.modifiedStat.Mana, buff, true));
                break;
            case BaseAttack.modifiedStat.Attack:
                currentAttack += buff;
                yield return StartCoroutine(uiHandler.ShowStatChangeDescription(BaseAttack.modifiedStat.Attack, buff, true));
                break;
            case BaseAttack.modifiedStat.Armor:
                currentArmor += buff;
                yield return StartCoroutine(uiHandler.ShowStatChangeDescription(BaseAttack.modifiedStat.Armor, buff, true));
                break;
            case BaseAttack.modifiedStat.Speed:
                currentSpeed += buff;
                yield return StartCoroutine(uiHandler.ShowStatChangeDescription(BaseAttack.modifiedStat.Speed, buff, true));
                break;
            case BaseAttack.modifiedStat.Evasion:
                currentEvasion += buff;
                yield return StartCoroutine(uiHandler.ShowStatChangeDescription(BaseAttack.modifiedStat.Evasion, buff, true));
                break;
            case BaseAttack.modifiedStat.NONE:
                break;
        }

        switch (statToNerf)
        {
            //case BaseAttack.modifiedStat.Health:  
            //    currentHealth -= nerf;
            //    yield return StartCoroutine(uiHandler.ShowStatChangeDescription(BaseAttack.modifiedStat.Health, nerf, false));
            //    break;
            case BaseAttack.modifiedStat.Mana:
                currentMana -= nerf;
                if (currentMana < 0)
                {
                    currentMana = 0;
                }
                yield return StartCoroutine(uiHandler.ShowStatChangeDescription(BaseAttack.modifiedStat.Mana, nerf, false));
                break;
            case BaseAttack.modifiedStat.Attack:
                currentAttack -= nerf;
                if (currentAttack < 0)
                {
                    currentAttack = 0;
                }
                yield return StartCoroutine(uiHandler.ShowStatChangeDescription(BaseAttack.modifiedStat.Attack, nerf, false));
                break;
            case BaseAttack.modifiedStat.Armor:
                currentArmor -= nerf;
                if (currentArmor < 0)
                {
                    currentArmor = 0;
                }
                yield return StartCoroutine(uiHandler.ShowStatChangeDescription(BaseAttack.modifiedStat.Armor, nerf, false));
                break;
            case BaseAttack.modifiedStat.Speed:
                currentSpeed -= nerf;
                if (currentSpeed < 0)
                {
                    currentSpeed = 0;
                }
                yield return StartCoroutine(uiHandler.ShowStatChangeDescription(BaseAttack.modifiedStat.Speed, nerf, false));
                break;
            case BaseAttack.modifiedStat.Evasion:
                currentEvasion -= nerf;
                if (currentEvasion < 0)
                {
                    currentEvasion = 0;
                }
                yield return StartCoroutine(uiHandler.ShowStatChangeDescription(BaseAttack.modifiedStat.Evasion, nerf, false));
                break;
            case BaseAttack.modifiedStat.NONE:
                break;
        }
    }

    public virtual IEnumerator ApplyStatusEffect(BaseAttack.StatusEffect status, int probability, GameObject attacker)
    {
        switch (status)
        {
            case BaseAttack.StatusEffect.Burn:
                if (!activeStatusEffects.Contains(StatusEffect.Burned) && Random.Range(0, 100) < probability) ///you could also make it works like the second status effect refreshes the status effect duration
                {
                    activeStatusEffects.Add(StatusEffect.Burned);
                    yield return StartCoroutine(uiHandler.ShowStatusTakenDescription(StatusEffect.Burned));
                }
                break;
            case BaseAttack.StatusEffect.Poison:
                if (!activeStatusEffects.Contains(StatusEffect.Poisoned) && Random.Range(0, 100) < probability)
                {
                    activeStatusEffects.Add(StatusEffect.Poisoned);
                    yield return StartCoroutine(uiHandler.ShowStatusTakenDescription(StatusEffect.Poisoned));
                }
                break;
            case BaseAttack.StatusEffect.Confuse:
                if (!activeStatusEffects.Contains(StatusEffect.Confused) && !activeStatusEffects.Contains(StatusEffect.Asleep) && Random.Range(0, 100) < probability)
                {
                    activeStatusEffects.Add(StatusEffect.Confused);
                    yield return StartCoroutine(uiHandler.ShowStatusTakenDescription(StatusEffect.Confused));
                }
                break;
            case BaseAttack.StatusEffect.Paralyze:
                if (!activeStatusEffects.Contains(StatusEffect.Paralyzed) && Random.Range(0, 100) < probability)
                {
                    activeStatusEffects.Add(StatusEffect.Paralyzed);
                    yield return StartCoroutine(uiHandler.ShowStatusTakenDescription(StatusEffect.Paralyzed));
                }
                break;
            case BaseAttack.StatusEffect.Provoke:
                if (!activeStatusEffects.Contains(StatusEffect.Provoked) && !activeStatusEffects.Contains(StatusEffect.Asleep) && Random.Range(0, 100) < probability) //provoke puo essere asovrascritto ad un altro provoke?
                {
                    activeStatusEffects.Add(StatusEffect.Provoked);
                    provokerGO = attacker;
                    yield return StartCoroutine(uiHandler.ShowStatusTakenDescription(StatusEffect.Provoked, provokerGO));
                }
                else if (activeStatusEffects.Contains(StatusEffect.Provoked) && !activeStatusEffects.Contains(StatusEffect.Asleep) && Random.Range(0, 100) < probability)
                {
                    activeStatusEffects.Remove(StatusEffect.Provoked); ///overwrites the target to the new provoker
                    activeStatusEffects.Add(StatusEffect.Provoked);
                    provokerGO = attacker;
                    yield return StartCoroutine(uiHandler.ShowStatusTakenDescription(StatusEffect.Provoked, provokerGO));
                }
                break;
            case BaseAttack.StatusEffect.Sleep:
                if (!activeStatusEffects.Contains(StatusEffect.Asleep) && Random.Range(0, 100) < probability)
                {
                    activeStatusEffects.Add(StatusEffect.Asleep);
                    yield return StartCoroutine(uiHandler.ShowStatusTakenDescription(StatusEffect.Asleep));
                    if (activeStatusEffects.Contains(StatusEffect.Provoked))
                    {
                        activeStatusEffects.Remove(StatusEffect.Provoked);
                        yield return StartCoroutine(uiHandler.ShowStatusRemovedDescription(StatusEffect.Provoked));
                    }
                    if (activeStatusEffects.Contains(StatusEffect.Confused))
                    {
                        activeStatusEffects.Remove(StatusEffect.Confused);
                        yield return StartCoroutine(uiHandler.ShowStatusRemovedDescription(StatusEffect.Confused));
                    }
                }
                break;
            case BaseAttack.StatusEffect.Stun:
                if (Random.Range(0, 100) < probability)
                {
                    activeStatusEffects.Add(StatusEffect.Stunned);
                    yield return StartCoroutine(uiHandler.ShowStatusTakenDescription(StatusEffect.Stunned));
                }
                break;
        }
    }

    public virtual void SubtractMana(int manaCost)
    {
        currentMana -= manaCost;
    }

    public virtual IEnumerator TakeStatusEffectDamage(StatusEffect status)
    {
        switch (status)
        {
            case StatusEffect.Burned:
                currentHealth -= Mathf.RoundToInt(maxHealth * 10 / 100);
                yield return StartCoroutine(uiHandler.ShowStatusDamageDescription(StatusEffect.Burned, Mathf.RoundToInt(maxHealth * 10 / 100), this));
                break;
            case StatusEffect.Poisoned:
                currentHealth -= Mathf.RoundToInt(maxHealth * 15 / 100);
                yield return StartCoroutine(uiHandler.ShowStatusDamageDescription(StatusEffect.Poisoned, Mathf.RoundToInt(maxHealth * 15 / 100), this));
                break;
        }
    }
}
