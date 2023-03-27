using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.InputSystem;

public enum ActionType
{
    NORMAL_ATTACK,
    SKILL,
    ITEM,
    NULL
}

public enum DamageType
{
    HARMFUL, 
    HEALING, 
    MANA, 
    UNDEFINED
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
    public DamageType damageType;
    public string actionName;
    public string description;
    public int mpCost;
    public GameObject projectile;
    public bool goToTarget;
    public bool isAreaOfEffect;
    public float damageMultiplier;
    public AnimationCycle animationCycle;

    public bool IsHarmful => this.damageType == DamageType.HARMFUL;
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
    [SerializeField] protected CombatEffects combatEffects;
    [field: SerializeField] public BattleState CurrentBattlePhase { get; set; }


    [Header("ANIMATION PARAMETERS")]
    [SerializeField] Animator myAnim;
    [SerializeField] protected float secondsToReachTarget = .75f;
    [SerializeField] protected float secondsToGoBack = .45f;
    [SerializeField] protected string idleAnimation;
    [SerializeField] string deadAnimation;
    [SerializeField] Transform projectileSpawnPoint;
    [SerializeField] ParticleSystem healingEffect;
    [Header("ANIMATION ACTIONS")]
    [SerializeField] protected CombatAction normalAttack;
    [SerializeField] CombatAction[] skills;
    [SerializeField] CombatAction useItem;

    int currentConsumableItemIndex;
    public CombatAction[] Skills => skills;

    protected CharacterUIController uiController;
    public CharacterUIController UIController => uiController;

    [Header("STATS")]
    [SerializeField] protected Stats myStats;
    public Stats MyStats => myStats;

    float currentCooldown;
    protected int currentHP;
    protected int currentMP;
    public int CurrentHP => currentHP;

    public int CurrentMP => currentMP;


    protected Vector2 originalPosition;

    protected CombatAction currentPreAction;
    protected CombatAction currentExecutingAction;
    public CombatAction CurrentPreAction => currentPreAction;
    public CombatAction CurrentExecutingAction => currentExecutingAction;

    CharacterBehaviour currentTarget = null;
    public CharacterBehaviour CurrentTarget => currentTarget;

    bool isBusy = false;
    public bool IsBusy() => isBusy;

    // Start is called before the first frame update
    public virtual void Start()
    {
        CombatManager.instance.playersOnField.Add(this);

        originalPosition = this.transform.localPosition;

        uiController = GetComponent<CharacterUIController>();

        ChangeBattleState(BattleState.RECHARGING);

        currentHP = myStats.baseHP;
        currentMP = myStats.baseMP;
    }

    public virtual void Update()
    {

        if (CurrentBattlePhase == BattleState.DEAD || CombatManager.instance.CurrentActivePlayer != this)
            return;

        UIController.GetBattlePanel().ShowHideSwapCharsIndicator(CombatManager.instance.ReadyPlayersAmount() > 1);



    }

    protected void SetToIdle()
    {
        StartCoroutine(SetToIdle_Coroutine());

    }

    IEnumerator SetToIdle_Coroutine()
    {
        currentTarget = null;
        uiController.ShowUI();
        PlayAnimation(idleAnimation);
        yield return new WaitForSeconds(CombatManager.instance.GlobalIntervalBetweenActions);
        isBusy = false;
    }

    protected void SetToBusy()
    {
        isBusy = true;
        uiController.HideBattlePanel();
        uiController.HideUI();
    }

    public void SwapActiveCharacter()
    {
        StartCoroutine(SwapActiveCharacter_Coroutine());
    }

    IEnumerator SwapActiveCharacter_Coroutine()
    {
        yield return new WaitForSeconds(0.02f);
        UIController.HideBattlePanel();
        CombatManager.instance.PickAnotherReadyCharacter();
    }

    IEnumerator RechargingCoroutine()
    {
        if (!uiController.cooldownBar)
            yield break;

        currentCooldown = 0;

        yield return new WaitUntil(() => GameManager.instance.gameStarted);

        while (currentCooldown < myStats.baseCooldown)
        {
            currentCooldown += Time.deltaTime;
            uiController.cooldownBar.fillAmount = currentCooldown / myStats.baseCooldown;
            yield return null;
        }
        ChangeBattleState(BattleState.READY);
    }


    public void ExecuteActionOn(CharacterBehaviour target)
    {
        currentTarget = target;
        StartCoroutine(ActionCoroutine(target));

    }

