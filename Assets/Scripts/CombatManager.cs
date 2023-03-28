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

    EnemyBehaviour currentActiveEnemy = null;

    [SerializeField] float globalIntervalBetweenActions = 1f;
    public float GlobalIntervalBetweenActions => globalIntervalBetweenActions;

    [SerializeField] float globalEnemyAttackCD = 5f;
    float originalGlobalEnemyAttackCDValue;
    float currentGlobalEnemyAttackCD;

    [SerializeField] float internalPlayerCD = 2f;
    float currentInternalPlayerCD;

    [SerializeField]int totalXPEarned = 0;
    [SerializeField] VictoryScreen victoryScreen;
    [SerializeField] GameOverScreen gameOverScreen;

    [Header("COMBAT QUEUE")]
    public List<Transform> combatQueue = new List<Transform>();

    public bool FieldIsClear()
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
        currentGlobalEnemyAttackCD = originalGlobalEnemyAttackCDValue = globalEnemyAttackCD;
        currentInternalPlayerCD = 0;
    }

    private void Update()
    {

        if (FieldIsClear())
        {
            if (enemiesOnField.Count == 0)
                return;

            if (!GameManager.instance.gameStarted)
                return;

            currentGlobalEnemyAttackCD -= Time.deltaTime;
            currentGlobalEnemyAttackCD = Mathf.Clamp(currentGlobalEnemyAttackCD, 0, globalEnemyAttackCD);

            currentInternalPlayerCD -= Time.deltaTime;
            currentInternalPlayerCD = Mathf.Clamp(currentInternalPlayerCD, 0, internalPlayerCD);

            if (EnemyCanAttack() && !IsGameOver())
            {
                currentActiveEnemy = enemiesOnField[Random.Range(0, enemiesOnField.Count)];
                currentActiveEnemy.AttackRandomPlayer();

            }
        }

    }

    public void ResetGlobalEnemyAttackCD()
    {
        currentActiveEnemy = null;
        globalEnemyAttackCD = Random.Range(originalGlobalEnemyAttackCDValue - 0.5f, originalGlobalEnemyAttackCDValue + 1f);
        currentGlobalEnemyAttackCD = globalEnemyAttackCD;
    }

    public void ResetInternalPlayerActionCD()
    {
        currentInternalPlayerCD = internalPlayerCD;
    }

    public bool EnemyCanAttack()
    {
        return currentGlobalEnemyAttackCD <= 0 && currentActiveEnemy == null;
    }

    public bool PlayerCanAttack()
    {
        return currentInternalPlayerCD <= 0;
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

        if (currentActivePlayer != null)
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
        //Debug.LogWarning("SETTING CURRENT ACTIVE PLAYER: " + c);
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
            StartCoroutine(CheckWinConditionCoroutine());
        }
        else playersOnField.Remove(c);
    }

    IEnumerator CheckWinConditionCoroutine()
    {

        if (enemiesOnField.Count == 0)
        {
            GameManager.instance.gameWon = true;

            foreach (CharacterBehaviour p in playersOnField)
            {
                p.GameOver_Win();
            }
            yield return new WaitForSeconds(2.75f);
            victoryScreen.ShowScreen();
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
