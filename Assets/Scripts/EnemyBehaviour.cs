using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnemyBehaviour : CharacterBehaviour
{
    [Header("ENEMY SPECIFIC PARAMETERS")]
    [SerializeField] float initialDelay;
    [SerializeField] float minAttackRate = 6;
    [SerializeField] float maxattackRate = 8;
    [SerializeField] int xpRewarded;
    [SerializeField] float chanceToUseSkill;
    public float ChanceToUseSkill => chanceToUseSkill;
    CharacterBehaviour currentPlayerTarget;
    public CharacterBehaviour CurrentPlayerTarget => currentPlayerTarget;

    public static Action<string> OnEnemyUsedSkill;

    int defaultSortingOrder;

    PhotonView photonView;

    public override void Start()
    {
        photonView = GetComponent<PhotonView>();

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

    public void SetTarget(CharacterBehaviour target)
    {
        currentPlayerTarget = target;

        if (PhotonNetwork.IsMasterClient && PhotonNetwork.InRoom)
        {
            photonView.RPC("UpdateTarget", RpcTarget.Others, target.photonView.ViewID);
            Debug.LogWarning("Im master client, sending RPC target -> " + target.photonView.ViewID);
        }
    }

    [PunRPC]
    void UpdateTarget(int viewID)
    {
        currentPlayerTarget = PhotonView.Find(viewID).GetComponent<CharacterBehaviour>();
        Debug.LogWarning("Updating current target via RPC! Current target is " + currentPlayerTarget);
    }

    public void SetRandomTarget()
    {
        int _randomPlayerIndex = UnityEngine.Random.Range(0, CombatManager.instance.playersOnField.Count);

        while (CombatManager.instance.playersOnField[_randomPlayerIndex].CurrentBattlePhase == BattleState.DEAD)
        {
            _randomPlayerIndex = UnityEngine.Random.Range(0, CombatManager.instance.playersOnField.Count);
        }

        SetTarget(CombatManager.instance.playersOnField[_randomPlayerIndex]);
        //currentPlayerTarget = CombatManager.instance.playersOnField[_randomPlayerIndex];
        //return currentPlayerTarget;
    }

    public override void ExecuteActionOn(CharacterBehaviour target=null)
    {
        StartCoroutine(AttackRandomPlayerCoroutine(target));
    }

    IEnumerator AttackRandomPlayerCoroutine(CharacterBehaviour target)
    {
        yield return new WaitUntil(() => GameManager.instance.GameStarted);

        yield return new WaitForSeconds(UnityEngine.Random.Range(initialDelay - .1f, initialDelay + .1f));

        while (CurrentBattlePhase != BattleState.DEAD)
        {
            //if (currentPlayerTarget == null)
            //    currentPlayerTarget = FindRandomPlayer();
            //if (currentPlayerTarget == null)
            //SetRandomTarget();
            //else SetTarget(target);
            //else currentPlayerTarget = target;

            if (!PhotonNetwork.InRoom || PhotonNetwork.IsMasterClient)
            {
                if (currentPlayerTarget == null)
                    SetRandomTarget();
                else SetTarget(target);
            }

            //else if (PhotonNetwork.IsMasterClient)
            //    SetRandomTarget();


            CombatManager.instance.AddToCombatQueue(this);

            ChangeBattleState(BattleState.WAITING);

            yield return new WaitUntil(() => CombatManager.instance.IsFieldClear() &&
                                             CombatManager.instance.IsMyTurn(this) &&
                                             currentPlayerTarget != null);

            ChangeBattleState(BattleState.EXECUTING_ACTION);
            SetCurrentAction();

            Debug.LogWarning("My target is: " + currentPlayerTarget);

            if (currentExecutingAction.goToTarget)
            {
                MoveToTarget(currentPlayerTarget);

                if (currentExecutingAction.actionType == ActionType.SKILL)
                    OnEnemyUsedSkill?.Invoke(currentExecutingAction.actionName);

                else if (currentExecutingAction.actionType == ActionType.NORMAL_ATTACK)
                {
                    var _rndValue = UnityEngine.Random.value;
                    isDoingCritDamageAction =  _rndValue > myStats.critChance ? false : true;
                }

                yield return new WaitForSeconds(myAnimController.SecondsToReachTarget);
                myAnimController.PlayAnimation(currentExecutingAction.animationCycle.name);
                GetComponentInChildren<SpriteRenderer>().sortingOrder = currentPlayerTarget.GetComponentInChildren<SpriteRenderer>().sortingOrder + 1;

                yield return new WaitForSeconds(currentExecutingAction.animationCycle.cycleTime - 0.25f);

                ApplyDamageOrHeal(currentPlayerTarget);

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
                CombatManager.instance.RemoveFromCombatQueue(this);
                ChangeBattleState(BattleState.RECHARGING);
                currentPlayerTarget = null;
            }

            yield return new WaitForSeconds(UnityEngine.Random.Range(minAttackRate, maxattackRate));
        }
    }

    public override void TakeDamageOrHeal(int amount, DamageType dmgType, bool isCrit)
    {
        if (dmgType == DamageType.HARMFUL)
        {
            currentHP -= amount;
            uiController.ShowFloatingDamageText(amount, dmgType, isCrit);

            if (currentHP <= 0)
            {
                Debug.LogWarning("ENEMY DIED!");
                CombatManager.instance.AddToTotalXP(xpRewarded);
                ChangeBattleState(BattleState.DEAD);

            }
        }

        uiController.RefreshHP(currentHP, myStats.baseHP);
    }

    private void SetCurrentAction(string s=null)
    {
        float _randomValue = UnityEngine.Random.value;

        if (_randomValue > chanceToUseSkill)
        {
            currentExecutingAction = normalAttack;
        }
        else
        {
            currentExecutingAction = Skills[0];
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Envie o target atual do inimigo apenas se este é o Master Client
            if (PhotonNetwork.IsMasterClient)
            {
                stream.SendNext(currentPlayerTarget);
            }
        }
        else
        {
            // Atualize o target do inimigo recebendo a informação do Master Client
            if (!PhotonNetwork.IsMasterClient)
            {
                currentPlayerTarget = (CharacterBehaviour)stream.ReceiveNext();
            }
        }
    }
}
