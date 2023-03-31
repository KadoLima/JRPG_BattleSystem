using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum BattleState
{
    RECHARGING,
    READY,
    PICKING_TARGET,
    EXECUTING_ACTION,
    SELECTING_TECH,
    SELECTING_ITEM,
    WAITING,
    DEAD,
    GAMEWIN,
    NULL
}

public class CombatManager : MonoBehaviour
{
    public List<CharacterBehaviour> playersOnField = new List<CharacterBehaviour>();
    public List<EnemyBehaviour> enemiesOnField = new List<EnemyBehaviour>();

    public static CombatManager instance;

    int currentTargetEnemyIndex = 0;
    int currentFriendlyTargetIndex = 0;
    public int CurrentTargetEnemyIndex => currentTargetEnemyIndex;
    public int CurrentFriendlyTargetIndex => currentFriendlyTargetIndex;

    CharacterBehaviour currentActivePlayer = null;
    public CharacterBehaviour CurrentActivePlayer => currentActivePlayer;

    //EnemyBehaviour currentActiveEnemy = null;

    //[SerializeField] float globalIntervalBetweenActions = 1f;
    //public float GlobalIntervalBetweenActions => globalIntervalBetweenActions;


    //[SerializeField] float globalEnemyAttackCD = 5f;
    //float originalGlobalEnemyAttackCDValue;
    //float currentGlobalEnemyAttackCD;
    [SerializeField] float globalPlayerAttackCD = 2f;
    float currentGlobalPlayerAttackCD;

    [SerializeField]int totalXPEarned = 0;
    //[SerializeField] VictoryScreen victoryScreen;
    [SerializeField] GameOverScreen gameOverScreen;

    [Header("COMBAT QUEUE")]
    [SerializeField] List<CharacterBehaviour> combatQueue = new List<CharacterBehaviour>();
    //[SerializeField] float delayToLeaveQueue = 1;

    public bool IsFieldClear()
    {
        foreach (var p in playersOnField)
        {
            if (p.CurrentBattlePhase == BattleState.EXECUTING_ACTION)
                return false;
        }

        foreach (var e in enemiesOnField)
        {
            if (e.CurrentBattlePhase == BattleState.EXECUTING_ACTION)
                return false;
        }

        return true;
    }

