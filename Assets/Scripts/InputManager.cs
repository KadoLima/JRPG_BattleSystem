using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    #region Inputs

    public void OnMenus_Confirm(InputValue value)
    {
        //if (IntroScreen.IntroDone == false)
        //    return;

        CharacterBehaviour _activePlayer = CombatManager.instance.CurrentActivePlayer;

        if (_activePlayer.CurrentBattlePhase == BattleState.PICKING_TARGET)
        {
            if (_activePlayer.CurrentPreAction.IsHarmful)
            {
                //Debug.LogWarning(CombatManager.instance.CurrentTargetEnemyIndex);
                _activePlayer.ExecuteActionOn(CombatManager.instance.enemiesOnField[CombatManager.instance.CurrentTargetEnemyIndex]);
            }
            else
            {
                //Debug.LogWarning(CombatManager.instance.CurrentFriendlyTargetIndex);
                _activePlayer.ExecuteActionOn(CombatManager.instance.playersOnField[CombatManager.instance.CurrentFriendlyTargetIndex]);
            }

            CombatManager.instance.combatQueue.Add(_activePlayer.transform);
        }
    }

    public void OnMenus_Back(InputValue value)
    {
        CharacterBehaviour _activePlayer = CombatManager.instance.CurrentActivePlayer;

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
        CharacterUIController _uiController = _activePlayer.GetComponent<CharacterUIController>();

        //Debug.LogWarning(CombatManager.instance.ReadyPlayersAmount());

        if (_activePlayer.CurrentBattlePhase == BattleState.DEAD || CombatManager.instance.ReadyPlayersAmount() <= 1 || !_uiController)
            return;

        //Debug.LogWarning("OnSwapActiveCharacter");


        if (_uiController.GetBattlePanel() && _activePlayer.CurrentBattlePhase == BattleState.READY)
        {
            //Debug.LogWarning("Changing active char!");
            _activePlayer.SwapActiveCharacter();
        }
    }

    public void OnTargetNavigationUP(InputValue value)
    {
        CharacterBehaviour _activePlayer = CombatManager.instance.CurrentActivePlayer;

        //Debug.LogWarning($"Active player is: {CombatManager.instance.CurrentActivePlayer}");

        if (_activePlayer.CurrentBattlePhase == BattleState.PICKING_TARGET && !_activePlayer.CurrentPreAction.isAreaOfEffect)
        {
            //Debug.LogWarning("UP 3");
            if (_activePlayer.CurrentPreAction.IsHarmful)
                CombatManager.instance.IncreaseTargetEnemyIndex();
            else CombatManager.instance.IncreaseFriendlyTargetIndex();
        }
    }

    public void OnTargetNavigationDOWN(InputValue value)
    {
        CharacterBehaviour _activePlayer = CombatManager.instance.CurrentActivePlayer;

        //Debug.LogWarning($"Active player is: {CombatManager.instance.CurrentActivePlayer}");

        if (_activePlayer.CurrentBattlePhase == BattleState.PICKING_TARGET && !_activePlayer.CurrentPreAction.isAreaOfEffect)
        {
            //Debug.LogWarning("DOWN 3");
            if (_activePlayer.CurrentPreAction.IsHarmful)
                CombatManager.instance.DecreaseTargetEnemyIndex();
            else CombatManager.instance.DecreaseFriendlyTargetIndex();
        }
    }
    #endregion
}
