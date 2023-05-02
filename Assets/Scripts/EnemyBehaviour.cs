using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

public class EnemyBehaviour : CharacterBehaviour
{
    [Header("ENEMY SPECIFIC PARAMETERS")]
    [SerializeField] float minAttackRate = 6;
    [SerializeField] float maxattackRate = 8;
    [SerializeField] int xpRewarded;
    [SerializeField] float chanceToUseSkill;
    public float ChanceToUseSkill => chanceToUseSkill;
    CharacterBehaviour currentPlayerTarget;
    public CharacterBehaviour CurrentPlayerTarget => currentPlayerTarget;

    public static Action<string> OnEnemyUsedSkill;

    int defaultSortingOrder;

    float waitTime;
    float randomizedInitialDelay;

    public override void Start()
    {
        myPhotonView = GetComponent<PhotonView>();

        currentPreAction = ScriptableObject.CreateInstance<CombatActionSO>();
        currentExecutingAction = ScriptableObject.CreateInstance<CombatActionSO>();

        defaultSortingOrder = GetComponentInChildren<SpriteRenderer>().sortingOrder;
        uiController = GetComponentInChildren<CharacterUIController>();
        CombatManager.instance.enemiesOnField.Add(this);
        originalPosition = transform.localPosition;
        currentHP = myStats.baseHP;

        transform.SetParent(CombatManager.instance.EnemiesParent);

        if (GameManager.instance.EnemiesWontAttack)
            return;

        ExecuteActionOn();
    }

    public void SetRandomTarget()
    {
        if (CombatManager.instance.AllPlayersDead())
            return;

        int _randomPlayerIndex = UnityEngine.Random.Range(0, CombatManager.instance.playersOnField.Count);

        while (CombatManager.instance.playersOnField[_randomPlayerIndex].CurrentBattlePhase == BattleState.DEAD)
        {
            _randomPlayerIndex = UnityEngine.Random.Range(0, CombatManager.instance.playersOnField.Count);
        }

        
        SetTarget(CombatManager.instance.playersOnField[_randomPlayerIndex]);
    }

    public void SetTarget(CharacterBehaviour target)
    {
        currentPlayerTarget = target;

        if (PhotonNetwork.IsMasterClient && PhotonNetwork.InRoom)
        {
            MyPhotonView.RPC(nameof(SyncTarget), RpcTarget.Others, currentPlayerTarget.MyPhotonView.ViewID);
            //Debug.LogWarning("Im master client, sending RPC target -> " + target.photonView.gameObject.name);
        }
    }

    [PunRPC]
    void SyncTarget(int viewID)
    {
        currentPlayerTarget = PhotonView.Find(viewID).GetComponent<CharacterBehaviour>();
        //Debug.LogWarning("Updating current target via RPC! Current target is " + currentPlayerTarget);
    }

    public override void ExecuteActionOn(CharacterBehaviour target=null)
    {
        StartCoroutine(AttackRandomPlayerCoroutine(target));
    }