    IEnumerator ActionCoroutine(CharacterBehaviour target)
    {
        if (CombatManager.instance.CurrentActivePlayer == this)
        {
            CombatManager.instance.SetCurrentActivePlayer(null);
            CombatManager.instance.LookForReadyPlayer();
        }

        currentExecutingAction = currentPreAction;

        CombatManager.instance.HideAllEnemyPointers();

        yield return new WaitUntil(() => CombatManager.instance.FieldIsClear() == true && CombatManager.instance.PlayerCanAttack() && 
                                         (CombatManager.instance.combatQueue.Count>0 && CombatManager.instance.combatQueue[0] == this.transform));

        ChangeBattleState(BattleState.EXECUTING_ACTION);
        

        if (target.CurrentBattlePhase == BattleState.DEAD)
        {
            if (currentExecutingAction.IsHarmful)
                target = CombatManager.instance.enemiesOnField[0];
            else target = CombatManager.instance.playersOnField[0];
        }


        if (currentExecutingAction.actionType == ActionType.SKILL)
        {
            SkillNameScreen.instance.Show(currentExecutingAction.actionName);
            currentMP -= currentExecutingAction.mpCost;
            uiController.RefreshMP(currentMP, myStats.baseMP);
            ScreenEffects.instance.ShowDarkScreen();
        }

        if (currentExecutingAction.goToTarget)
        {

            TrailEffect _trailEffect = GetComponentInChildren<TrailEffect>();

            if (_trailEffect)
                _trailEffect.ShowTrail();

            MoveToTarget(target);
            yield return new WaitForSeconds(secondsToReachTarget);

            if (_trailEffect)
                _trailEffect.HideTrail();
        }


        PlayAnimation(currentExecutingAction.animationCycle.name);

        if (currentExecutingAction.projectile)
        {
            if (currentExecutingAction.isAreaOfEffect == false)
            {
                GameObject p = Instantiate(currentExecutingAction.projectile.gameObject, projectileSpawnPoint.transform.position, Quaternion.identity);
                p.transform.SetParent(gameObject.transform);
                yield return new WaitForSeconds(0.25f);
                p.GetComponent<SpellBehaviour>().Execute(projectileSpawnPoint.position, target);
            }
            else
            {
                yield return new WaitForSeconds(0.25f);
                for (int i = 0; i < CombatManager.instance.enemiesOnField.Count; i++)
                {
                    GameObject p = Instantiate(currentExecutingAction.projectile.gameObject, projectileSpawnPoint.transform.position, Quaternion.identity);
                    p.transform.SetParent(this.gameObject.transform);
                    p.GetComponent<SpellBehaviour>().Execute(projectileSpawnPoint.position, CombatManager.instance.enemiesOnField[i]);
                }
                yield return new WaitForSeconds(0.25f);
            }
        }

        CombatManager.instance.HideAllFriendlyTargetPointers();

        yield return new WaitForSeconds(currentExecutingAction.animationCycle.cycleTime - 0.25f);

        ApplyDamageOrHeal(target);

        if (currentExecutingAction.actionType == ActionType.ITEM)
        {

            CharacterInventory inventory = GetComponent<CharacterInventory>();
            inventory.ConsumeItem(currentConsumableItemIndex);
            PlayHealingEffect(target);
        }


        yield return new WaitForSeconds(0.25f);


        if (currentExecutingAction.goToTarget)
            GoBackToStartingPositionAndSetToIdle();

        ScreenEffects.instance.HideDarkScreen();

        yield return new WaitForSeconds(0.2f);
        CombatManager.instance.combatQueue.Remove(this.transform);
        CombatManager.instance.ResetInternalPlayerActionCD();
        ChangeBattleState(BattleState.RECHARGING);
    }

    public void UseNormalAttack()
    {
        if (CombatManager.instance.CurrentActivePlayer != this)
            return;

        currentPreAction = normalAttack;
        ChangeBattleState(BattleState.PICKING_TARGET);
    }

    public void SelectTech(int skillIndex)
    {
        if (CombatManager.instance.CurrentActivePlayer != this)
            return;

        currentPreAction = skills[skillIndex];

        if (currentMP < currentPreAction.mpCost)
            return;

        ChangeBattleState(BattleState.PICKING_TARGET);
    }

    public void SelectConsumableItem(int itemIndex, DamageType dmgType)
    {
        if (CombatManager.instance.CurrentActivePlayer != this)
            return;

        //Debug.LogWarning("player selected a consumable item...");
        currentPreAction = useItem;
        currentPreAction.damageType = dmgType;
        currentConsumableItemIndex = itemIndex;

        ChangeBattleState(BattleState.PICKING_TARGET);
    }

    public void ShowDescription(int skillIndex)
    {
        uiController.ShowDescriptionTooltip(skills[skillIndex].description);
    }

    public void ShowDescription(string text)
    {
        uiController.ShowDescriptionTooltip(text);
    }

    public void MoveToTarget(CharacterBehaviour enemy)
    {
        transform.DOMove(enemy.GetAttackedPos.position, secondsToReachTarget).SetEase(Ease.InOutExpo);
    }

    public void GoBackToStartingPosition()
    {
        transform.DOLocalMove(originalPosition, secondsToGoBack).SetEase(Ease.OutExpo);
    }

    public void GoBackToStartingPositionAndSetToIdle()
    {
        transform.DOLocalMove(originalPosition, secondsToGoBack).SetEase(Ease.OutExpo).OnComplete(SetToIdle);
    }

