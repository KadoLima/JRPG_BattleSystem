using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.InputSystem;
using Photon.Pun;
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


public class CharacterBehaviour : MonoBehaviour
{
    [field: SerializeField] public Transform GetAttackedPos { get; private set; }
    [SerializeField] protected SpriteEffects combatEffects;
    [field: SerializeField] public BattleState CurrentBattlePhase { get; set; }

    [Header("Character ID")]
    [SerializeField] int id;
    public int ID => id;

    [Header("STATS")]
    //[SerializeField] protected Stats myStats;
    [SerializeField] protected CharacterStats myStats;
    public CharacterStats MyStats => myStats;

    [Header("Anim Controller")]
    [SerializeField] protected CharacterAnimationController myAnimController;

    [Space(20)]

    [SerializeField] protected CombatAction normalAttack;
    [SerializeField] CombatAction[] skills;
    public CombatAction[] Skills => skills;
    [SerializeField] CombatAction useItem;

    protected CharacterUIController uiController;
    public CharacterUIController UIController => uiController;


    //public Stats MyStats => myStats;

    int currentConsumableItemIndex;
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

    public static Action<string> OnSkillUsed;
    public static Action OnSkillEnded;

    protected bool isDoingCritDamageAction;
    public bool IsDoingCritDamageAction => isDoingCritDamageAction;

    public PhotonView photonView => GetComponent<PhotonView>();

    protected int syncedCalculatedValue;

    //int defaultSortingOrder;

    protected virtual void OnEnable()
    {
        GameManager.OnGameWon += GameOver_Win;
    }

    protected virtual void OnDisable()
    {
        GameManager.OnGameWon -= GameOver_Win;
    }

    public virtual void Start()
    {
        //defaultSortingOrder = GetComponentInChildren<SpriteRenderer>().sortingOrder;

        CombatManager.instance.playersOnField.Add(this);

        originalPosition = this.transform.localPosition;

        uiController = GetComponentInChildren<CharacterUIController>();

        ChangeBattleState(BattleState.RECHARGING);

        currentHP = myStats.baseHP;
        currentMP = myStats.baseMP;

        transform.SetParent(CombatManager.instance.PlayersParent);
    }

