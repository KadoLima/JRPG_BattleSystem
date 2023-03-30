using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public void OnMenus_Confirm(InputValue value)
    {

        if (GameManager.instance.GameWon)
            return;

        CharacterBehaviour _activePlayer = CombatManager.instance.CurrentActivePlayer;

        if (_activePlayer == null)
            return;

        if (_activePlayer.CurrentBattlePhase == BattleState.PICKING_TARGET)
        {
            if (_activePlayer.CurrentPreAction.IsHarmful)
            {
                _activePlayer.ExecuteActionOn(CombatManager.instance.enemiesOnField[CombatManager.instance.CurrentTargetEnemyIndex]);
            }
            else
            {
                _activePlayer.ExecuteActionOn(CombatManager.instance.playersOnField[CombatManager.instance.CurrentFriendlyTargetIndex]);
            }

            CombatManager.instance.combatQueue.Add(_activePlayer.transform);
        }
    }

    public void OnMenus_Back(InputValue value)
    {
        CharacterBehaviour _activePlayer = CombatManager.instance.CurrentActivePlayer;

        if (_activePlayer == null)
            return;
        
        if (_activePlayer.CurrentBattlePhase == BattleState.SELECTING_TECH ||
            _activePlayer.CurrentBattlePhase == BattleState.SELECTING_ITEM ||
            _activePlayer.CurrentBattlePhase == BattleState.PICKING_TARGET)
        {
            CombatManager.instance.HideAllEnemyPointers();
            CombatManager.instance.HideAllFriendlyTargetPointers();
            _activePlayer.ChangeBattleState(BattleState.READY);
        }
    }

    public void OnSwapActiveCharacter(InputValue value)
    {
        CharacterBehaviour _activePlayer = CombatManager.instance.CurrentActivePlayer;

        if (_activePlayer == null)
            return;
            
        CharacterUIController _uiController = _activePlayer.GetComponent<CharacterUIController>();

        if (_activePlayer.CurrentBattlePhase == BattleState.DEAD || CombatManager.instance.ReadyPlayersAmount() <= 1 || !_uiController)
            return;

        if (_uiController.GetBattlePanel() && _activePlayer.CurrentBattlePhase == BattleState.READY)
        {
            _activePlayer.SwapActiveCharacter();
        }
    }

    public void OnTargetNavigationUP(InputValue value)
    {
        CharacterBehaviour _activePlayer = CombatManager.instance.CurrentActivePlayer;

        if (_activePlayer == null)
            return;

        if (_activePlayer.CurrentBattlePhase == BattleState.PICKING_TARGET && !_activePlayer.CurrentPreAction.isAreaOfEffect)
        {
            if (_activePlayer.CurrentPreAction.IsHarmful)
                CombatManager.instance.IncreaseTargetEnemyIndex();
            else CombatManager.instance.IncreaseFriendlyTargetIndex();
        }
    }

    public void OnTargetNavigationDOWN(InputValue value)
    {
        CharacterBehaviour _activePlayer = CombatManager.instance.CurrentActivePlayer;

        if (_activePlayer == null)
            return;

        if (_activePlayer.CurrentBattlePhase == BattleState.PICKING_TARGET && !_activePlayer.CurrentPreAction.isAreaOfEffect)
        {
            if (_activePlayer.CurrentPreAction.IsHarmful)
                CombatManager.instance.DecreaseTargetEnemyIndex();
            else CombatManager.instance.DecreaseFriendlyTargetIndex();
        }
    }
 
}
