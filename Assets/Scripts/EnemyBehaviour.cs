using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : CharacterBehaviour
{
    [Header("ENEMY SPECIFIC PARAMETERS")]
    [SerializeField] int xpRewarded;
    [SerializeField] float chanceToUseSkill;
    public float ChanceToUseSkill => chanceToUseSkill;
    CharacterBehaviour currentPlayerTarget;
    public CharacterBehaviour CurrentPlayerTarget => currentPlayerTarget;

    // Start is called before the first frame update
    public override void Start()
    {
        uiController = GetComponentInChildren<CharacterUIController>();
        CombatManager.instance.enemiesOnField.Add(this);
        originalPosition = transform.localPosition;
        currentHP = myStats.baseHP;

    }



    //public CharacterBehaviour GetRandomPlayer()
    //{
    //    int _randomPlayerIndex = Random.Range(0,CombatManager.instance.playersOnField.Count);

    //    while (CombatManager.instance.playersOnField[_randomPlayerIndex].CurrentBattlePhase == BattleState.DEAD)
    //    {
    //        _randomPlayerIndex = Random.Range(0, CombatManager.instance.playersOnField.Count);
    //    }

    //    currentPlayerTarget = CombatManager.instance.playersOnField[_randomPlayerIndex];
    //    return currentPlayerTarget;
    //}

    public override void ExecuteActionOn(CharacterBehaviour target)
    {
        //Debug.LogWarning($"I'm {gameObject.name} and I'm starting action");
        currentPlayerTarget = target;
        StartCoroutine(AttackRandomPlayerCoroutine(target));
        CombatManager.instance.AddToCombatQueue(this);
    }

    //public void AttackRandomPlayer()
    //{
    //    Debug.LogWarning("ADDING THIS GUY TO COMBATQ -> " + this.gameObject.name);
    //    //CombatManager.instance.combatQueue.Add(this.gameObject.transform);

    //    StartCoroutine(AttackRandomPlayerCoroutine());

    //}


    IEnumerator AttackRandomPlayerCoroutine(CharacterBehaviour targetPlayer)
    {
        ChangeBattleState(BattleState.READY);

        yield return new WaitUntil(() => CombatManager.instance.IsFieldClear() && 
                                         CombatManager.instance.IsMyTurn(this));
        ChangeBattleState(BattleState.EXECUTING_ACTION);
        SetCurrentAction();

        if (currentExecutingAction.goToTarget)
        {
            MoveToTarget(targetPlayer);

            if (currentExecutingAction.actionType == ActionType.SKILL)
            {
                OnUsedSkill?.Invoke();
                //ScreenEffects.instance.ShowDarkScreen();
            }

            yield return new WaitForSeconds(secondsToReachTarget);

            PlayAnimation(currentExecutingAction.animationCycle.name);

            yield return new WaitForSeconds(currentExecutingAction.animationCycle.cycleTime - 0.25f);

            ApplyDamageOrHeal(targetPlayer);

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

    private void SetCurrentAction()
    {
        float _randomValue = Random.value;

        if (_randomValue > chanceToUseSkill)
            currentExecutingAction = normalAttack;
        else
        {
            currentExecutingAction = Skills[0];
            SkillNameScreen.instance.Show(currentExecutingAction.actionName);
        }
    }
}
