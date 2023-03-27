using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : CharacterBehaviour
{
    [Header("ENEMY SPECIFIC PARAMETERS")]
    [SerializeField] bool isAggressive = true;
    [SerializeField] int xpRewarded;
    [SerializeField] float chanceToUseSkill;
    public float ChanceToUseSkill => chanceToUseSkill;
    CharacterBehaviour currentPlayerTarget;
    public CharacterBehaviour CurrentPlayerTarget => currentPlayerTarget;

    // Start is called before the first frame update
    public override void Start()
    {
        uiController = GetComponent<CharacterUIController>();
        CombatManager.instance.enemiesOnField.Add(this);
        originalPosition = transform.localPosition;
        currentHP = myStats.baseHP;

    }



    public CharacterBehaviour GetRandomPlayer()
    {
        int randomPlayerIndex = Random.Range(0,CombatManager.instance.playersOnField.Count);

        while (CombatManager.instance.playersOnField[randomPlayerIndex].CurrentBattlePhase == BattleState.DEAD)
        {
            randomPlayerIndex = Random.Range(0, CombatManager.instance.playersOnField.Count);
        }

        currentPlayerTarget = CombatManager.instance.playersOnField[randomPlayerIndex];
        return currentPlayerTarget;
    }

    public void AttackRandomPlayer()
    {
        CombatManager.instance.combatQueue.Add(this.transform);

        if (isAggressive)
        {
            StartCoroutine(AttackRandomPlayerCoroutine());
        }
    }


    IEnumerator AttackRandomPlayerCoroutine()
    {
        ChangeBattleState(BattleState.READY);
        CharacterBehaviour currentTarget = GetRandomPlayer();

        yield return new WaitUntil(() => CombatManager.instance.FieldIsClear() == true &&
                                         (CombatManager.instance.combatQueue.Count > 0 && CombatManager.instance.combatQueue[0] == this.transform));
        ChangeBattleState(BattleState.EXECUTING_ACTION);
        SetCurrentAction();

        if (currentExecutingAction.goToTarget)
        {
            MoveToTarget(currentTarget);

            if (currentExecutingAction.actionType == ActionType.SKILL)
                ScreenEffects.instance.ShowDarkScreen();

            yield return new WaitForSeconds(secondsToReachTarget);

            PlayAnimation(currentExecutingAction.animationCycle.name);

            yield return new WaitForSeconds(currentExecutingAction.animationCycle.cycleTime - 0.25f);

            ApplyDamageOrHeal(currentTarget);

            yield return new WaitForSeconds(0.25f);

            if (currentExecutingAction.goToTarget)
                GoBackToStartingPosition();

            ScreenEffects.instance.HideDarkScreen();

            yield return new WaitForSeconds(.2f);
            PlayAnimation(idleAnimation);

            //yield return new WaitForSeconds(.25f);
            CombatManager.instance.combatQueue.Remove(this.transform);
            CombatManager.instance.ResetGlobalEnemyAttackCD();
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
        float randomValue = Random.value;

        if (randomValue > chanceToUseSkill)
            currentExecutingAction = normalAttack;
        else
        {
            currentExecutingAction = Skills[0];
            SkillNameScreen.instance.Show(currentExecutingAction.actionName);
        }
    }
}
