using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class BaseClass : MonoBehaviour
{
    [field: SerializeField] public string unitName { get; private set; }    
    [field: SerializeField] public int maxHealth { get; private set; }  
    private int currentHealth;
    [field: SerializeField] public int maxMana { get; private set; }
    private int currentMana;
    [field: SerializeField] public int attack { get; private set; }
    private int currentAttack;
    [field: SerializeField] public int armor { get; private set; }
    private int currentArmor;
    [field: SerializeField] public int speed { get; private set; }
    private int currentSpeed;
    [field: SerializeField] public int evasion { get; private set; }
    private int currentEvasion;
    [field: SerializeField] public string description { get; private set; }
    [field: SerializeField] public List<BaseAttack> attacks { get; private set; } = new List<BaseAttack>();
    [field: SerializeField] public BaseAttack confusedAttack { get; private set; }
    [field: SerializeField] public GameObject provokerGO { get; private set; }
    [field: SerializeField] public GameObject indicator { get; private set; }

    private int attackStatChange, armorStatChange, speedStatChange, evasionStatChange;
    private int maxStatBuff = 30;
    private int maxStatNerf = -30;

    private HandleUI uiHandler;

    protected bool isDefending = false;

    private bool dodgedTheAttack = false;
    private bool isAlive = true;
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
                if (!dodgedTheAttack && isAlive)
                {
                    yield return StartCoroutine(ChangeStat(attack.buffedStat, attack.nerfedStat, attack.statBuffQuantity, attack.statNerfQuantity));
                }
                break;
            case BaseAttack.typeOfAttack.Status:
                yield return StartCoroutine(ApplyStatusEffect(attack.statusChange, attack.statusSuccessRate, attacker));
                break;
            case BaseAttack.typeOfAttack.DmgAndStatus:
                yield return StartCoroutine(TakeDamage(totalDamage, attack.ignoreArmor));
                if (!dodgedTheAttack && isAlive)
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
                if(damageTaken <= 0)
                {
                    damageTaken = 0;
                }
                yield return StartCoroutine(uiHandler.ShowDamageTakenDescription(damageTaken));
                currentHealth -= damageTaken;
                if (currentHealth <= 0)
                {
                    currentHealth = 0;
                    uiHandler.UpdateBattleUI();
                    if (gameObject.CompareTag("Player"))
                    {
                        yield return StartCoroutine(GetComponent<PlayerStateMachine>().DeadPlayerRoutine());
                        yield return StartCoroutine(uiHandler.ShowDeadUnitDescription(this));
                        GetComponent<PlayerStateMachine>().currentState = PlayerStateMachine.TurnState.DEAD;
                        isAlive = false;
                    }
                    else
                    {
                        yield return StartCoroutine(GetComponent<EnemyStateMachine>().DeadEnemyRoutine());
                        yield return StartCoroutine(uiHandler.ShowDeadUnitDescription(this));
                        GetComponent<EnemyStateMachine>().currentState = EnemyStateMachine.TurnState.DEAD;
                        isAlive = false;
                    }
                }
            }
            else
            {
                damageTaken = damage;
                if (damageTaken <= 0)
                {
                    damageTaken = 0;
                }
                yield return StartCoroutine(uiHandler.ShowDamageTakenDescription(damageTaken));
                currentHealth -= damageTaken;
                if (currentHealth <= 0)
                {
                    currentHealth = 0;
                    uiHandler.UpdateBattleUI();
                    if (gameObject.CompareTag("Player"))
                    {
                        yield return StartCoroutine(GetComponent<PlayerStateMachine>().DeadPlayerRoutine());
                        yield return StartCoroutine(uiHandler.ShowDeadUnitDescription(this));
                        GetComponent<PlayerStateMachine>().currentState = PlayerStateMachine.TurnState.DEAD;
                        isAlive = false;
                    }
                    else
                    {
                        yield return StartCoroutine(GetComponent<EnemyStateMachine>().DeadEnemyRoutine());
                        yield return StartCoroutine(uiHandler.ShowDeadUnitDescription(this));
                        GetComponent<EnemyStateMachine>().currentState = EnemyStateMachine.TurnState.DEAD;
                        isAlive = false;
                    }
                }
            }
        }
        else
        {
            if (isDefending) 
            {
                damageTaken = (damage - currentArmor) / 2;
                if (damageTaken <= 0)
                {
                    damageTaken = 0;
                }
                yield return StartCoroutine(uiHandler.ShowDamageTakenDescription(damageTaken));
                currentHealth -= damageTaken;
                if (currentHealth <= 0)
                {
                    currentHealth = 0;
                    uiHandler.UpdateBattleUI();
                    if (gameObject.CompareTag("Player"))
                    {
                        yield return StartCoroutine(GetComponent<PlayerStateMachine>().DeadPlayerRoutine());
                        yield return StartCoroutine(uiHandler.ShowDeadUnitDescription(this));
                        GetComponent<PlayerStateMachine>().currentState = PlayerStateMachine.TurnState.DEAD;
                        isAlive = false;
                    }
                    else
                    {
                        yield return StartCoroutine(GetComponent<EnemyStateMachine>().DeadEnemyRoutine());
                        yield return StartCoroutine(uiHandler.ShowDeadUnitDescription(this));
                        GetComponent<EnemyStateMachine>().currentState = EnemyStateMachine.TurnState.DEAD;
                        isAlive = false;
                    } 
                       
                }
            }
            else
            {
                damageTaken = (damage - currentArmor);
                if (damageTaken <= 0)
                {
                    damageTaken = 0;
                }
                yield return StartCoroutine(uiHandler.ShowDamageTakenDescription(damageTaken));
                currentHealth -= damageTaken;
                if (currentHealth <= 0)
                {
                    currentHealth = 0;
                    uiHandler.UpdateBattleUI();
                    if (gameObject.CompareTag("Player"))
                    {
                        yield return StartCoroutine(GetComponent<PlayerStateMachine>().DeadPlayerRoutine());
                        yield return StartCoroutine(uiHandler.ShowDeadUnitDescription(this));
                        GetComponent<PlayerStateMachine>().currentState = PlayerStateMachine.TurnState.DEAD;
                        isAlive = false;
                    }
                    else
                    {
                        yield return StartCoroutine(GetComponent<EnemyStateMachine>().DeadEnemyRoutine());
                        yield return StartCoroutine(uiHandler.ShowDeadUnitDescription(this));
                        GetComponent<EnemyStateMachine>().currentState = EnemyStateMachine.TurnState.DEAD;
                        isAlive = false;
                    }
                }
            }   
        }
    }

    public virtual IEnumerator ChangeStat(BaseAttack.modifiedStat statToBuff, BaseAttack.modifiedStat statToNerf, int buff, int nerf)
    {
        switch (statToBuff)
        {
            case BaseAttack.modifiedStat.Attack:
                attackStatChange += buff;
                if(attackStatChange <= maxStatBuff)
                {
                    currentAttack = attack + attackStatChange;
                    yield return StartCoroutine(uiHandler.ShowStatChangeDescription(BaseAttack.modifiedStat.Attack, buff, true));
                }
                else
                {
                    attackStatChange = attack + maxStatBuff - currentAttack;
                    currentAttack += attackStatChange;
                    yield return StartCoroutine(uiHandler.ShowStatChangeDescription(BaseAttack.modifiedStat.Attack, attackStatChange, true));
                    attackStatChange = maxStatBuff;
                }
                break;
            case BaseAttack.modifiedStat.Armor:
                armorStatChange += buff;
                if (armorStatChange <= maxStatBuff)
                {
                    currentArmor = armor + armorStatChange;
                    yield return StartCoroutine(uiHandler.ShowStatChangeDescription(BaseAttack.modifiedStat.Armor, buff, true));
                }
                else
                {
                    armorStatChange = armor + maxStatBuff - currentArmor;
                    currentArmor += armorStatChange;
                    yield return StartCoroutine(uiHandler.ShowStatChangeDescription(BaseAttack.modifiedStat.Armor, armorStatChange, true));
                    armorStatChange = maxStatBuff;
                }
                break;
            case BaseAttack.modifiedStat.Speed:
                speedStatChange += buff;
                if (speedStatChange <= maxStatBuff)
                {
                    currentSpeed = speed + speedStatChange;
                    yield return StartCoroutine(uiHandler.ShowStatChangeDescription(BaseAttack.modifiedStat.Speed, buff, true));
                }
                else
                {
                    speedStatChange = speed + maxStatBuff - currentSpeed;
                    currentSpeed += speedStatChange;
                    yield return StartCoroutine(uiHandler.ShowStatChangeDescription(BaseAttack.modifiedStat.Speed, speedStatChange, true));
                    speedStatChange = maxStatBuff;
                }
                break;
            case BaseAttack.modifiedStat.Evasion:
                evasionStatChange += buff;
                if(evasionStatChange <= maxStatBuff)
                {
                    currentEvasion = evasion + evasionStatChange;
                    yield return StartCoroutine(uiHandler.ShowStatChangeDescription(BaseAttack.modifiedStat.Evasion, buff, true));
                }
                else
                {
                    evasionStatChange = evasion + maxStatBuff - currentEvasion;
                    currentEvasion += evasionStatChange;
                    yield return StartCoroutine(uiHandler.ShowStatChangeDescription(BaseAttack.modifiedStat.Evasion, evasionStatChange, true));
                    evasionStatChange = maxStatBuff;
                }
                break;
            case BaseAttack.modifiedStat.NONE:
                break;
        }

        switch (statToNerf)
        {
            case BaseAttack.modifiedStat.Attack:
                attackStatChange -= nerf;
                if (attackStatChange >= maxStatNerf)
                {
                    currentAttack = attack + attackStatChange;
                    yield return StartCoroutine(uiHandler.ShowStatChangeDescription(BaseAttack.modifiedStat.Attack, nerf, false));
                }
                else
                {
                    attackStatChange = attack + maxStatNerf - currentAttack;
                    currentAttack += attackStatChange;
                    yield return StartCoroutine(uiHandler.ShowStatChangeDescription(BaseAttack.modifiedStat.Attack, Mathf.Abs(attackStatChange), false));
                    attackStatChange = maxStatNerf;
                }
                break;
            case BaseAttack.modifiedStat.Armor:
                armorStatChange -= nerf;
                if (armorStatChange >= maxStatNerf)
                {
                    currentArmor = armor + armorStatChange;
                    yield return StartCoroutine(uiHandler.ShowStatChangeDescription(BaseAttack.modifiedStat.Armor, nerf, false));
                }
                else
                {
                    armorStatChange = armor + maxStatNerf - currentArmor;
                    currentArmor += armorStatChange;
                    yield return StartCoroutine(uiHandler.ShowStatChangeDescription(BaseAttack.modifiedStat.Armor, Mathf.Abs(armorStatChange), false));
                    armorStatChange = maxStatNerf;
                }
                break;
            case BaseAttack.modifiedStat.Speed:
                speedStatChange -= nerf;
                if (speedStatChange >= maxStatNerf)
                {
                    currentSpeed = speed + speedStatChange;
                    yield return StartCoroutine(uiHandler.ShowStatChangeDescription(BaseAttack.modifiedStat.Speed, nerf, false));
                }
                else
                {
                    speedStatChange = speed + maxStatNerf - currentSpeed;
                    currentSpeed += speedStatChange;
                    yield return StartCoroutine(uiHandler.ShowStatChangeDescription(BaseAttack.modifiedStat.Speed, Mathf.Abs(speedStatChange), false));
                    speedStatChange = maxStatNerf;
                }
                break;
            case BaseAttack.modifiedStat.Evasion:
                evasionStatChange -= nerf;
                if (evasionStatChange >= maxStatNerf)
                {
                    currentEvasion = evasion + evasionStatChange;
                    yield return StartCoroutine(uiHandler.ShowStatChangeDescription(BaseAttack.modifiedStat.Evasion, nerf, false));
                }
                else
                {
                    evasionStatChange = evasion + maxStatNerf - currentEvasion;
                    currentEvasion += evasionStatChange;
                    yield return StartCoroutine(uiHandler.ShowStatChangeDescription(BaseAttack.modifiedStat.Evasion, Mathf.Abs(evasionStatChange), false));
                    evasionStatChange = maxStatNerf;
                }
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
                yield return StartCoroutine(uiHandler.ShowStatusDamageDescription(StatusEffect.Burned, Mathf.RoundToInt(maxHealth * 10 / 100), this));
                currentHealth -= Mathf.RoundToInt(maxHealth * 10 / 100);
                if (currentHealth < 0)
                {
                    currentHealth = 0;
                    uiHandler.UpdateBattleUI();
                    if (gameObject.CompareTag("Player"))
                    {
                        yield return StartCoroutine(GetComponent<PlayerStateMachine>().DeadPlayerRoutine());
                        GetComponent<PlayerStateMachine>().currentState = PlayerStateMachine.TurnState.DEAD;
                        isAlive = false;
                    }
                    else
                    {
                        yield return StartCoroutine(GetComponent<EnemyStateMachine>().DeadEnemyRoutine());
                        GetComponent<EnemyStateMachine>().currentState = EnemyStateMachine.TurnState.DEAD;
                        isAlive = false;
                    }
                }
                uiHandler.UpdateBattleUI();
                break;
            case StatusEffect.Poisoned:
                yield return StartCoroutine(uiHandler.ShowStatusDamageDescription(StatusEffect.Poisoned, Mathf.RoundToInt(maxHealth * 15 / 100), this));
                currentHealth -= Mathf.RoundToInt(maxHealth * 15 / 100);
                if (currentHealth < 0)
                {
                    currentHealth = 0;
                    uiHandler.UpdateBattleUI();
                    if (gameObject.CompareTag("Player"))
                    {
                        yield return StartCoroutine(GetComponent<PlayerStateMachine>().DeadPlayerRoutine());
                        GetComponent<PlayerStateMachine>().currentState = PlayerStateMachine.TurnState.DEAD;
                        isAlive = false;
                    }
                    else
                    {
                        yield return StartCoroutine(GetComponent<EnemyStateMachine>().DeadEnemyRoutine());
                        GetComponent<EnemyStateMachine>().currentState = EnemyStateMachine.TurnState.DEAD;
                        isAlive = false;
                    }
                }
                uiHandler.UpdateBattleUI();
                break;
        }
    }

    #region GetCurrentStatsMethods
    public int GetCurrentHealth()
    {
        return currentHealth;
    }   //ENCAPSULATION

    public int GetCurrentMana()
    {
        return currentMana;
    }    //ENCAPSULATION

    public int GetCurrentAttack()
    {
        return currentAttack;
    }    //ENCAPSULATION

    public int GetCurrentArmor()
    {
        return currentArmor;
    }    //ENCAPSULATION

    public int GetCurrentSpeed()
    {
        return currentSpeed;
    }    //ENCAPSULATION    

    public int GetCurrentEvasion()
    {
        return currentEvasion;
    }    //ENCAPSULATION
    #endregion
}
