using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : CharacterBehaviour
{
    [Header("ENEMY SPECIFIC PARAMETERS")]
    [SerializeField] float initialDelay;
    [SerializeField] float minAttackRate = 6;
    [SerializeField] float maxattackRate = 8;
    [SerializeField] int xpRewarded;
    [SerializeField] float chanceToUseSkill;
    public float ChanceToUseSkill => chanceToUseSkill;
    CharacterBehaviour currentPlayerTarget;
    public CharacterBehaviour CurrentPlayerTarget => currentPlayerTarget;

    public static Action<string> OnEnemyUsedSkill; 

    public override void Start()
    {
        uiController = GetComponentInChildren<CharacterUIController>();
        CombatManager.instance.enemiesOnField.Add(this);
        originalPosition = transform.localPosition;
        currentHP = myStats.baseHP;

        if (GameManager.instance.Debug_EnemiesDontAttack)
            return;

        ExecuteActionOn();
    }



    public CharacterBehaviour GetRandomPlayer()
    {
        int _randomPlayerIndex = UnityEngine.Random.Range(0, CombatManager.instance.playersOnField.Count);

        while (CombatManager.instance.playersOnField[_randomPlayerIndex].CurrentBattlePhase == BattleState.DEAD)
        {
            _randomPlayerIndex = UnityEngine.Random.Range(0, CombatManager.instance.playersOnField.Count);
        }

        currentPlayerTarget = CombatManager.instance.playersOnField[_randomPlayerIndex];
        return currentPlayerTarget;
    }

    public override void ExecuteActionOn(CharacterBehaviour target=null)
    {
        StartCoroutine(AttackRandomPlayerCoroutine(target));
    }

    IEnumerator AttackRandomPlayerCoroutine(CharacterBehaviour target)
    {
        yield return new WaitUntil(() => GameManager.instance.GameStarted);

        yield return new WaitForSeconds(UnityEngine.Random.Range(initialDelay - .1f, initialDelay + .1f));

        while (CurrentBattlePhase != BattleState.DEAD)
        {
            if (currentPlayerTarget == null)
                currentPlayerTarget = GetRandomPlayer();
            else currentPlayerTarget = target;

            CombatManager.instance.AddToCombatQueue(this);

            ChangeBattleState(BattleState.WAITING);

            yield return new WaitUntil(() => CombatManager.instance.IsFieldClear() &&
                                             CombatManager.instance.IsMyTurn(this));

            ChangeBattleState(BattleState.EXECUTING_ACTION);
            SetCurrentAction();

            if (currentExecutingAction.goToTarget)
            {
                GetComponentInChildren<SpriteRenderer>().sortingOrder++;
                MoveToTarget(currentPlayerTarget);

                if (currentExecutingAction.actionType == ActionType.SKILL)
                    OnEnemyUsedSkill?.Invoke(currentExecutingAction.actionName);

                else if (currentExecutingAction.actionType == ActionType.NORMAL_ATTACK)
                {
                    var _rndValue = UnityEngine.Random.value;
                    isDoingCritDamageAction =  _rndValue > myStats.critChance ? false : true;
                }

                yield return new WaitForSeconds(secondsToReachTarget);

                PlayAnimation(currentExecutingAction.animationCycle.name);

                yield return new WaitForSeconds(currentExecutingAction.animationCycle.cycleTime - 0.25f);

                ApplyDamageOrHeal(currentPlayerTarget);

                yield return new WaitForSeconds(0.25f);

                if (currentExecutingAction.goToTarget)
                {
                    GetComponentInChildren<SpriteRenderer>().sortingOrder--;
                    GoBackToStartingPosition();
                }

                OnSkillEnded?.Invoke();

                yield return new WaitForSeconds(.2f);
                PlayAnimation(idleAnimation);

                isDoingCritDamageAction = false;
                CombatManager.instance.RemoveFromCombatQueue(this);
                ChangeBattleState(BattleState.RECHARGING);
                currentPlayerTarget = null;
            }

            yield return new WaitForSeconds(UnityEngine.Random.Range(minAttackRate, maxattackRate));
        }
    }

    public override void TakeDamageOrHeal(int amount, DamageType dmgType, bool isCrit)
    {
        if (dmgType == DamageType.HARMFUL)
        {
            currentHP -= amount;
            uiController.ShowFloatingDamageText(amount, dmgType, isCrit);

            if (currentHP <= 0)
            {
                Debug.LogWarning("ENEMY DIED!");
                CombatManager.instance.AddToTotalXP(xpRewarded);
                ChangeBattleState(BattleState.DEAD);

            }
        }

        uiController.RefreshHP(currentHP, myStats.baseHP);
    }

    private void SetCurrentAction(string s=null)
    {
        float _randomValue = UnityEngine.Random.value;

        if (_randomValue > chanceToUseSkill)
        {
            currentExecutingAction = normalAttack;
        }
        else
        {
            currentExecutingAction = Skills[0];
        }
    }
}
