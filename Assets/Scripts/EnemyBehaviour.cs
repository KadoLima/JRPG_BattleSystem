using System;
using System.Collections;
using UnityEngine;

public class EnemyBehaviour : CharacterBehaviour
{
    [Header("ENEMY SPECIFIC PARAMETERS")]
    [SerializeField] float minAttackRate = 6;
    [SerializeField] float maxattackRate = 8;
    [SerializeField] int xpRewarded;
    [SerializeField] float chanceToUseSkill;

    int defaultSortingOrder;
    float waitTime;
    float randomizedInitialDelay;
    CharacterBehaviour currentPlayerTarget;
    SpriteRenderer spriteRenderer;

    public float ChanceToUseSkill => chanceToUseSkill;
    public CharacterBehaviour CurrentPlayerTarget => currentPlayerTarget;

    public static Action<string> OnEnemyUsedSkill;

    public override void Start()
    {
        Initialize();
    }

    protected override void Initialize()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        defaultSortingOrder = spriteRenderer.sortingOrder;

        currentPreAction = ScriptableObject.CreateInstance<CombatActionSO>();
        currentExecutingAction = ScriptableObject.CreateInstance<CombatActionSO>();

        characterUIController = GetComponentInChildren<CharacterUIController>();
        CombatManager.instance.EnemiesOnField.Add(this);
        originalPosition = transform.localPosition;
        currentHP = myStats.baseHP;

        transform.SetParent(CombatManager.instance.EnemiesParent);

        if (GameManager.instance.EnemiesWontAttack)
            return;

        ExecuteActionOn();
    }


    public void SetRandomTarget()
    {
        if (CombatManager.instance.AllPlayersDead())
            return;

        int _randomPlayerIndex = UnityEngine.Random.Range(0, CombatManager.instance.PlayersOnField.Count);

        while (CombatManager.instance.PlayersOnField[_randomPlayerIndex].CurrentBattlePhase == BattleState.DEAD)
        {
            _randomPlayerIndex = UnityEngine.Random.Range(0, CombatManager.instance.PlayersOnField.Count);
        }

        
        SetTarget(CombatManager.instance.PlayersOnField[_randomPlayerIndex]);
    }

    public void SetTarget(CharacterBehaviour target)
    {
        currentPlayerTarget = target;
    }

    public override void ExecuteActionOn(CharacterBehaviour target=null)
    {
        StartCoroutine(AttackRandomPlayerCoroutine(target));
    }

    IEnumerator AttackRandomPlayerCoroutine(CharacterBehaviour target)
    {
        yield return new WaitUntil(() => GameManager.instance.GameStarted);

        randomizedInitialDelay = UnityEngine.Random.Range(4 - .1f, 4 + .1f);

        yield return new WaitForSeconds(.1f);
        yield return new WaitForSeconds(randomizedInitialDelay);

        while (CurrentBattlePhase != BattleState.DEAD)
        {

            if (currentPlayerTarget == null)
                SetRandomTarget();
            else SetTarget(target);

            yield return new WaitForSeconds(.1f); //syncing

            CombatManager.instance.AddToCombatQueue(this);

            ChangeBattleState(BattleState.WAITING);

            yield return new WaitUntil(() => CombatManager.instance.IsFieldClear() &&
                                             CombatManager.instance.IsMyTurn(this) &&
                                             currentPlayerTarget != null);


            if (CombatManager.instance.AllPlayersDead())
            {
                ChangeBattleState(BattleState.WAITING);
                yield break;
            }

            ChangeBattleState(BattleState.EXECUTING_ACTION);

            SetRandomAction();

            yield return new WaitUntil(() => currentExecutingAction.actionType != ActionType.RECHARGING);

            if (currentPlayerTarget.CurrentBattlePhase == BattleState.DEAD)
                SetRandomTarget();


            if (currentExecutingAction.goToTarget)
            {
                MoveToTarget(currentPlayerTarget);

                if (currentExecutingAction.actionType == ActionType.SKILL)
                {
                    OnEnemyUsedSkill?.Invoke(currentExecutingAction.actionName);
                }

                else if (currentExecutingAction.actionType == ActionType.NORMAL_ATTACK)
                {
                    var _rndValue = UnityEngine.Random.value;
                    isDoingCritDamageAction = _rndValue > myStats.critChance ? false : true;
                }

                yield return new WaitForSeconds(myAnimController.SecondsToReachTarget);
                myAnimController.PlayAnimation(currentExecutingAction.animationCycle.name);
                spriteRenderer.sortingOrder = currentPlayerTarget.GetComponentInChildren<SpriteRenderer>().sortingOrder + 1;

                yield return new WaitForSeconds(currentExecutingAction.animationCycle.cycleTime - 0.25f);

                StartCoroutine(ApplyDamageOrHeal(currentPlayerTarget));

                yield return new WaitForSeconds(0.25f);

                if (currentExecutingAction.goToTarget)
                {
                    spriteRenderer.sortingOrder = defaultSortingOrder;
                    GoBackToStartingPosition();
                }

                OnSkillEnded?.Invoke();

                yield return new WaitForSeconds(.2f);
                myAnimController.PlayAnimation(myAnimController.IdleAnimationName);

                isDoingCritDamageAction = false;
                CombatManager.instance.RemoveFromCombatQueue(this);
                ChangeBattleState(BattleState.RECHARGING);
                currentPlayerTarget = null;
            }

            waitTime = UnityEngine.Random.Range(minAttackRate, maxattackRate);

            yield return new WaitForSeconds(.1f); //sync

            yield return new WaitForSeconds(waitTime);
        }
    }

    public override void TakeDamageOrHeal(int amount, DamageType dmgType, bool isCrit)
    {
        if (dmgType == DamageType.HARMFUL)
        {
            currentHP -= amount;
            characterUIController.ShowFloatingDamageText(amount, dmgType, isCrit);

            if (currentHP <= 0)
            {
                CombatManager.instance.AddToTotalXP(xpRewarded);
                ChangeBattleState(BattleState.DEAD);
            }
        }

        characterUIController.RefreshHP(currentHP, myStats.baseHP);
    }

    private void SetRandomAction(string s = null)
    {
        float _randomValue = UnityEngine.Random.value;

        if (_randomValue > chanceToUseSkill)
            currentExecutingAction = SelectAction(ActionType.NORMAL_ATTACK);
        else
            currentExecutingAction = SelectAction(ActionType.SKILL);
    }
}
