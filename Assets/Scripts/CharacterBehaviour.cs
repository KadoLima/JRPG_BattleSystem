using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

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
    public ParticleSystem particles;
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

    CharacterBehaviour currentEnemy = null;
    public CharacterBehaviour CurrentEnemy => currentEnemy;

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
        if (CurrentBattlePhase == BattleState.DEAD)
            return;

        PickingTargetCycle();

        if (uiController.GetBattlePanel() && CurrentBattlePhase == BattleState.READY)
        {
            if (CombatManager.instance.ReadyPlayersAmount() > 1)
            {
                UIController.GetBattlePanel().ShowHideArrows(true);

                if (CombatManager.instance.CurrentActivePlayer == this)
                {
                    if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.A)
                        || Input.GetKeyDown(KeyCode.D))
                    {
                        StartCoroutine(PickAnotherTarget_Coroutine());
                    }
                }
            }
            else
            {
                UIController.GetBattlePanel().ShowHideArrows(false);
            }
        }
    }

    protected void SetToIdle()
    {
        StartCoroutine(SetToIdle_Coroutine());
        //isBusy = false;
        //currentEnemy = null;
        ////UIController.ShowCanvas();
        //uiController.ShowUI();
        //PlayAnimation(idleAnimation);
    }

    IEnumerator SetToIdle_Coroutine()
    {
        currentEnemy = null;
        //UIController.ShowCanvas();
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

        //UIController.HideCanvas();
    }

    IEnumerator PickAnotherTarget_Coroutine()
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

        while (currentCooldown < myStats.baseCooldown)
        {
            currentCooldown += Time.deltaTime;
            uiController.cooldownBar.fillAmount = currentCooldown / myStats.baseCooldown;
            yield return null;
        }
        ChangeBattleState(BattleState.READY);
    }



    private void PickingTargetCycle()
    {
        if (CurrentBattlePhase == BattleState.PICKING_TARGET)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                if (currentPreAction.IsHarmful)
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

    public void ExecuteActionOn(CharacterBehaviour target)
    {
        currentEnemy = target;

        StartCoroutine(ActionCoroutine(target));

    }

    IEnumerator ActionCoroutine(CharacterBehaviour target)
    {
        if (CombatManager.instance.CurrentActivePlayer == this)
        {
            CombatManager.instance.SetCurrentActivePlayer(null);
            CombatManager.instance.LookForReadyPlayer();
        }

        ChangeBattleState(BattleState.EXECUTING_ACTION);

        currentExecutingAction = currentPreAction;

        CombatManager.instance.HideAllEnemyPointers();

        yield return new WaitUntil(() => CombatManager.instance.FieldIsClear() == true);
        SetToBusy();

        if (currentExecutingAction.actionType == ActionType.SKILL)
        {
            SkillNameScreen.instance.Show(currentExecutingAction.actionName);
            currentMP -= currentExecutingAction.mpCost;
            //Debug.LogWarning($"SPENDING {currentAction.mpCost} , player has {currentMP} left");
            uiController.RefreshMP(currentMP, myStats.baseMP);
            ScreenEffects.instance.ShowDarkScreen();
        }

        if (currentExecutingAction.goToTarget)
        {
            Debug.LogWarning("GOING TO TARGET");

            TrailEffect _trailEffect = GetComponentInChildren<TrailEffect>();

            if (_trailEffect)
                _trailEffect.ShowTrail();

            MoveToTarget(target);
            yield return new WaitForSeconds(secondsToReachTarget);

            if (_trailEffect)
                _trailEffect.HideTrail();
        }

        if (currentExecutingAction.animationCycle.particles)
            currentExecutingAction.animationCycle.particles.Play();

        PlayAnimation(currentExecutingAction.animationCycle.name);

        if (currentExecutingAction.projectile)
        {
            if (currentExecutingAction.isAreaOfEffect == false)
            {
                GameObject p = Instantiate(currentExecutingAction.projectile.gameObject, projectileSpawnPoint.transform.position, Quaternion.identity);
                p.transform.SetParent(this.gameObject.transform);
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

        yield return new WaitForSeconds(currentExecutingAction.animationCycle.cycleTime - 0.25f);

        ApplyDamageOrHeal(target);

        if (currentExecutingAction.actionType == ActionType.ITEM)
        {
            Debug.LogWarning("Player is using an item..");
            CharacterInventory inventory = GetComponent<CharacterInventory>();
            inventory.ConsumeItem(currentConsumableItemIndex);
            //UIController.GetBattlePanel().GetSubPanels().RefreshConsumableItensListUI();
        }


        yield return new WaitForSeconds(0.25f);


        if (currentExecutingAction.goToTarget)
            GoBackToStartingPositionAndSetToIdle();

        ScreenEffects.instance.HideDarkScreen();

        yield return new WaitForSeconds(0.2f);
        ChangeBattleState(BattleState.RECHARGING);
    }

    public void UseNormalAttack()
    {
        currentPreAction = normalAttack;
        ChangeBattleState(BattleState.PICKING_TARGET);
    }

    public void SelectTech(int skillIndex)
    {
        currentPreAction = skills[skillIndex];

        if (currentMP < currentPreAction.mpCost)
            return;

        ChangeBattleState(BattleState.PICKING_TARGET);
    }

    public void SelectConsumableItem(int itemIndex, DamageType dmgType)
    {
        Debug.LogWarning("player selected a consumable item...");
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

    //public CharacterBehaviour GetRandomEnemy()
    //{
    //    currentEnemy = CombatManager.instance.enemiesOnField[UnityEngine.Random.Range(0, CombatManager.instance.enemiesOnField.Count)];
    //    return currentEnemy;
    //}

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
                        CombatManager.instance.HideAllEnemyPointers();
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
                break;
            case BattleState.SELECTING_TECH:
                break;
            case BattleState.SELECTING_ITEM:
                break;
            case BattleState.WAITING:
                break;
            case BattleState.DEAD:
                myAnim.Play(deadAnimation);

                if (!GetComponent<EnemyBehaviour>())
                {
                    StopAllCoroutines();
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
                //UIController.HideBattlePanel();
                uiController.HideCanvas();
                ScreenEffects.instance.HideDarkScreen();
                break;
            case BattleState.NULL:
                break;
            default:
                break;
        }
    }

    bool IsReady()
    {
        return currentCooldown >= myStats.baseCooldown;
    }

    protected void ApplyDamageOrHeal(CharacterBehaviour target)
    {
        if (!currentExecutingAction.isAreaOfEffect)
            target.TakeDamageOrHeal(CalculatedValue(), currentExecutingAction.damageType);
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
        else
        {
            if (currentExecutingAction.actionType == ActionType.ITEM)
            {
                CharacterInventory inventory = GetComponent<CharacterInventory>();

                if (inventory.inventoryItens[currentConsumableItemIndex].itemData.damageType == DamageType.HEALING)
                {
                    currentHP += amount;
                    
                    if (currentHP > myStats.baseHP)
                        currentHP = myStats.baseHP;

                    //UIController.ShowFloatingDamageText(amount, currentExecutingAction.damageType);
                }
                else if (inventory.inventoryItens[currentConsumableItemIndex].itemData.damageType == DamageType.MANA)
                {
                    currentMP += amount;

                    if (currentMP > myStats.baseMP)
                        currentMP = myStats.baseMP;

                }
                uiController.ShowFloatingDamageText(amount, inventory.inventoryItens[currentConsumableItemIndex].itemData.damageType);
            }

            

        }
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
