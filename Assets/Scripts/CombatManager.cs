using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState
{
    RECHARGING,
    READY,
    PICKING_TARGET,
    EXECUTING_ACTION,
    WAITING,
    DEAD,
    NULL
}

public class CombatManager : MonoBehaviour
{
    public List<CharacterBehaviour> playersOnField = new List<CharacterBehaviour>();
    public List<EnemyBehaviour> enemiesOnField = new List<EnemyBehaviour>();

    public static CombatManager instance;

    int currentTargetEnemyIndex = 0;
    public int CurrentTargetEnemyIndex => currentTargetEnemyIndex;

    public bool CanExecuteAction()
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

    }

    private void Update()
    {
        if (playersOnField[0].CurrentBattlePhase == BattleState.PICKING_TARGET)
        {
            CycleThroughTargets();
        }
    }

    public void RandomEnemyStartAction(int forceEnemyIndex=-1)
    {
        EnemyBehaviour _rndEnemy;
        int _forceEnemyIndex = forceEnemyIndex;

        if (_forceEnemyIndex != -1)
            _rndEnemy = enemiesOnField[_forceEnemyIndex];
        else _rndEnemy = enemiesOnField[Random.Range(0, enemiesOnField.Count)];

        ActionType _actionToExecute;

        if (Random.value > _rndEnemy.ChanceToUseSkill)
        {
            _actionToExecute = ActionType.NORMAL_ATTACK;
        }
        else
        {
            _actionToExecute = ActionType.SKILL_1;
        }

        _rndEnemy.StartAction(_actionToExecute, _rndEnemy.GetRandomPlayer());
    }

    public void SetTargetedEnemyByIndex(int index)
    {
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

        if (currentTargetEnemyIndex > enemiesOnField.Count-1)
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

    public void HideAllEnemyPointers()
    {
        foreach (EnemyBehaviour enemy in enemiesOnField)
        {
            enemy.HidePointer();
        }
    }

    public void CycleThroughTargets()
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




}
