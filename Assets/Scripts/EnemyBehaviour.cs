using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : CharacterBehaviour
{
    //[field: SerializeField] public Transform GetAttackedPos { get ; private set; }
    //[SerializeField] CombatEffects combatEffects;
    [SerializeField] float chanceToUseSkill;
    public float ChanceToUseSkill => chanceToUseSkill;
    CharacterBehaviour currentPlayerTarget;
    public CharacterBehaviour CurrentPlayerTarget => currentPlayerTarget;

    // Start is called before the first frame update
    public override void Start()
    {
        UIController = GetComponent<CharacterUIController>();
        CombatManager.instance.enemiesOnField.Add(this);
        originalPosition = this.transform.localPosition;
        currentHP = myStats.baseHP;
    }



    public CharacterBehaviour GetRandomPlayer()
    {
        currentPlayerTarget = CombatManager.instance.playersOnField[Random.Range(0, CombatManager.instance.playersOnField.Count)];
        return currentPlayerTarget;
    }

    public void AttackRandomPlayer()
    {
        StartCoroutine(AttackRandomPlayerCoroutine());
    }


    IEnumerator AttackRandomPlayerCoroutine()
    {
        CharacterBehaviour currentTarget = GetRandomPlayer();
        ChangeBattleState(BattleState.EXECUTING_ACTION);
        yield return new WaitUntil(() => CombatManager.instance.FieldIsClear() == true && CombatManager.instance.EnemyCanAttack() == true);
        SetToBusy();

        SetCurrentAction();

        if (currentAction.goToTarget)
        {
            MoveToTarget(currentTarget);

            if (currentAction.actionType == ActionType.SKILL)
                ScreenEffects.instance.ShowDarkScreen();

            yield return new WaitForSeconds(secondsToReachTarget);

            PlayAnimation(currentAction.animationCycle.name);

            yield return new WaitForSeconds(currentAction.animationCycle.cycleTime - 0.25f);

            ApplyDamageOrHeal(currentTarget);

            yield return new WaitForSeconds(0.25f);

            if (currentAction.goToTarget)
                GoBackToStartingPositionAndSetToIdle();

            ScreenEffects.instance.HideDarkScreen();

            yield return new WaitForSeconds(0.2f);
            ChangeBattleState(BattleState.RECHARGING);

            SetToIdle();
        }
    }

    private void SetCurrentAction()
    {
        float randomValue = Random.value;

        if (randomValue > chanceToUseSkill)
            currentAction = normalAttack;
        else currentAction = Skills[0];
    }
}