    protected void PlayAnimation(string animString)
    {
        myAnim.Play(animString);
    }

    public void ChangeBattleState(BattleState phase)
    {
        StartCoroutine(ChangePhaseDelayed(phase));
    }

    IEnumerator ChangePhaseDelayed(BattleState p)
    {
        yield return new WaitForSeconds(0.02f);
        CurrentBattlePhase = p;

        switch (CurrentBattlePhase)
        {
            case BattleState.RECHARGING:
                currentPreAction.actionType = ActionType.NULL;
                StartCoroutine(RechargingCoroutine());
                SetToIdle();
                break;
            case BattleState.READY:

                if (uiController.GetBattlePanel())
                {
                    CombatManager.instance.LookForReadyPlayer();


                    if (CombatManager.instance.CurrentActivePlayer == this)
                    {
                        uiController.ShowHidePointer(false);
                        uiController.ShowBattlePanel();
                        uiController.GetBattlePanel().HideSubPanels();
                    }
                }

                break;

            case BattleState.PICKING_TARGET:
                uiController.HideBattlePanel();
                if (currentPreAction.IsHarmful)
                    CombatManager.instance.SetTargetedEnemyByIndex(0, currentPreAction.isAreaOfEffect);
                else CombatManager.instance.SetTargetedFriendlyTargetByIndex(0, currentPreAction.isAreaOfEffect);
                break;
            case BattleState.EXECUTING_ACTION:
                SetToBusy();
                break;
            case BattleState.SELECTING_TECH:
                break;
            case BattleState.SELECTING_ITEM:
                break;
            case BattleState.WAITING:
                break;
            case BattleState.DEAD:
                myAnim.Play(deadAnimation);
                StopAllCoroutines();

                if (!GetComponent<EnemyBehaviour>())
                {
                    Material mat = GetComponentInChildren<SpriteRenderer>().material;
                    mat.SetFloat("_GreyscaleBlend", 1);
                    uiController.HideCanvas(10, 0);
                    StartCoroutine(CombatManager.instance.ShowGameOverIfNeeded_Coroutine());

                }
                else
                {
                    uiController.HideCanvas(5, .25f);
                    combatEffects.DieEffect();
                    CombatManager.instance.RemoveFromField_Delayed(this.GetComponent<EnemyBehaviour>());
                }
                break;

            case BattleState.GAMEWIN:
                GoBackToStartingPosition();
                PlayAnimation(idleAnimation);
                uiController.HideCanvas();
                ScreenEffects.instance.HideDarkScreen();
                break;
            case BattleState.NULL:
                break;
            default:
                break;
        }
    }

    public void PlayHealingEffect(CharacterBehaviour target)
    {
        target.healingEffect.Play();
    }

    bool IsReady()
    {
        return currentCooldown >= myStats.baseCooldown;
    }

    protected void ApplyDamageOrHeal(CharacterBehaviour target)
    {
        if (!currentExecutingAction.isAreaOfEffect)
            target.TakeDamageOrHeal(CalculatedValue(), this.currentExecutingAction.damageType);
        else
        {
            for (int i = 0; i < CombatManager.instance.enemiesOnField.Count; i++)
            {
                var dmg = CalculatedValue();
                CombatManager.instance.enemiesOnField[i].TakeDamageOrHeal(dmg, currentExecutingAction.damageType);
            }
        }
    }


    public virtual void TakeDamageOrHeal(int amount, DamageType dmgType)
    {

        if (dmgType == DamageType.HARMFUL)
        {
            currentHP -= amount;
            uiController.ShowFloatingDamageText(amount, dmgType);

            if (currentHP <= 0)
            {
                ChangeBattleState(BattleState.DEAD);
            }
        }

        else if (dmgType == DamageType.HEALING)
        {
            currentHP += amount;

            if (currentHP > myStats.baseHP)
                currentHP = myStats.baseHP;

        }
        else if (dmgType == DamageType.MANA)
        {
            currentMP += amount;

            if (currentMP > myStats.baseMP)
                currentMP = myStats.baseMP;

        }
        uiController.ShowFloatingDamageText(amount, dmgType);
        uiController.RefreshHPMP();
    }

    private int CalculatedValue()
    {
        int rawDamage = myStats.baseDamage();
        if (currentExecutingAction.actionType != ActionType.ITEM)
            return Mathf.RoundToInt(rawDamage * currentExecutingAction.damageMultiplier);


        CharacterInventory inventory = GetComponent<CharacterInventory>();
        return inventory.inventoryItens[currentConsumableItemIndex].itemData.effectAmount;
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
        uiController.ShowHidePointer(true);
    }

    public void HidePointer()
    {
        uiController.ShowHidePointer(false);
    }

    public void GameOver_Win()
    {
        StopAllCoroutines();
        ChangeBattleState(BattleState.GAMEWIN);


    }

    
}
