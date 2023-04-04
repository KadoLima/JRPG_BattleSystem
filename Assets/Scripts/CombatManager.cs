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

    [Header("Global cooldown between actions")]
    [SerializeField] float globalCooldown = .75f;

    int totalXPEarned = 0;

    [Space(20)]
    [SerializeField] GameOverScreen gameOverScreen;

    [Header("COMBAT QUEUE")]
    [SerializeField] List<CharacterBehaviour> combatQueue = new List<CharacterBehaviour>();

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

    private void Update()
    {
        if (enemiesOnField.Count == 0)
            return;

        if (!GameManager.instance.GameStarted)
            return;

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

        return combatQueue[0] == c;
    }

    public void AddToCombatQueue(CharacterBehaviour characterToAdd)
    {
        combatQueue.Add(characterToAdd);
    }

    public void RemoveFromCombatQueue(CharacterBehaviour characterToRemove)
    {
        StartCoroutine(RemoveFromCombatQueueCoroutine(characterToRemove));
    }

    IEnumerator RemoveFromCombatQueueCoroutine(CharacterBehaviour characterToRemove)
    {
        yield return new WaitForSeconds(globalCooldown);
        combatQueue.Remove(characterToRemove);
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
            yield break;
        }

        foreach (CharacterBehaviour c in playersOnField)
        {
            if (c.CurrentBattlePhase == BattleState.READY)
            {
                SetCurrentActivePlayer(c);
                yield break;
            }
        }

        SetCurrentActivePlayer(null);
    }

    public void SetCurrentActivePlayer(CharacterBehaviour c)
    {
        currentActivePlayer = c;

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
                enemiesOnField[i].UIController.ShowPointer();
            else enemiesOnField[i].UIController.HidePointer();
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
            enemy.UIController.ShowPointer();
        }
    }

    public void HideAllEnemyPointers()
    {
        foreach (EnemyBehaviour enemy in enemiesOnField)
        {
            enemy.UIController.HidePointer();
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
            GameManager.instance.EndGame();
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
                playersOnField[i].UIController.ShowPointer();
            else playersOnField[i].UIController.HidePointer();
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
        currentFriendlyTargetIndex--;

        if (currentFriendlyTargetIndex < 0)
            currentFriendlyTargetIndex = playersOnField.Count - 1;

        SetTargetedFriendlyTargetByIndex(currentFriendlyTargetIndex);
    }

    public void ShowAllFriendlyTargetPointers()
    {
        foreach (CharacterBehaviour character in playersOnField)
            character.UIController.ShowPointer();
    }

    public void HideAllFriendlyTargetPointers()
    {
        foreach (CharacterBehaviour character in playersOnField)
            character.UIController.HidePointer();

    }


    #endregion


}