    public bool IsAnyEnemyAttacking()
    {
        foreach (var e in enemiesOnField)
        {
            if (e.IsBusy())
                return true;
        }

        return false;
    }

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //currentGlobalEnemyAttackCD = originalGlobalEnemyAttackCDValue = globalEnemyAttackCD;
        currentGlobalPlayerAttackCD = 0;
    }

    private void Update()
    {

        //if (IsFieldClear())
        //
        if (enemiesOnField.Count == 0)
            return;

        if (!GameManager.instance.GameStarted)
            return;

        //currentGlobalEnemyAttackCD -= Time.deltaTime;
        //currentGlobalEnemyAttackCD = Mathf.Clamp(currentGlobalEnemyAttackCD, 0, globalEnemyAttackCD);

        currentGlobalPlayerAttackCD -= Time.deltaTime;
        currentGlobalPlayerAttackCD = Mathf.Clamp(currentGlobalPlayerAttackCD, 0, globalPlayerAttackCD);

        //if (!IsGameOver() && EnemyCanAttack())
        //if (!IsGameOver())
        //{
        //    //currentActiveEnemy = enemiesOnField[Random.Range(0, enemiesOnField.Count)];
        //    CurrentReadyEnemy().ExecuteActionOn(GetRandomPlayer());
        //    ResetGlobalEnemyAttackCD();
        //}
        //}

    }

    public EnemyBehaviour CurrentReadyEnemy()
    {
        if (enemiesOnField.Count == 1)
            return enemiesOnField[0];

        int _randomEnemyIndex = Random.Range(0, enemiesOnField.Count);

        Debug.LogWarning(_randomEnemyIndex);


        while (enemiesOnField[_randomEnemyIndex].CurrentBattlePhase == BattleState.EXECUTING_ACTION)
        {
            _randomEnemyIndex = Random.Range(0, enemiesOnField.Count);
        }

        var _currentEnemy = enemiesOnField[_randomEnemyIndex];
        return _currentEnemy;
    }



    public CharacterBehaviour GetRandomPlayer()
    {
        int _randomPlayerIndex = Random.Range(0, playersOnField.Count);

        while (playersOnField[_randomPlayerIndex].CurrentBattlePhase == BattleState.DEAD)
        {
            _randomPlayerIndex = Random.Range(0, playersOnField.Count);
        }

        var _currentPlayerTarget = playersOnField[_randomPlayerIndex];
        return _currentPlayerTarget;
    }

    public bool IsMyTurn(CharacterBehaviour c)
    {
        if (combatQueue.Count == 0)
            return false;

        //Debug.LogWarning($"Is it my turn? I'm {combatQueue[0].name}. ---> {combatQueue[0] == c}");
        return combatQueue[0] == c;
    }

    public void AddToCombatQueue(CharacterBehaviour characterToAdd)
    {
        combatQueue.Add(characterToAdd);
    }

    public void RemoveFromCombatQueue(CharacterBehaviour characterToRemove)
    {
        combatQueue.Remove(characterToRemove);
    }

    //IEnumerator RemoveFromQueueCoroutine(CharacterBehaviour characterToRemove)
    //{
    //    yield return new WaitForSeconds(delayToLeaveQueue);
    //    combatQueue.Remove(characterToRemove);
    //}


    //public void ResetGlobalEnemyAttackCD()
    //{
    //    Debug.LogWarning("RESETTING");
    //    currentActiveEnemy = null;
    //    //globalEnemyAttackCD = originalGlobalEnemyAttackCDValue;
    //    //globalEnemyAttackCD = Random.Range(originalGlobalEnemyAttackCDValue - 0.5f, originalGlobalEnemyAttackCDValue + 1f);
    //    //currentGlobalEnemyAttackCD = globalEnemyAttackCD;
    //}

    public void ResetInternalPlayerActionCD()
    {
        currentGlobalPlayerAttackCD = globalPlayerAttackCD;
    }

    //public bool EnemyCanAttack()
    //{
        //return currentGlobalEnemyAttackCD <= 0;
        //return currentGlobalEnemyAttackCD <= 0 && currentActiveEnemy == null;
    //}

    public bool PlayerCanAttack()
    {
        return currentGlobalPlayerAttackCD <= 0;
    }

    public void AddToTotalXP(int amount)
    {
        totalXPEarned += amount;
    }

    public int TotalXPEarned()
    {
        return totalXPEarned;
    }

    public int ReadyPlayersAmount()
    {
        int _playersReady = 0;

        foreach (CharacterBehaviour c in playersOnField)
        {
            if (c.CurrentBattlePhase == BattleState.READY && c.CurrentBattlePhase != BattleState.DEAD)
            {
                _playersReady++;
            }
        }

        return _playersReady;
    }

    public void LookForReadyPlayer()
    {
        StartCoroutine(LookForReadyPlayerCoroutine());
    }

    IEnumerator LookForReadyPlayerCoroutine()
    {
        yield return new WaitForSeconds(0.02f);

        if (currentActivePlayer != null && currentActivePlayer.CurrentBattlePhase != BattleState.DEAD)
        {
            //Debug.LogWarning("current Active Player: " +  currentActivePlayer);
            yield break;
        }

        foreach (CharacterBehaviour c in playersOnField)
        {
            if (c.CurrentBattlePhase == BattleState.READY)
            {
                //Debug.LogWarning("2222");
                SetCurrentActivePlayer(c);
                yield break;
            }
        }

        SetCurrentActivePlayer(null);
    }

    public void SetCurrentActivePlayer(CharacterBehaviour c)
    {
        currentActivePlayer = c;
        //Debug.LogWarning("SETTING CURRENT ACTIVE PLAYER: " + c);

        if (c != null)
            c.UIController.ShowBattlePanel();
    }

    public int GetCurrentActivePlayerIndex()
    {
        for (int i = 0; i < playersOnField.Count; i++)
        {
            if (currentActivePlayer == playersOnField[i])
                return i;
        }

        return -1;
    }

    public void PickAnotherReadyCharacter()
    {
        int _index = GetCurrentActivePlayerIndex();
        _index++;

        if (_index == playersOnField.Count)
            _index = 0;

        SetCurrentActivePlayer(playersOnField[_index]);
    }

    #region Enemy Target
    public void RandomEnemyStartAction(int forceEnemyIndex = -1)
    {
        EnemyBehaviour _rndEnemy;
        int _forceEnemyIndex = forceEnemyIndex;

        if (_forceEnemyIndex != -1)
            _rndEnemy = enemiesOnField[_forceEnemyIndex];
        else _rndEnemy = enemiesOnField[Random.Range(0, enemiesOnField.Count)];
    }

    public void SetTargetedEnemyByIndex(int index, bool isAreaOfEffect = false)
    {
        //Debug.LogWarning("Selecting enemy...");
        if (isAreaOfEffect)
        {
            currentTargetEnemyIndex = index;
            ShowAllEnemyPointers();
            return;
        }
        currentTargetEnemyIndex = index;

        for (int i = 0; i < enemiesOnField.Count; i++)
        {
            if (i == index)
                enemiesOnField[i].ShowPointer();
            else enemiesOnField[i].HidePointer();
        }
    }

    public void IncreaseTargetEnemyIndex()
    {
        currentTargetEnemyIndex++;

        if (currentTargetEnemyIndex > enemiesOnField.Count - 1)
            currentTargetEnemyIndex = 0;

        SetTargetedEnemyByIndex(currentTargetEnemyIndex);

    }

    public void DecreaseTargetEnemyIndex()
    {
        currentTargetEnemyIndex--;

        if (currentTargetEnemyIndex < 0)
            currentTargetEnemyIndex = enemiesOnField.Count - 1;

        SetTargetedEnemyByIndex(currentTargetEnemyIndex);
    }

    public void ShowAllEnemyPointers()
    {
        foreach (EnemyBehaviour enemy in enemiesOnField)
        {
            enemy.ShowPointer();
        }
    }

    public void HideAllEnemyPointers()
    {
        foreach (EnemyBehaviour enemy in enemiesOnField)
        {
            enemy.HidePointer();
        }
    }


    public void RemoveFromField_Delayed(CharacterBehaviour c)
    {
        StartCoroutine(RemoveFromField_Delayed_Coroutine(c));
    }

    IEnumerator RemoveFromField_Delayed_Coroutine(CharacterBehaviour c)
    {
        yield return new WaitForSeconds(0.02f);
        if (c.GetComponent<EnemyBehaviour>())
        {
            enemiesOnField.Remove(c.GetComponent<EnemyBehaviour>());
            CheckWinConditionCoroutine();
            //StartCoroutine(CheckWinConditionCoroutine());
        }
        else playersOnField.Remove(c);
    }

    void CheckWinConditionCoroutine()
    {

        if (enemiesOnField.Count == 0)
        {
            //GameManager.instance.GameWon = true;

            //foreach (CharacterBehaviour p in playersOnField)
            //{
            //    p.GameOver_Win();
            //}
            GameManager.instance.EndGame();
            //yield return new WaitForSeconds(2.75f);
            //victoryScreen.ShowScreen();
        }
    }

    bool IsGameOver()
    {

        foreach (CharacterBehaviour p in playersOnField)
        {
            if (p.CurrentBattlePhase != BattleState.DEAD)
            {
                return false;
            }
        }
        return true;
        
    }

    public IEnumerator ShowGameOverIfNeeded_Coroutine()
    {
        yield return new WaitForSeconds(0.1f);

        if (IsGameOver())
        {
            yield return new WaitForSeconds(1);
            gameOverScreen.ShowGameOverScreen();
        }
    }

    #endregion


    #region Friendly Target


    public void SetTargetedFriendlyTargetByIndex(int index, bool isAreaOfEffect = false)
    {
        //Debug.LogWarning("SELECTING FRIENDLY TARGET");

        if (isAreaOfEffect)
        {
            currentFriendlyTargetIndex = index;
            ShowAllFriendlyTargetPointers();
            return;
        }
        currentFriendlyTargetIndex = index;

        for (int i = 0; i < playersOnField.Count; i++)
        {
            if (i == index)
                playersOnField[i].ShowPointer();
            else playersOnField[i].HidePointer();
        }
    }


    public void IncreaseFriendlyTargetIndex()
    {
        currentFriendlyTargetIndex++;

        if (currentFriendlyTargetIndex > playersOnField.Count - 1)
            currentFriendlyTargetIndex = 0;

        SetTargetedFriendlyTargetByIndex(currentFriendlyTargetIndex);

    }

    public void DecreaseFriendlyTargetIndex()
    {
        //Debug.LogWarning(currentFriendlyTargetIndex);
        currentFriendlyTargetIndex--;

        if (currentFriendlyTargetIndex < 0)
            currentFriendlyTargetIndex = playersOnField.Count - 1;

        SetTargetedFriendlyTargetByIndex(currentFriendlyTargetIndex);
    }




    public void ShowAllFriendlyTargetPointers()
    {
        foreach (CharacterBehaviour character in playersOnField)
        {
            character.ShowPointer();
        }
    }

    public void HideAllFriendlyTargetPointers()
    {
        foreach (CharacterBehaviour character in playersOnField)
        {
            character.HidePointer();
        }
    }


    #endregion


}
