using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "InventoryItem/New Inventory Item")]
public class InventoryItemData : ScriptableObject
{
    public enum ItemType
    {
        CONSUMABLE,
        EQUIPMENT,
        NULL
    }
    public enum ConsumableType
    {
        HP_POTION,
        MANA_POTION,
        BERSERK_POTION,
        NULL
    }

    public string itemName;
    public string itemDescription;
    public Sprite itemSprite;
    public ItemType itemType;
    public ConsumableType consumableType;
    public int effectAmount;
    public int effectTurns;

public void ApplyItemEffect()
{
        CharacterBehaviour currentPlayer = CombatManager.instance.CurrentActivePlayer;

    switch (this.consumableType)
        {
            case ConsumableType.HP_POTION:
                currentPlayer.IncreaseHP(effectAmount);
                break;
            case ConsumableType.MANA_POTION:
                currentPlayer.IncreaseMP(effectAmount);
                break;
            case ConsumableType.BERSERK_POTION:
                break;
        }
}
}
