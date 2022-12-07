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
    public ParticleSystem particles;
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
    public bool isFriendlyAction;
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
    [SerializeField] protected float secondsToReachTarget = .75f;
    [SerializeField] protected float secondsToGoBack = .45f;
    [Header("ANIMATION ACTIONS")]
    [SerializeField] string idleAnimation;
    [SerializeField] string deadAnimation;
    [SerializeField] protected CombatAction normalAttack;
    [SerializeField] CombatAction[] skills;
    [SerializeField] CombatAction useItem;
    int currentConsumableItemIndex;
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

    protected CombatAction currentAction;
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

    public virtual void Update()
    {
        PickingTargetCycle();
    }

    private void PickingTargetCycle()
    {
        if (CurrentBattlePhase == BattleState.PICKING_TARGET)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                if (!currentAction.isFriendlyAction)
                    ExecuteActionOn(CombatManager.instance.enemiesOnField[CombatManager.instance.CurrentTargetEnemyIndex]);
                else ExecuteActionOn(CombatManager.instance.playersOnField[CombatManager.instance.CurrentFriendlyTargetIndex]);
            }

            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow) ||
                     Input.GetKeyDown(KeyCode.Backspace) ||
                     Input.GetKeyDown(KeyCode.Escape))
            {
                ChangeBattleState(BattleState.READY);
            }
        }
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

    public void SelectConsumableItem(int itemIndex)
    {
        currentAction = useItem;
        currentConsumableItemIndex = itemIndex;
        Debug.LogWarning(itemIndex);

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



    public void ExecuteActionOn(CharacterBehaviour target)
    {
        currentEnemy = target;

        StartCoroutine(ActionCoroutine(target));
        ChangeBattleState(BattleState.EXECUTING_ACTION);
    }

    IEnumerator ActionCoroutine(CharacterBehaviour target)
    {
       
        CombatManager.instance.HideAllEnemyPointers();

        yield return new WaitUntil(() => CombatManager.instance.FieldIsClear() == true);
        SetToBusy();

        if (currentAction.actionType == ActionType.SKILL)
        {
            currentMP -= currentAction.mpCost;
            //Debug.LogWarning($"SPENDING {currentAction.mpCost} , player has {currentMP} left");
            UIController.RefreshMP(currentMP,myStats.baseMP);
            ScreenEffects.instance.ShowDarkScreen();
        }

        if (currentAction.goToTarget)
        {
            MoveToTarget(target);
            yield return new WaitForSeconds(secondsToReachTarget);
        }

        if (currentAction.animationCycle.particles)
            currentAction.animationCycle.particles.Play();

        PlayAnimation(currentAction.animationCycle.name);

        yield return new WaitForSeconds(currentAction.animationCycle.cycleTime - 0.25f);

        ApplyDamageOrHeal(target);

        if (currentAction.actionType == ActionType.ITEM)
        {
            CharacterInventory inventory = GetComponent<CharacterInventory>();
            inventory.ConsumeItem(currentConsumableItemIndex);
            //UIController.GetBattlePanel().GetSubPanels().RefreshConsumableItensListUI();
        }

        yield return new WaitForSeconds(0.25f);


        if (currentAction.goToTarget)
            GoBackToStartingPositionAndSetToIdle();

        ScreenEffects.instance.HideDarkScreen();

        yield return new WaitForSeconds(0.2f);
        ChangeBattleState(BattleState.RECHARGING);

        SetToIdle();
    }

    protected void ApplyDamageOrHeal(CharacterBehaviour target)
    {
        if (!currentAction.isAreaOfEffect)
            target.TakeDamageOrHeal(CalculatedValue());
        else
        {
            for (int i = 0; i < CombatManager.instance.enemiesOnField.Count; i++)
            {
                var dmg = CalculatedValue();
                //Debug.LogWarning($"{CombatManager.instance.enemiesOnField[i].gameObject.name} took {dmg} damage");
                CombatManager.instance.enemiesOnField[i].TakeDamageOrHeal(dmg);
            }
        }
    }

    private int CalculatedValue()
    {
        int rawDamage = myStats.baseDamage();
        if (currentAction.actionType != ActionType.ITEM)
            return Mathf.RoundToInt(rawDamage * currentAction.damageMultiplier);


        CharacterInventory inventory = GetComponent<CharacterInventory>();
        return inventory.inventoryItens[currentConsumableItemIndex].itemData.effectAmount;
    }

    public void MoveToTarget(CharacterBehaviour enemy)
    {
        transform.DOMove(enemy.GetAttackedPos.position, secondsToReachTarget).SetEase(Ease.InOutExpo);
    }

    public void GoBackToStartingPositionAndSetToIdle()
    {
        transform.DOMove(originalPosition, secondsToGoBack).SetEase(Ease.OutExpo).OnComplete(SetToIdle);
    }

    public CharacterBehaviour GetRandomEnemy()
    {
        currentEnemy = CombatManager.instance.enemiesOnField[UnityEngine.Random.Range(0, CombatManager.instance.enemiesOnField.Count)];
        return currentEnemy;
    }

    protected void SetToBusy()
    {
        isBusy = true;
        UIController.HideCanvas();
    }

    protected void PlayAnimation(string animString)
    {
        myAnim.Play(animString);
    }

    protected void SetToIdle()
    {
        isBusy = false;
        //currentAction = null;
        currentEnemy = null;
        UIController.ShowCanvas();

        PlayAnimation(idleAnimation);

        //ChangeBattleState(BattleState.RECHARGING);
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
                CombatManager.instance.HideAllFriendlyTargetPointers();
                UIController.ShowBattlePanel();
                UIController.GetBattlePanel().HideSubPanels();
                break;
            case BattleState.PICKING_TARGET:
                UIController.HideBattlePanel();
                if (!currentAction.isFriendlyAction)
                    CombatManager.instance.SetTargetedEnemyByIndex(0, currentAction.isAreaOfEffect);
                else CombatManager.instance.SetTargetedFriendlyTargetByIndex(0, currentAction.isAreaOfEffect);
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

    public void TakeDamageOrHeal(int amount)
    {
        if (!currentAction.isFriendlyAction)
        {
            currentHP -= amount;
            UIController.ShowFloatingDamageText(amount);

            if (currentHP <= 0)
            {
                ChangeBattleState(BattleState.DEAD);

                if (GetComponent<EnemyBehaviour>())
                {
                    UIController.HideCanvas(5, .5f);
                    combatEffects.DieEffect();
                    CombatManager.instance.RemoveDelayed(this.GetComponent<EnemyBehaviour>());
                }
            }
        }
        else
        {
            if (currentAction.actionType == ActionType.ITEM)
            {
                CharacterInventory inventory = GetComponent<CharacterInventory>();

                if (inventory.inventoryItens[currentConsumableItemIndex].itemData.consumableType == InventoryItemData.ConsumableType.HP_POTION)
                {
                    Debug.LogWarning("RESTORING HP");
                    currentHP += amount;
                    
                    if (currentHP > myStats.baseHP)
                        currentHP = myStats.baseHP;

                    UIController.ShowFloatingDamageText(amount, true);
                }
                else if (inventory.inventoryItens[currentConsumableItemIndex].itemData.consumableType == InventoryItemData.ConsumableType.MANA_POTION)
                {
                    Debug.LogWarning("RESTORING MANA");
                    currentMP += amount;

                    if (currentMP > myStats.baseMP)
                        currentMP = myStats.baseMP;

                    UIController.ShowFloatingDamageText(amount, false, true);
                }
            }

            

        }

        UIController.RefreshHP(currentHP, myStats.baseHP);
        UIController.RefreshMP(currentMP, myStats.baseMP);
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

    public void ShowPointer()
    {
        UIController.ShowHidePointer(true);
    }

    public void HidePointer()
    {
        UIController.ShowHidePointer(false);
    }


}