    IEnumerator AttackRandomPlayerCoroutine(CharacterBehaviour target)
    {
        yield return new WaitUntil(() => GameManager.instance.GameStarted);

        if (PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected)
        {
            randomizedInitialDelay = UnityEngine.Random.Range(4 - .1f, 4 + .1f);

            if (PhotonNetwork.IsMasterClient)
                MyPhotonView.RPC(nameof(SyncInitialDelay), RpcTarget.Others, randomizedInitialDelay);
        }
        yield return new WaitForSeconds(.1f);
        yield return new WaitForSeconds(randomizedInitialDelay);

        while (CurrentBattlePhase != BattleState.DEAD)
        {

            if (!PhotonNetwork.IsConnected || PhotonNetwork.IsMasterClient)
            {
                if (currentPlayerTarget == null)
                    SetRandomTarget();
                else SetTarget(target);
            }

            //else if (PhotonNetwork.IsMasterClient)
            //    SetRandomTarget();

            yield return new WaitForSeconds(.1f); //syncing

            AddToCombatQueue();

            ChangeBattleState(BattleState.WAITING);

            yield return new WaitUntil(() => CombatManager.instance.IsFieldClear() &&
                                             CombatManager.instance.IsMyTurn(this) &&
                                             currentPlayerTarget != null);

            if (CombatManager.instance.AllPlayersDead())
            {
                ChangeBattleState(BattleState.WAITING);
                yield break;
            }

            ChangeBattleState(BattleState.EXECUTING_ACTION);

            SetRandomAction();

            //yield return new WaitUntil(() => currentExecutingAction.actionInfo.actionType != ActionType.NULL);

            //Debug.LogWarning("CURRENT ACTION: " + currentExecutingAction.actionType);

            if (currentPlayerTarget.CurrentBattlePhase == BattleState.DEAD)
                SetRandomTarget();

            if (currentExecutingAction.goToTarget)
            {
                MoveToTarget(currentPlayerTarget);

                if (currentExecutingAction.actionType == ActionType.SKILL)
                {
                    OnEnemyUsedSkill?.Invoke(currentExecutingAction.actionName);
                }

                else if (currentExecutingAction.actionType == ActionType.NORMAL_ATTACK)
                {
                    if (PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected)
                    {
                        var _rndValue = UnityEngine.Random.value;
                        isDoingCritDamageAction = _rndValue > myStats.critChance ? false : true;

                        if (PhotonNetwork.IsMasterClient)
                            MyPhotonView.RPC(nameof(SyncIsDoingCritDamageAction), RpcTarget.Others, isDoingCritDamageAction);
                    }
                }

                yield return new WaitForSeconds(myAnimController.SecondsToReachTarget);
                myAnimController.PlayAnimation(currentExecutingAction.animationCycle.name);
                GetComponentInChildren<SpriteRenderer>().sortingOrder = currentPlayerTarget.GetComponentInChildren<SpriteRenderer>().sortingOrder + 1;

                yield return new WaitForSeconds(currentExecutingAction.animationCycle.cycleTime - 0.25f);

                StartCoroutine(ApplyDamageOrHeal(currentPlayerTarget));

                yield return new WaitForSeconds(0.25f);

                if (currentExecutingAction.goToTarget)
                {
                    GetComponentInChildren<SpriteRenderer>().sortingOrder = defaultSortingOrder;
                    GoBackToStartingPosition();
                }

                OnSkillEnded?.Invoke();

                yield return new WaitForSeconds(.2f);
                myAnimController.PlayAnimation(myAnimController.IdleAnimationName);

                isDoingCritDamageAction = false;
                RemoveFromCombatQueue();
                ChangeBattleState(BattleState.RECHARGING);
                currentPlayerTarget = null;
                //currentExecutingAction.actionType = ActionType.RECHARGING;
            }

            if (PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected)
            {
                waitTime = UnityEngine.Random.Range(minAttackRate, maxattackRate);

                if (PhotonNetwork.IsMasterClient)
                    MyPhotonView.RPC(nameof(SyncWaitTime), RpcTarget.Others, waitTime);
            }

            yield return new WaitForSeconds(.1f); //sync

            yield return new WaitForSeconds(waitTime);
        }
    }

    [PunRPC]
    void SyncInitialDelay(float value)
    {
        randomizedInitialDelay = value;
        //Debug.LogWarning("RECEIVING RPC FOR isDoingCritDmgAction. Value is: " + isDoingCritDamageAction);
    }

    [PunRPC]
    void SyncIsDoingCritDamageAction(bool value)
    {
        isDoingCritDamageAction = value;

        //Debug.LogWarning("RECEIVING RPC FOR isDoingCritDmgAction. Value is: " + isDoingCritDamageAction);
    }

    [PunRPC]
    void SyncWaitTime(float time)
    {
        waitTime = time;
        //Debug.LogWarning("RECEIVING WAIT TIME RPC. waitTime = " + waitTime);
    }

    [PunRPC]
    private void SyncCurrentAction(int actionIndex)
    {
        if (actionIndex == 0)
            currentExecutingAction = SelectAction(ActionType.NORMAL_ATTACK);
        else currentExecutingAction = SelectAction(ActionType.SKILL);
        //currentExecutingAction = normalAttack;
        //else currentExecutingAction = Skills[0];

        //Debug.LogWarning("Receiving RPC currentAction -> " + currentExecutingAction.actionType);
    }

    [PunRPC]
    private void SyncCombatQueue(int[] combatQueueViewIDs)
    {
        CharacterBehaviour[] _combatQueueArray = new CharacterBehaviour[combatQueueViewIDs.Length];

        for (int i = 0; i < combatQueueViewIDs.Length; i++)
        {
            PhotonView photonView = PhotonView.Find(combatQueueViewIDs[i]);
            if (photonView != null)
            {
                _combatQueueArray[i] = photonView.GetComponent<CharacterBehaviour>();
            }
        }

        CombatManager.instance.CombatQueue = _combatQueueArray.ToList();

        //Debug.LogWarning("Receiving CombatQueue RPC. Length is " + combatQueueViewIDs.Length);

    }

    [PunRPC]
    void SyncCalculatedValue(int amount)
    {
        syncedCalculatedValue = amount;
        //Debug.LogWarning("Enemy receiving RPC. Dmg amount = " + syncedCalculatedValue);
    }