    public virtual void Update()
    {
        if (CurrentBattlePhase == BattleState.DEAD || CombatManager.instance.CurrentActivePlayer != this)
            return;

        UIController.FindMainBattlePanel().ShowHideSwapCharsIndicator(CombatManager.instance.ReadyPlayersAmount() > 1);
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

                if (uiController.FindMainBattlePanel())
                {
                    CombatManager.instance.LookForReadyPlayer();


                    if (CombatManager.instance.CurrentActivePlayer == this)
                    {
                        //Debug.LogWarning("Showing UI for " + this.gameObject.name + " because I'm owner = " + IsOwner);
                        //uiController.HidePointer();
                        uiController.ShowMainBattlePanel();
                        //uiController.GetBattlePanel().HideSubPanels();
                    }
                }

                break;

            case BattleState.PICKING_TARGET:
                uiController.HideMainBattlePanel();
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
                CombatManager.instance.RemoveFromCombatQueue(this);
                StopAllCoroutines();

                if (!GetComponent<EnemyBehaviour>())
                {
                    myAnimController.PlayAnimation(myAnimController.DeadAnimationName);
                    Material _mat = GetComponentInChildren<SpriteRenderer>().material;
                    _mat.SetFloat("_GreyscaleBlend", 1);
                    uiController.HideCanvas(10, 0);
                    CombatManager.instance.LookForReadyPlayer();
                    StartCoroutine(CombatManager.instance.ShowGameOverIfNeeded_Coroutine());

                }
                else
                {
                    myAnimController.DisableAnimator();
                    uiController.HideCanvas(5, .25f);
                    combatEffects.DieEffect();
                    CombatManager.instance.RemoveFromField_Delayed(this.GetComponent<EnemyBehaviour>());
                }
                break;

            case BattleState.GAMEWIN:
                GoBackToStartingPosition();
                myAnimController.PlayAnimation(myAnimController.IdleAnimationName);
                uiController.HideCanvas();
                break;

            case BattleState.NULL:
                break;
            default:
                break;
        }
    }

    public virtual void ExecuteActionOn(CharacterBehaviour target)
    {
        CombatManager.instance.AddToCombatQueue(this);
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
        target.uiController.HidePointer();

        yield return new WaitUntil(() => CombatManager.instance.IsFieldClear() &&
                                         CombatManager.instance.IsMyTurn(this));

        ChangeBattleState(BattleState.EXECUTING_ACTION);


        if (target.CurrentBattlePhase == BattleState.DEAD)
        {
            if (currentExecutingAction.IsHarmful)
                target = CombatManager.instance.enemiesOnField[0];
            else target = CombatManager.instance.playersOnField[0];
        }


        if (currentExecutingAction.actionType == ActionType.SKILL)
        {
            OnSkillUsed?.Invoke(currentExecutingAction.actionName);
            DecreaseMP(currentExecutingAction.mpCost);
        }
        else if (CurrentExecutingAction.actionType == ActionType.NORMAL_ATTACK)
        {
            var _rndValue = UnityEngine.Random.value;
            isDoingCritDamageAction = _rndValue > myStats.critChance ? false : true;
            Debug.LogWarning("CRIT? " + isDoingCritDamageAction);
        }


        if (currentExecutingAction.goToTarget)
        {
            TrailEffect _trailEffect = GetComponentInChildren<TrailEffect>();

            if (_trailEffect)
                _trailEffect.ShowTrail();

            MoveToTarget(target);
            yield return new WaitForSeconds(myAnimController.SecondsToReachTarget);

            //GetComponentInChildren<SpriteRenderer>().sortingOrder = target.GetComponentInChildren<SpriteRenderer>().sortingOrder + 1 ;

            if (_trailEffect)
                _trailEffect.HideTrail();
        }

        myAnimController.PlayAnimation(currentExecutingAction.animationCycle.name);

        if (currentExecutingAction.projectile)
        {
            if (currentExecutingAction.isAreaOfEffect == false)
            {
                GameObject _instantiatedProjectile = Instantiate(currentExecutingAction.projectile.gameObject, myAnimController.ProjectileSpawnPoint.transform.position, Quaternion.identity);
                _instantiatedProjectile.transform.SetParent(gameObject.transform);
                yield return new WaitForSeconds(0.25f);
                _instantiatedProjectile.GetComponent<SpellBehaviour>().Execute(myAnimController.ProjectileSpawnPoint.position, target);
            }
            else
            {
                yield return new WaitForSeconds(0.25f);
                for (int i = 0; i < CombatManager.instance.enemiesOnField.Count; i++)
                {
                    GameObject _instantiatedProjectile = Instantiate(currentExecutingAction.projectile.gameObject, myAnimController.ProjectileSpawnPoint.position, Quaternion.identity);
                    _instantiatedProjectile.transform.SetParent(this.gameObject.transform);
                    _instantiatedProjectile.GetComponent<SpellBehaviour>().Execute(myAnimController.ProjectileSpawnPoint.position, CombatManager.instance.enemiesOnField[i]);
                }
                yield return new WaitForSeconds(0.25f);
            }
        }

        CombatManager.instance.HideAllFriendlyTargetPointers();

        yield return new WaitForSeconds(currentExecutingAction.animationCycle.cycleTime - 0.25f);

        StartCoroutine(ApplyDamageOrHeal(target));

        if (currentExecutingAction.actionType == ActionType.ITEM)
        {
            CharacterInventory _inventory = GetComponentInChildren<CharacterInventory>();
            _inventory.ConsumeItem(currentConsumableItemIndex);
            target.myAnimController.PlayHealingEffect();
        }


        yield return new WaitForSeconds(0.25f);

        GoBackToStartingPositionAndSetToIdle();
        OnSkillEnded?.Invoke();
        CombatManager.instance.RemoveFromCombatQueue(this);
        isDoingCritDamageAction = false;
        ChangeBattleState(BattleState.RECHARGING);
    }

    protected void SetToIdle()
    {
        StartCoroutine(SetToIdle_Coroutine());

    }

    IEnumerator SetToIdle_Coroutine()
    {
        currentTarget = null;
        //GetComponentInChildren<SpriteRenderer>().sortingOrder = defaultSortingOrder;
        uiController.ShowUI();
        myAnimController.PlayAnimation(myAnimController.IdleAnimationName);
        yield return new WaitForSeconds(0.001f);
        isBusy = false;
    }

    protected void SetToBusy()
    {
        isBusy = true;
        uiController.HideMainBattlePanel();
        uiController.HideUI();
    }

    public void SwapActiveCharacter()
    {
        StartCoroutine(SwapActiveCharacter_Coroutine());
    }

    IEnumerator SwapActiveCharacter_Coroutine()
    {
        yield return new WaitForSeconds(0.02f);
        UIController.HideMainBattlePanel();
        CombatManager.instance.PickAnotherReadyCharacter();
    }

    IEnumerator RechargingCoroutine()
    {
        if (!uiController.cooldownBar)
            yield break;

        currentCooldown = 0;

        yield return new WaitUntil(() => GameManager.instance.GameStarted);

        while (currentCooldown < myStats.rechargeRate)
        {
            currentCooldown += Time.deltaTime;
            uiController.cooldownBar.fillAmount = currentCooldown / myStats.rechargeRate;
            yield return null;
        }
        ChangeBattleState(BattleState.READY);
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

        currentPreAction = useItem;
        currentPreAction.damageType = dmgType;
        currentConsumableItemIndex = itemIndex;

        ChangeBattleState(BattleState.PICKING_TARGET);
    }

    public void ShowDescription(int skillIndex)
    {
        uiController.ShowDescriptionTooltip(skills[skillIndex].description);
    }

    public void MoveToTarget(CharacterBehaviour enemy)
    {
        transform.DOMove(enemy.GetAttackedPos.position, myAnimController.SecondsToReachTarget).SetEase(Ease.InOutExpo);
    }

    public void GoBackToStartingPosition()
    {
        transform.DOLocalMove(originalPosition, myAnimController.SecondsToGoBack).SetEase(Ease.OutExpo);
    }

    public void GoBackToStartingPositionAndSetToIdle()
    {
        if (currentExecutingAction.goToTarget)
        {
            //GetComponentInChildren<SpriteRenderer>().sortingOrder--;
            transform.DOLocalMove(originalPosition, myAnimController.SecondsToGoBack).SetEase(Ease.OutExpo).OnComplete(SetToIdle);
        }
    }


    protected IEnumerator ApplyDamageOrHeal(CharacterBehaviour target)
    {

        if (PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected)
        {
            syncedCalculatedValue = CalculatedValue();

            if (PhotonNetwork.IsMasterClient)
                photonView.RPC(nameof(SyncCalculatedValue), RpcTarget.Others, syncedCalculatedValue);
            //Debug.LogWarning("Sending dmg amount = " + syncedCalculatedValue);
        }

        yield return new WaitUntil(() => syncedCalculatedValue != 0);

        if (!currentExecutingAction.isAreaOfEffect)
            target.TakeDamageOrHeal(syncedCalculatedValue, currentExecutingAction.damageType, isDoingCritDamageAction);
        else
        {
            for (int i = 0; i < CombatManager.instance.enemiesOnField.Count; i++)
            {
                CombatManager.instance.enemiesOnField[i].TakeDamageOrHeal(syncedCalculatedValue, currentExecutingAction.damageType, isDoingCritDamageAction);
            }
        }

        syncedCalculatedValue = 0;
    }

    [PunRPC]
    void SyncCalculatedValue(int amount)
    {
        syncedCalculatedValue = amount;
        Debug.LogWarning("Receiving dmg amount = " + syncedCalculatedValue);
    }

    public virtual void TakeDamageOrHeal(int amount, DamageType dmgType, bool isCrit)
    {
        //Debug.LogWarning("Taking dmg/heal = " + amount);

        if (dmgType == DamageType.HARMFUL)
        {
            currentHP -= amount;
            uiController.ShowFloatingDamageText(amount, dmgType, isCrit);

            if (currentHP <= 0)
            {
                ChangeBattleState(BattleState.DEAD);
            }
        }

        else if (dmgType == DamageType.HEALING)
        {
            IncreaseHP(amount);

        }
        else if (dmgType == DamageType.MANA)
        {
            IncreaseMP(amount);
        }

        uiController.ShowFloatingDamageText(amount, dmgType, isCrit);
        uiController.RefreshHPMP();
    }

    private int CalculatedValue()
    {
        int _rawDamage = myStats.baseDamage();


        if (currentExecutingAction.actionType != ActionType.ITEM)
        {
            return Mathf.RoundToInt(_rawDamage * CritDamageMultiplier() * currentExecutingAction.damageMultiplier);
        }


        CharacterInventory _inventory = GetComponentInChildren<CharacterInventory>();
        return _inventory.inventoryItens[currentConsumableItemIndex].itemData.effectAmount;
    }

    public int CritDamageMultiplier()
    {
        if (currentExecutingAction.actionType == ActionType.SKILL)
            return 1;

        return isDoingCritDamageAction ? 2 : 1;
    }

    public void IncreaseHP(int amount)
    {
        currentHP += amount;
        if (currentHP > myStats.baseHP) currentHP = myStats.baseHP;

        uiController.RefreshHPMP();
    }

    public void IncreaseMP(int amount)
    {
        currentMP += amount;
        if (currentMP > myStats.baseMP) currentMP = myStats.baseMP;

        uiController.RefreshHPMP();
    }

    private void DecreaseMP(int amount)
    {
        currentMP -= amount;
        uiController.RefreshHPMP();
    }

    public void GameOver_Win()
    {
        StopAllCoroutines();
        ChangeBattleState(BattleState.GAMEWIN);
    }


}
