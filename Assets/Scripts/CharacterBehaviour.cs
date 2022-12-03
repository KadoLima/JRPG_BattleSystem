using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public enum ActionType
{
    NORMAL_ATTACK,
    SKILL,
    ITEM
}

[System.Serializable]
public struct AnimationCycle
{
    public string name;
    public float cycleTime;
}

[System.Serializable]
public struct CombatAction
{
    public ActionType actionType;
    public string actionName;
    public string description;
    public int mpCost;
    public bool goToTarget;
    public bool isAreaOfEffect;
    public float damageMultiplier;
    public AnimationCycle animationCycle;
}

[System.Serializable]
public struct Stats
{
    public int baseHP;
    public int baseMP;
    public int minDamage;
    public int maxDamage;
    public float baseCooldown;

    public int baseDamage() => UnityEngine.Random.Range(minDamage, maxDamage);
}

public class CharacterBehaviour : MonoBehaviour
{
    [field: SerializeField] public Transform GetAttackedPos { get; private set; }
    [SerializeField] CombatEffects combatEffects;
    [field: SerializeField] public BattleState CurrentBattlePhase { get; set; }


    [Header("ANIMATION PARAMETERS")]
    [SerializeField] Animator myAnim;
    [SerializeField] float secondsToReachTarget = .75f;
    [SerializeField] float secondsToGoBack = .45f;
    [Header("ANIMATION ACTIONS")]
    [SerializeField] string idleAnimation;
    [SerializeField] string deadAnimation;
    [SerializeField] CombatAction normalAttack;
    [SerializeField] CombatAction[] skills;
    public CombatAction[] Skills => skills;

    protected CharacterUIController UIController;

    [Header("STATS")]
    [SerializeField] protected Stats myStats;
    float currentCooldown;
    protected int currentHP;
    protected int currentMP;
    public int CurrentHP => currentHP;

    public int CurrentMP => currentMP;
    //[SerializeField] float baseDamage;
    //[field: SerializeField] public float BaseCooldown { get; private set; }


    //SpriteRenderer enemySpriteRenderer = null;

    protected Vector2 originalPosition;

    CombatAction currentAction;
    public CombatAction CurrentAction => currentAction;

    CharacterBehaviour currentEnemy = null;
    public CharacterBehaviour CurrentEnemy => currentEnemy;

    bool isBusy = false;
    public bool IsBusy() => isBusy;

    // Start is called before the first frame update
    public virtual void Start()
    {
        CombatManager.instance.playersOnField.Add(this);

        originalPosition = this.transform.localPosition;

        UIController = GetComponent<CharacterUIController>();

        ChangeBattleState(BattleState.RECHARGING);

        currentHP = myStats.baseHP;
        currentMP = myStats.baseMP;
    }

    private void Update()
    {
        PickingTargetCycle();
    }

    public void UseNormalAttack()
    {
        currentAction = normalAttack;
        ChangeBattleState(BattleState.PICKING_TARGET);
    }

    public void SelectTech(int skillIndex)
    {
        currentAction = skills[skillIndex];

        if (currentMP < currentAction.mpCost)
            return;

        ChangeBattleState(BattleState.PICKING_TARGET);
    }

    public void ShowDescription(int skillIndex)
    {
        UIController.ShowDescriptionTooltip(skills[skillIndex].description);
    }

    public void ShowDescription(string text)
    {
        UIController.ShowDescriptionTooltip(text);
    }