    //[PunRPC]
    //void SyncCalculatedValue(int amount)
    //{
    //    syncedCalculatedValue = amount;
    //    Debug.LogWarning("Receiving dmg amount = " + syncedCalculatedValue);
    //}

    void AddToCombatQueue()
    {
        if (!PhotonNetwork.IsConnected)
        {
            CombatManager.instance.AddToCombatQueue(this);
            return;
        }

        if (PhotonNetwork.IsMasterClient)
        {
            CombatManager.instance.AddToCombatQueue(this);

            CharacterBehaviour[] _combatQueueArray = CombatManager.instance.CombatQueue.ToArray();

            int[] combatQueueViewIDs = new int[_combatQueueArray.Length];

            for (int i = 0; i < _combatQueueArray.Length; i++)
            {
                combatQueueViewIDs[i] = _combatQueueArray[i].MyPhotonView.ViewID;
            }

            MyPhotonView.RPC(nameof(SyncCombatQueue), RpcTarget.Others, combatQueueViewIDs);

            //Debug.LogWarning("Sending CombatQueue RPC. Length is " + combatQueueViewIDs.Length);
        }
    }

    void RemoveFromCombatQueue()
    {
        CombatManager.instance.RemoveFromCombatQueue(this);

        //if (!PhotonNetwork.IsConnected)
        //{
        //    CombatManager.instance.RemoveFromCombatQueue(this);
        //    return;
        //}

        //if (PhotonNetwork.IsMasterClient)
        //{
        //    CombatManager.instance.RemoveFromCombatQueue(this);

        //    CharacterBehaviour[] _combatQueueArray = CombatManager.instance.CombatQueue.ToArray();

        //    int[] combatQueueViewIDs = new int[_combatQueueArray.Length];

        //    for (int i = 0; i < _combatQueueArray.Length; i++)
        //    {
        //        combatQueueViewIDs[i] = _combatQueueArray[i].photonView.ViewID;
        //    }

        //    photonView.RPC(nameof(SyncCombatQueue), RpcTarget.Others, combatQueueViewIDs);
        //}
    }


    public override void TakeDamageOrHeal(int amount, DamageType dmgType, bool isCrit)
    {
        if (dmgType == DamageType.HARMFUL)
        {
            currentHP -= amount;
            uiController.ShowFloatingDamageText(amount, dmgType, isCrit);

            if (currentHP <= 0)
            {
                //Debug.LogWarning("ENEMY DIED!");
                CombatManager.instance.AddToTotalXP(xpRewarded);
                ChangeBattleState(BattleState.DEAD);

            }
        }

        uiController.RefreshHP(currentHP, myStats.baseHP);
    }

    private void SetRandomAction(string s=null)
    {
        if (!PhotonNetwork.IsConnected || PhotonNetwork.IsMasterClient)
        {
            //Debug.LogWarning("Selecting a random action");
            float _randomValue = UnityEngine.Random.value;

            if (_randomValue > chanceToUseSkill)
                currentExecutingAction = SelectAction(ActionType.NORMAL_ATTACK);
            else
                currentExecutingAction = SelectAction(ActionType.SKILL);

            //Debug.LogWarning(currentExecutingAction);
            //if (_randomValue > chanceToUseSkill)
            //    currentExecutingAction = normalAttack;
            //else
            //    currentExecutingAction = Skills[0];

            if (PhotonNetwork.IsMasterClient)
            {
                int _actionIndex = -1;

                if (currentExecutingAction.actionType == ActionType.NORMAL_ATTACK)
                    _actionIndex = 0;
                else _actionIndex = 1;

                MyPhotonView.RPC(nameof(SyncCurrentAction), RpcTarget.Others, _actionIndex);
                //Debug.LogWarning("Sending RPC currentAction -> " + currentExecutingAction.actionType);
            }
        }
    }



    //void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    //{
    //    if (stream.IsWriting)
    //    {
    //        // Envie o target atual do inimigo apenas se este é o Master Client
    //        if (PhotonNetwork.IsMasterClient)
    //        {
    //            stream.SendNext(currentPlayerTarget);
    //            stream.SendNext(currentExecutingAction);
    //        }
    //    }
    //    else
    //    {
    //        // Atualize o target do inimigo recebendo a informação do Master Client
    //        if (!PhotonNetwork.IsMasterClient)
    //        {
    //            currentPlayerTarget = (CharacterBehaviour)stream.ReceiveNext();
    //            currentExecutingAction = (CombatAction)stream.ReceiveNext();
    //        }
    //    }
    //}
}
