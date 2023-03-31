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

    // Start is called before the first frame update
    public override void Start()
    {
        uiController = GetComponentInChildren<CharacterUIController>();
        CombatManager.instance.enemiesOnField.Add(this);
        originalPosition = transform.localPosition;
        currentHP = myStats.baseHP;

        ExecuteActionOn(null);
    }



    public CharacterBehaviour GetRandomPlayer()
    {
        int _randomPlayerIndex = UnityEngine.Random.Range(0, CombatManager.instance.playersOnField.Count);
        //Debug.LogWarning("count: " + CombatManager.instance.playersOnField.Count + ", chosen is: " + _randomPlayerIndex);

        while (CombatManager.instance.playersOnField[_randomPlayerIndex].CurrentBattlePhase == BattleState.DEAD)
        {
            _randomPlayerIndex = UnityEngine.Random.Range(0, CombatManager.instance.playersOnField.Count);
        }

        currentPlayerTarget = CombatManager.instance.playersOnField[_randomPlayerIndex];
        return currentPlayerTarget;
    }

    public override void ExecuteActionOn(CharacterBehaviour target)
    {
        //Debug.LogWarning($"I'm {gameObject.name} and I'm starting action");
        StartCoroutine(AttackRandomPlayerCoroutine(target));
        //CombatManager.instance.AddToCombatQueue(this);
    }

    //public void AttackRandomPlayer()
    //{
    //    Debug.LogWarning("ADDING THIS GUY TO COMBATQ -> " + this.gameObject.name);
    //    //CombatManager.instance.combatQueue.Add(this.gameObject.transform);

    //    StartCoroutine(AttackRandomPlayerCoroutine());

    //}


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
            //Debug.LogWarning("Queueing " + this.gameObject.name);

            ChangeBattleState(BattleState.WAITING);

            yield return new WaitUntil(() => CombatManager.instance.IsFieldClear() &&
                                             CombatManager.instance.IsMyTurn(this));

            ChangeBattleState(BattleState.EXECUTING_ACTION);
            SetCurrentAction();

            //Debug.LogWarning(currentExecutingAction.actionType.ToString().ToUpper());

            if (currentExecutingAction.goToTarget)
            {
                MoveToTarget(currentPlayerTarget);

                if (currentExecutingAction.actionType == ActionType.SKILL)
                {
                    Debug.LogWarning(gameObject.name + " is using a skill named " + currentExecutingAction.actionName.ToUpper());
                    OnEnemyUsedSkill?.Invoke(currentExecutingAction.actionName);
                    //ScreenEffects.instance.ShowDarkScreen();
                }

                yield return new WaitForSeconds(secondsToReachTarget);
                Debug.LogWarning(currentExecutingAction.animationCycle.name);
                PlayAnimation(currentExecutingAction.animationCycle.name);

                yield return new WaitForSeconds(currentExecutingAction.animationCycle.cycleTime - 0.25f);

                ApplyDamageOrHeal(currentPlayerTarget);

                yield return new WaitForSeconds(0.25f);

                if (currentExecutingAction.goToTarget)
                    GoBackToStartingPosition();

                OnSkillEnded?.Invoke();
                //ScreenEffects.instance.HideDarkScreen();

                yield return new WaitForSeconds(.2f);
                PlayAnimation(idleAnimation);

                yield return new WaitForSeconds(.25f);
                //CombatManager.instance.combatQueue.RemoveAt(0);
                //CombatManager.instance.ResetGlobalEnemyAttackCD();
                CombatManager.instance.RemoveFromCombatQueue(this);
                ChangeBattleState(BattleState.RECHARGING);
                currentPlayerTarget = null;
            }

            yield return new WaitForSeconds(UnityEngine.Random.Range(minAttackRate, maxattackRate));
        }
    }

    public override void TakeDamageOrHeal(int amount, DamageType dmgType)
    {

        if (dmgType == DamageType.HARMFUL)
        {
            currentHP -= amount;
            uiController.ShowFloatingDamageText(amount, dmgType);

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
        Debug.LogWarning("1111");
        float _randomValue = UnityEngine.Random.value;

        if (_randomValue > chanceToUseSkill)
        {
            currentExecutingAction = normalAttack;
            //currentExecutingAction.actionType = ActionType.NORMAL_ATTACK;
        }
        else
        {
            currentExecutingAction = Skills[0];
            //currentExecutingAction.actionType = ActionType.SKILL;
            //SkillNameScreen.instance.Show(currentExecutingAction.actionName);
        }
    }
}
