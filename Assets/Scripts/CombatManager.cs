using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField] float globalEnemyAttackCD = 5f;
    float currentGlobalEnemyAttackCD;

    [SerializeField]int totalXPEarned = 0;
    [SerializeField] VictoryScreen victoryScreen;
    [SerializeField] GameOverScreen gameOverScreen;

    public bool FieldIsClear()
    {
        foreach (var p in playersOnField)
        {
            if (p.IsBusy())
                return false;
        }

        foreach (var e in enemiesOnField)
        {
            if (e.IsBusy())
                return false;
        }

        return true;
    }

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentGlobalEnemyAttackCD = globalEnemyAttackCD;
    }

    private void Update()
    {
        if (CurrentActivePlayer!= null && CurrentActivePlayer.CurrentBattlePhase == BattleState.PICKING_TARGET && !playersOnField[0].CurrentPreAction.isAreaOfEffect)
        {
            if (playersOnField[0].CurrentPreAction.IsHarmful)
                CycleThroughEnemyTargets();
            else CycleThroughFriendlyTargets();
        }

        if (FieldIsClear())
        {
            if (enemiesOnField.Count == 0)
                return;

            currentGlobalEnemyAttackCD -= Time.deltaTime;
            currentGlobalEnemyAttackCD = Mathf.Clamp(currentGlobalEnemyAttackCD, 0, globalEnemyAttackCD);

            if (EnemyCanAttack() && !IsGameOver())
            {
                enemiesOnField[Random.Range(0, enemiesOnField.Count)].AttackRandomPlayer();
                ResetGlobalEnemyAttackCD();
            }
        }

        //Debug.LogWarning(FieldIsClear());
    }

    public void ResetGlobalEnemyAttackCD()
    {
        globalEnemyAttackCD = Random.Range(3, 5);
        currentGlobalEnemyAttackCD = globalEnemyAttackCD;
    }

    public bool EnemyCanAttack()
    {
        return currentGlobalEnemyAttackCD <= 0;
    }

    public void AddToTotalXP(int amount)
    {
        totalXPEarned += amount;

       // Debug.LogWarning("TOTALXPEARNED = " + totalXPEarned);
    }

    public int TotalXPEarned()
    {
        return totalXPEarned;
    }

    public int ReadyPlayersAmount()
    {
        int _readyPlayers = 0;

        foreach (CharacterBehaviour c in playersOnField)
        {
            if (c.CurrentBattlePhase == BattleState.READY)
            {
                _readyPlayers++;
            }
        }

        return _readyPlayers;
    }

    public void LookForReadyPlayer()
    {
        if (currentActivePlayer != null)
        {
            return;
        }

        foreach (CharacterBehaviour c in playersOnField)
        {
            if (c.CurrentBattlePhase == BattleState.READY)
            {
                SetCurrentActivePlayer(c);
                return;
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
        int index = GetCurrentActivePlayerIndex();
        index++;

        if (index == playersOnField.Count)
            index = 0;

        Debug.LogWarning(index);
        SetCurrentActivePlayer(playersOnField[index]);
    }

    //public int RechargingPlayersAmount()
    //{
    //    int _rechargingPlayers = 0;

    //    foreach (CharacterBehaviour c in playersOnField)
    //    {
    //        if (c.CurrentBattlePhase == BattleState.RECHARGING)
    //        {
    //            _rechargingPlayers++;
    //        }
    //    }

    //    return _rechargingPlayers;
    //}

    //public bool IsOnlyPlayerReady(CharacterBehaviour player)
    //{
    //    foreach (CharacterBehaviour c in playersOnField)
    //    {
    //        if (player != c && c.CurrentBattlePhase != BattleState.RECHARGING)
    //            return false;
    //    }

    //    return true;
    //}

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

    public void CycleThroughEnemyTargets()
    {
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            DecreaseTargetEnemyIndex();
        }

        else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            IncreaseTargetEnemyIndex();
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
            foreach (CharacterBehaviour p in playersOnField)
            {
                p.GameOver_Win();
            }
            yield return new WaitForSeconds(2);
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
        currentFriendlyTargetIndex--;

        if (currentFriendlyTargetIndex < 0)
            currentFriendlyTargetIndex = playersOnField.Count - 1;

        SetTargetedFriendlyTargetByIndex(currentTargetEnemyIndex);
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


    public void CycleThroughFriendlyTargets()
    {
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            DecreaseFriendlyTargetIndex();
        }

        else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            IncreaseFriendlyTargetIndex();
        }
    }

    #endregion


}
