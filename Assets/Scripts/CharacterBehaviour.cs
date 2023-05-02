using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.InputSystem;
using Photon.Pun;


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

    [SerializeField] protected CombatActionSO[] characterActions; // 0 = recharging, 1 = normal attack, 2-8 = skills, 9 = useItem
    public CombatActionSO[] CharacterActions => characterActions;

    [Space(20)]
    [Header("INVENTORY")]
    [SerializeField] CharacterInventory inventory;

    //[SerializeField] protected CombatActionSO recharging;
    //[SerializeField] protected CombatActionSO normalAttack;
    //[SerializeField] CombatActionSO[] skills;
    //public CombatActionSO[] Skills => skills;
    //[SerializeField] CombatActionSO useItem;

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

    protected CombatActionSO currentPreAction;
    protected CombatActionSO currentExecutingAction;
    public CombatActionSO CurrentPreAction => currentPreAction;
    //public CombatAction CurrentExecutingAction => currentExecutingAction;

    CharacterBehaviour currentTarget = null;
    public CharacterBehaviour CurrentTarget
    {
        get => currentTarget;
        set => currentTarget = value;
    }

    bool isBusy = false;
    public bool IsBusy() => isBusy;

    public static Action<string> OnSkillUsed;
    public static Action OnSkillEnded;

    protected bool isDoingCritDamageAction;
    public bool IsDoingCritDamageAction => isDoingCritDamageAction;

    protected PhotonView myPhotonView;
    public PhotonView MyPhotonView => myPhotonView;

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
        myPhotonView = GetComponent<PhotonView>();

        currentPreAction = ScriptableObject.CreateInstance<CombatActionSO>();
        currentExecutingAction = ScriptableObject.CreateInstance<CombatActionSO>();

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

    #region STATS CHANGE
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

    #endregion

    #region DAMAGE
    protected IEnumerator ApplyDamageOrHeal(CharacterBehaviour target)
    {

        if (PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected)
        {
            syncedCalculatedValue = CalculatedValue();

            if (PhotonNetwork.IsMasterClient)
                MyPhotonView.RPC(nameof(SyncCalculatedValue), RpcTarget.Others, syncedCalculatedValue);
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

    public virtual void TakeDamageOrHeal(int amount, DamageType dmgType, bool isCrit)
    {
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

        return inventory.inventoryItens[currentConsumableItemIndex].itemData.effectAmount;
    }

    public int CritDamageMultiplier()
    {
        if (currentExecutingAction.actionType == ActionType.SKILL)
            return 1;

        return isDoingCritDamageAction ? 2 : 1;
    }

    #endregion

    #region MOVEMENT
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

    #endregion

    #region ACTIONS

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

    public CombatActionSO SelectAction(ActionType type)
    {
        //Debug.LogWarning(type);

        for (int i = 0; i < characterActions.Length; i++)
        {
            if (characterActions[i].actionType == type)
                return characterActions[i];
        }

        //Debug.LogError("ACTION TYPE NOT FOUND");
        return null;
    }

    public void UseNormalAttack()
    {
        //Debug.LogWarning(CombatManager.instance.CurrentActivePlayer);

        if (CombatManager.instance.CurrentActivePlayer != this)
            return;

        //currentPreAction = normalAttack;
        currentPreAction = SelectAction(ActionType.NORMAL_ATTACK);
        //Debug.LogWarning(currentPreAction.actionType);
        ChangeBattleState(BattleState.PICKING_TARGET);
    }

    public void SelectTech(int skillIndex)
    {
        if (CombatManager.instance.CurrentActivePlayer != this)
            return;

        //currentPreAction = skills[skillIndex];
        currentPreAction = characterActions[2 + skillIndex];

        if (currentMP < currentPreAction.mpCost)
            return;

        ChangeBattleState(BattleState.PICKING_TARGET);
    }

    public void SelectConsumableItem(int itemIndex, DamageType dmgType)
    {
        if (CombatManager.instance.CurrentActivePlayer != this)
            return;

        // currentPreAction = useItem;
        currentPreAction = SelectAction(ActionType.ITEM);
        currentPreAction.damageType = dmgType;
        currentConsumableItemIndex = itemIndex;

        ChangeBattleState(BattleState.PICKING_TARGET);
    }

    public virtual void ExecuteActionOn(CharacterBehaviour target)
    {
        if (!PhotonNetwork.IsConnected || myPhotonView.IsMine)
        {
            CombatManager.instance.AddToCombatQueue(this);
            currentTarget = target;
            uiController.HideChatBubble();

            if (PhotonNetwork.IsConnected)
            {
                int _targetViewID = currentTarget.GetComponent<PhotonView>().ViewID;
                //Debug.LogWarning($"Target is {currentTarget.name}, ViewID {_targetViewID}");
                //Debug.LogWarning(currentPreAction.actionType);


                int _actionIndex = -1;
                for (int i = 0; i < characterActions.Length; i++)
                {
                    if (currentPreAction == characterActions[i])
                    {
                        _actionIndex = i;
                        break;
                    }
                }

                //Debug.LogWarning(_actionIndex);
                myPhotonView.RPC(nameof(SyncExecuteAction), RpcTarget.Others, _targetViewID, _actionIndex);
                //myPhotonView.RPC(nameof(SyncExecuteAction), RpcTarget.Others, _targetViewID, JsonUtility.ToJson(currentPreAction));
            }

            StartCoroutine(ExecuteActionCoroutine(currentTarget));
        }
    }

    IEnumerator ExecuteActionCoroutine(CharacterBehaviour target)
    {
        if (CombatManager.instance.CurrentActivePlayer == this)
        {
            CombatManager.instance.CurrentActivePlayer = null;
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
        else if (currentExecutingAction.actionType == ActionType.NORMAL_ATTACK)
        {
            var _rndValue = UnityEngine.Random.value;

            if (!PhotonNetwork.IsConnected || myPhotonView.IsMine)
            {
                isDoingCritDamageAction = _rndValue > myStats.critChance ? false : true;
                //Debug.LogWarning("Sending CRIT DAMAGE bool = " + isDoingCritDamageAction);

                if (PhotonNetwork.IsConnected)
                    myPhotonView.RPC(nameof(SyncIsDoingCritDamage), RpcTarget.Others, isDoingCritDamageAction);
            }
            //Debug.LogWarning("CRIT? " + isDoingCritDamageAction);
        }

        //Debug.LogWarning("currentExecutingAction: " + currentExecutingAction.actionType);

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

        //Debug.LogWarning(gameObject.name + " -> " + currentExecutingAction.actionType);
        myAnimController.PlayAnimation(currentExecutingAction.animationCycle.name);

        //Debug.LogWarning(currentExecutingAction);

        if (currentExecutingAction.projectile!=null)
        {
            if (currentExecutingAction.isAreaOfEffect == false)
            {
                SpellBehaviour _instantiatedProjectile = Instantiate(currentExecutingAction.projectile.gameObject, myAnimController.ProjectileSpawnPoint.transform.position, Quaternion.identity).GetComponent<SpellBehaviour>();

                //SpellBehaviour _instantiatedProjectile = Instantiate(currentExecutingAction.projectile.gameObject, myAnimController.ProjectileSpawnPoint.transform.position, Quaternion.identity).GetComponent<SpellBehaviour>();
                //_instantiatedProjectile.transform.SetParent(gameObject.transform);
                yield return new WaitForSeconds(0.25f);
                _instantiatedProjectile.Execute(myAnimController.ProjectileSpawnPoint.position, target);
            }
            else
            {
                yield return new WaitForSeconds(0.25f);
                for (int i = 0; i < CombatManager.instance.enemiesOnField.Count; i++)
                {
                    SpellBehaviour _instantiatedProjectile = Instantiate(currentExecutingAction.projectile.gameObject, myAnimController.ProjectileSpawnPoint.position, Quaternion.identity).GetComponent<SpellBehaviour>();
                    _instantiatedProjectile.transform.SetParent(this.gameObject.transform);
                    _instantiatedProjectile.Execute(myAnimController.ProjectileSpawnPoint.position, CombatManager.instance.enemiesOnField[i]);
                }
                yield return new WaitForSeconds(0.25f);
            }
        }

        CombatManager.instance.HideAllFriendlyTargetPointers();

        yield return new WaitForSeconds(currentExecutingAction.animationCycle.cycleTime - 0.25f);

        StartCoroutine(ApplyDamageOrHeal(target));

        if (currentExecutingAction.actionType == ActionType.ITEM)
        {
            inventory.ConsumeItem(currentConsumableItemIndex);
            target.myAnimController.PlayHealingEffect();
        }

        yield return new WaitForSeconds(0.25f);

        GoBackToStartingPositionAndSetToIdle();
        OnSkillEnded?.Invoke();
        CombatManager.instance.RemoveFromCombatQueue(this);
        isDoingCritDamageAction = false;
        ChangeBattleState(BattleState.RECHARGING);
    }

    #endregion

    #region STATE MACHINE

    public void ChangeBattleState(BattleState phase)
    {
        StartCoroutine(ChangeBattleStateCoroutine(phase));
    }

    IEnumerator ChangeBattleStateCoroutine(BattleState p)
    {
        yield return new WaitForSeconds(0.02f);
        CurrentBattlePhase = p;

        switch (CurrentBattlePhase)
        {
            case BattleState.RECHARGING:
                //currentPreAction = recharging;
                //currentExecutingAction = recharging;
                currentPreAction = SelectAction(ActionType.RECHARGING);
                currentExecutingAction = currentPreAction;
                StartCoroutine(RechargingCoroutine());
                SetToIdle();
                break;

            case BattleState.READY:

                if (uiController.FindMainBattlePanel())
                {
                    if (!PhotonNetwork.IsConnected)
                    {
                        if (CombatManager.instance.CurrentActivePlayer == null)
                            CombatManager.instance.SetCurrentActivePlayer(this);
                        else if (CombatManager.instance.CurrentActivePlayer == this)
                            UIController.ShowMainBattlePanel();
                        else CombatManager.instance.LookForReadyPlayer();
                        //CombatManager.instance.LookForReadyPlayer();
                    }

                    else
                    {
                        if (!MyPhotonView.IsMine)
                            uiController.ShowChatBubble();
                        else
                        {
                            CombatManager.instance.SetCurrentActivePlayer(this);
                            uiController.HideChatBubble();
                        }
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
                //StopAllCoroutines();

                if (!GetComponent<EnemyBehaviour>())
                {
                    myAnimController.PlayAnimation(myAnimController.DeadAnimationName);
                    Material _mat = GetComponentInChildren<SpriteRenderer>().material;
                    _mat.SetFloat("_GreyscaleBlend", 1);
                    uiController.HideCanvas(10, 0);

                    if (CombatManager.instance.CurrentActivePlayer == this)
                        CombatManager.instance.LookForReadyPlayer();

                    StartCoroutine(CombatManager.instance.ShowGameOverIfNeeded_Coroutine());

                }
                else
                {
                    myAnimController.DisableAnimator();
                    uiController.HideCanvas(5, .25f);
                    combatEffects.DieEffect();
                    CombatManager.instance.RemoveFromField(this.GetComponent<EnemyBehaviour>());

                    yield return new WaitForSeconds(.1f);

                    if (CombatManager.instance.CurrentActivePlayer == null)
                        break;

                    if (CombatManager.instance.CurrentActivePlayer.CurrentTarget == this &&
                        CombatManager.instance.CurrentActivePlayer.CurrentBattlePhase == BattleState.PICKING_TARGET)
                    {
                        Debug.LogWarning("I'm dead! Picking other");
                        CombatManager.instance.SetTargetedEnemyByIndex(0);
                    }
                }
                break;

            case BattleState.GAMEWIN:
                //GoBackToStartingPosition();
                myAnimController.PlayAnimation(myAnimController.IdleAnimationName);
                uiController.HideCanvas();
                yield return new WaitForSeconds(.5f);
                StopAllCoroutines();
                break;

            case BattleState.NULL:
                break;
            default:
                break;
        }
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

    #endregion

    #region GAME FLOW

    public void ShowDescription(int skillIndex)
    {
        //uiController.ShowDescriptionTooltip(skills[skillIndex].description);
        UIController.ShowDescriptionTooltip(characterActions[2 + skillIndex].description);
    }

    public void SwapActiveCharacter()
    {
        StartCoroutine(SwapActiveCharacter_Coroutine());
    }

    IEnumerator SwapActiveCharacter_Coroutine()
    {
        yield return new WaitForSeconds(0.02f);
        UIController.HideMainBattlePanel();
        CombatManager.instance.PickNextReadyCharacter();
    }

    public void GameOver_Win()
    {
        //StopAllCoroutines();
        ChangeBattleState(BattleState.GAMEWIN);
    }

    #endregion

    #region ONLINE

    [PunRPC]
    public void SyncExecuteAction(int viewID, int actionIndex)
    {
        currentPreAction = characterActions[actionIndex];
        //Debug.LogWarning("Received RPC action => " + currentPreAction.actionType);
        currentTarget = PhotonView.Find(viewID).GetComponent<CharacterBehaviour>();
        CombatManager.instance.AddToCombatQueue(this);
        uiController.HideChatBubble();

        StartCoroutine(ExecuteActionCoroutine(currentTarget));
    }

    [PunRPC]
    public void SyncIsDoingCritDamage(bool result)
    {
        //Debug.LogWarning("Receiving CRIT DAMAGE bool = " + result);
        isDoingCritDamageAction = result;
    }

    [PunRPC]
    void SyncCalculatedValue(int amount)
    {
        syncedCalculatedValue = amount;
        //Debug.LogWarning("Receiving dmg amount = " + syncedCalculatedValue);
    }

    #endregion


}