    private void PickingTargetCycle()
    {
        if (CurrentBattlePhase == BattleState.PICKING_TARGET)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                ExecuteActionOn(CombatManager.instance.enemiesOnField[CombatManager.instance.CurrentTargetEnemyIndex]);
            }

            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow) || 
                     Input.GetKeyDown(KeyCode.Backspace) || 
                     Input.GetKeyDown(KeyCode.Escape))
            {
                ChangeBattleState(BattleState.READY);
            }
        }
    }

    public void ExecuteActionOn(CharacterBehaviour enemy, int skillIndex = 0)
    {
        currentEnemy = enemy;

        //switch (actionType)
        //{
        //    case ActionType.NORMAL_ATTACK:
        //        currentAction = normalAttack;
        //        break;
        //    case ActionType.SKILL:
        //        currentAction = skills[skillIndex];
        //        break;
        //}
        StartCoroutine(ActionCoroutine(enemy));
        ChangeBattleState(BattleState.EXECUTING_ACTION);
    }

    IEnumerator ActionCoroutine(CharacterBehaviour enemy)
    {
        SetToBusy();

        CombatManager.instance.HideAllEnemyPointers();

        if (currentAction.actionType == ActionType.SKILL)
        {
            currentMP -= currentAction.mpCost;
            Debug.LogWarning($"SPENDING {currentAction.mpCost} , player has {currentMP} left");
            UIController.RefreshMP(currentMP,myStats.baseMP);
            ScreenEffects.instance.ShowDarkScreen();
        }

        if (currentAction.goToTarget)
        {
            MoveToTarget(enemy);
            yield return new WaitForSeconds(secondsToReachTarget);
        }

        PlayAnimation(currentAction.animationCycle.name);

        yield return new WaitForSeconds(currentAction.animationCycle.cycleTime - 0.25f);

        ApplyDamage(enemy);

        yield return new WaitForSeconds(0.25f);


        if (currentAction.goToTarget)
            GoBackToStartingPositionAndSetToIdle();
        else SetToIdle();

        ScreenEffects.instance.HideDarkScreen();

        yield return new WaitForSeconds(0.2f);
        ChangeBattleState(BattleState.RECHARGING);
    }

    private void ApplyDamage(CharacterBehaviour enemy)
    {
        if (!currentAction.isAreaOfEffect)
            enemy.TakeDamage(CalculatedDamageValue());
        else
        {
            for (int i = 0; i < CombatManager.instance.enemiesOnField.Count; i++)
            {
                var dmg = CalculatedDamageValue();
                Debug.LogWarning($"{CombatManager.instance.enemiesOnField[i].gameObject.name} took {dmg} damage");
                CombatManager.instance.enemiesOnField[i].TakeDamage(dmg);
            }
        }
    }

    private int CalculatedDamageValue()
    {
        int rawDamage = myStats.baseDamage();
        //Debug.LogWarning($"{currentEnemy.name} took {rawDamage * currentAction.damageMultiplier} damage");
        return Mathf.RoundToInt(rawDamage * currentAction.damageMultiplier);
    }

    public void MoveToTarget(CharacterBehaviour enemy)
    {
        //enemySpriteRenderer = enemy.GetComponentInChildren<SpriteRenderer>();
        //enemySpriteRenderer.sortingOrder++;
        transform.DOMove(enemy.GetAttackedPos.position, secondsToReachTarget).SetEase(Ease.InOutExpo);
    }

    public void GoBackToStartingPositionAndSetToIdle()
    {
        Debug.LogWarning("GOING BACK TO ORIGINAL POSITION -> " + this.secondsToGoBack);
        //enemySpriteRenderer.sortingOrder--;
        //enemySpriteRenderer = null;
        transform.DOMove(originalPosition, secondsToGoBack).SetEase(Ease.OutExpo).OnComplete(SetToIdle);
    }

    public CharacterBehaviour GetRandomEnemy()
    {
        currentEnemy = CombatManager.instance.enemiesOnField[UnityEngine.Random.Range(0, CombatManager.instance.enemiesOnField.Count)];
        return currentEnemy;
    }

    void SetToBusy()
    {
        isBusy = true;
        UIController.HideCanvas();
    }

    void PlayAnimation(string animString)
    {
        myAnim.Play(animString);
    }

    void SetToIdle()
    {
        isBusy = false;
        //currentAction = null;
        currentEnemy = null;
        UIController.ShowCanvas();

        PlayAnimation(idleAnimation);

        ChangeBattleState(BattleState.RECHARGING);
    }


    IEnumerator FillCooldownCoroutine()
    {
        if (!UIController.cooldownBar)
            yield break;

        currentCooldown = 0;

        while (currentCooldown < myStats.baseCooldown)
        {
            currentCooldown += Time.deltaTime;
            UIController.cooldownBar.fillAmount = currentCooldown / myStats.baseCooldown;
            yield return null;
        }

        ChangeBattleState(BattleState.READY);
    }

    public void ChangeBattleState(BattleState phase)
    {
        switch (phase)
        {
            case BattleState.RECHARGING:
                StartCoroutine(FillCooldownCoroutine());
                break;
            case BattleState.READY:
                CombatManager.instance.HideAllEnemyPointers();
                UIController.ShowBattlePanel();
                break;
            case BattleState.PICKING_TARGET:
                UIController.HideBattlePanel();
                CombatManager.instance.SetTargetedEnemyByIndex(0,currentAction.isAreaOfEffect);
                break;
            case BattleState.EXECUTING_ACTION:
                break;
            case BattleState.SELECTING_TECH:
                break;
            case BattleState.SELECTING_ITEM:
                break;
            case BattleState.WAITING:
                break;
            case BattleState.DEAD:
                myAnim.Play(deadAnimation);
                break;
            case BattleState.NULL:
                break;
            default:
                break;
        }

        StartCoroutine(ChangePhaseDelayed(phase));
    }

    IEnumerator ChangePhaseDelayed(BattleState p)
    {
        yield return new WaitForSeconds(0.02f);
        CurrentBattlePhase = p;
    }

    bool IsReady()
    {
        return currentCooldown >= myStats.baseCooldown;
    }

    public void TakeDamage(int damageAmount)
    {

        currentHP -= damageAmount;

        UIController.ShowFloatingDamageText(damageAmount);
        UIController.RefreshHP(currentHP, myStats.baseHP);

        if (currentHP <= 0)
        {
            ChangeBattleState(BattleState.DEAD);

            if (GetComponent<EnemyBehaviour>())
            {
                UIController.HideCanvas(5,.5f);
                combatEffects.DieEffect();
                CombatManager.instance.RemoveDelayed(this.GetComponent<EnemyBehaviour>());
            }
        }
    }

    public void IncreaseHP(int amount)
    {
        currentHP += amount;
        if (currentHP > myStats.baseHP) currentHP = myStats.baseHP;
    }

    public void IncreaseMP(int amount)
    {
        currentMP += amount;
        if (currentMP > myStats.baseMP) currentMP = myStats.baseMP;
    }


}
